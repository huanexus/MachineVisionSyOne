using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks.Dataflow;

namespace Vision.SerialSy
{

    public class SchemeIo
    {
        public SchemeDi Sdi { get; set; }
        public SchemeDo Sdo { get; set; }
        public static SchemeIo[] Schemes { get; set; }

        static SchemeIo()
        {
            Schemes = new SchemeIo[2];
            Schemes[0] = new SchemeIo(0);
            Schemes[1] = new SchemeIo(1);

            IoPoint[] ioGrabs = new IoPoint[2];
            for (int i = 0; i < 2; i++)
            {
                ioGrabs[i] = Schemes[i].Sdi["Grab"];
            }
       }

        public SchemeIo()
        {
            Sdi = new SchemeDi();
            Sdo = new SchemeDo();
        }

        public SchemeIo(int channel)
        {
            Sdi = SchemeDi.Schemes[channel];
            Sdo = SchemeDo.Schemes[channel];
        }
    }

    public class SchemeDi
    {
        public int Id { get; set; }
        public int Data { get; set; }
        public Dictionary<string, int> Info { get; set; }
        public Dictionary<string, int> Mask { get; set; }
        public Dictionary<string, int> Shift { get; set; }
        public Dictionary<string, IoPoint> Io { get; set; }

        
        public static SchemeDi[] Schemes { get; set; }
        public static SeatIo MySeatIo { get; set; }

        public ActionBlock<int> IoBlock { get; set; }
        public event EventHandler<GrabEventArgs> GrabEvent;

        public int TrigCount { get; set; }

        

        static SchemeDi()
        {
            MySeatIo = new SeatIo();
            Schemes = new SchemeDi[2];
            Schemes[0] = new SchemeDi(0, 5, 7, 56);         
            Schemes[1] = new SchemeDi(1, 16, 1, 0);
            Schemes[0].Shift["Info"] = 3;
        }
        public SchemeDi(): this(-1, 16, 1, 0)
        {
            
        }

        public SchemeDi(int id, int resetMask, int grabMask, int infoMask)
        {
            Id = id;            
            Data = 0;

            IoBlock = new ActionBlock<int>(x => ProcessIo(x));

            TrigCount = 0;

            Mask = new Dictionary<string, int>();
            Io = new Dictionary<string, IoPoint>();
            Info = new Dictionary<string, int>();
            Shift = new Dictionary<string, int>();
            Shift["Info"] = 0;
            Mask["Reset"] = resetMask;
            Mask["Grab"] = grabMask;
            Mask["Info"] = infoMask;
            
            Info["Grab"] = 0;
            Io["Grab"] = new IoPoint();

            Io["Grab"].TrigUp.Subscribe(x => MessageLogger.LogMessage(string.Format("CCD{0}第{1}次拍照", id, x + 1)));
          //  Io["Grab"].TrigDown.Subscribe(x => MessageLogger.LogMessage(string.Format("--CCD{0}第{1}次拍照完成", id, x + 1)));
            Io["Grab"].TrigUp.Subscribe(x => OnGrab(x));
            
        }

        private void OnGrab(int x)
        {
            if (GrabEvent != null)
            {
                int trig = x;
                GrabEvent(this, new GrabEventArgs(trig));
            }
        }

        private void ProcessIo(int x)
        {
            Data = x;
            GetReset();
            GetGrab();
            if (Mask["Info"] != 0)
            {
                int info = GetInfo();
                MySeatIo.Seat = info;
            }      

        }   
     
        public IoPoint this[string name]
        {
            get {return Io[name];}
        }

        public int GetInfo()
        {
            return (Data & Mask["Info"]) >> Shift["Info"];
        }

        public int GetGrab()
        {
            int info = (Data & Mask["Grab"]);

            if(info > 0 && info != Mask["Reset"])
            {
                TrigCount = Io["Grab"].TrigUpCount;
                if(Id == 0)
                {
                    TrigCount = info - 1;
                }
                Io["Grab"].TrigUpInfo = TrigCount;
                Io["Grab"].Process(true);
            }
            else
            {
                Io["Grab"].Process(false);
            }
            return info;
        }

        public void GetReset()
        {
            if(Mask["Reset"] > 0)
            {
                int info = (Data & Mask["Reset"]);
                if (info == Mask["Reset"])
                {
                    Io["Grab"].Reset();
                    MessageLogger.LogMessage(string.Format("CCD{0}初始化", Id));     
                }                  
            }            
        }
    }

    public class SchemeDo
    {
        public int Id { get; set; }
        public int Data { get; set; }
        public int Done { get; set; }
        public int Ok { get; set; }
        public int Ng { get; set; }      

        public Dictionary<string, int> Mask { get; set; }
        public Dictionary<string, DoPoint> Io { get; set; }
        public Dictionary<string, int> Info { get; set; }

        public Dictionary<string, int> Port { get; set; }
        public static SchemeDo[] Schemes { get; set; }

        public ActionBlock<int> IoBlock { get; set; }

        SerialSy Sy { get; set; }

       
        static SchemeDo()
        {
            Schemes = new SchemeDo[2];
            Schemes[0] = new SchemeDo(0, 0, 2, 1 );    // Done, Ok, Ng
            Schemes[1] = new SchemeDo(1, 0, 1, 2);   // Done, Ok, Ng
        }

        public SchemeDo(): this(-1, 0, 1, 2)
        {
            
        }

        public SchemeDo(int id, int portDone, int portOk, int portNg)
        {
            Id = id;
            Data = 0;
            IoBlock = new ActionBlock<int>(x => ProcessIo(x));
            Mask = new Dictionary<string, int>();
            Io = new Dictionary<string, DoPoint>();
            Port = new Dictionary<string, int>();
            Io["Done"] = new DoPoint(portDone);
            Io["Ok"] = new DoPoint(portOk);
            Io["Ng"] = new DoPoint(portNg);
            Port["Done"] = portDone;
            Port["Ok"] = portOk;
            Port["Ng"] = portNg;
            Mask["Done"] = 1 << portDone;
            Mask["Ok"] = 1 << portOk;
            Mask["Ng"] = 1 << portNg;
            Sy = null;
        }

        private void ProcessIo(int x)
        {
            Data = x;
        }

        public void SetDevice(SerialSy sy)
        {
            Sy = sy;

            foreach(var io in Io.Values)
            {
                io.Sy = sy;
            }
        }
        
    }
}
