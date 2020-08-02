using System;
using System.Collections.Generic;

using System.Collections;

using System.Linq;

using Nett;

//using System.Threading.Tasks.Dataflow;

using System.Threading.Tasks;


namespace Vision.SerialSy
{

    public partial class VirtualIo
    {
        public string[] Names { get; set; }
        public string[] Labels { get; set; }
        public int[] Values { get; set; }
        public BitArray[] Bits { get; set; }
        public BitArray[] oldBits { get; set; }
        public SerialSy Sy { get { return SerialSy.GetDevice(Channel); } }
        public int Channel { get; set; }

        public int Id { get { return CcdStations[Channel]; } set { CcdStations[Channel] = value; } }

        public int DiPortCount { get; set; }
        public int DoPortCount { get; set; }
        public System.Timers.Timer ScDiTimer { get; set; }
        public System.Timers.Timer ScDoTimer { get; set; }
        public static int DeviceNum { get; set; }
        public static List<VirtualIo> Devices { get; set; }

        public event EventHandler IoReset;
        public event EventHandler<SignalEventArgs> DiChanged;
        public event EventHandler<SignalEventArgs> DoChanged;

        public event EventHandler<TrigUpEventArgs> DiTrigUp;
       

        public int[] mDiPorts { get; set; }
        public int TrigCount { get; set; }
        public Dictionary<string, int> DiPorts { get; set; }
        public Dictionary<string, int> DoPorts { get; set; }
        public static Dictionary<int, int> CcdStations { get; set; }       

        public bool IsDoMask { get; set; }
        public bool IsDummyIo { get; set; }
        public int DummyDi { get; set; }
        public SchemeIo Scheme { get; set; }
        static VirtualIo()
        {
            DeviceNum = 2;           
            CcdStations = new Dictionary<int, int>();
            Devices = new List<VirtualIo>();
            for (int i = 0; i < DeviceNum; i++)
            {
                Devices.Add(new VirtualIo(i));
                CcdStations[i] = i;
            }

        }
        private VirtualIo(int channel)
        {
            Names = new string[] { "DI", "DO", };
            Labels = new string[] { "输入", "输出" };
            Values = new int[] { 0, 0 };
            Bits = new BitArray[] { new BitArray(8), new BitArray(8) };
            oldBits = new BitArray[] { new BitArray(8), new BitArray(8) };
            TrigCount = 0;
            Channel = channel;
            IsDummyIo = true;

            Scheme = SchemeIo.Schemes[channel];
            DoPorts = new Dictionary<string, int>();
            DoPorts["Done"] = Scheme.Sdo.Port["Done"];
            DoPorts["Ok"] = Scheme.Sdo.Port["Ok"];
            DoPorts["Ng"] = Scheme.Sdo.Port["Ng"];
            DiPortCount = 6;
            DoPortCount = 6;
            IsDoMask = false;
           
           // Scheme.Sdi.Io["Reset"].TrigUp.Subscribe(x => ResetAll());
            Scheme.Sdo.SetDevice(Sy);
        }

        public static VirtualIo GetDevice(int channel)
        {
            VirtualIo io = null;
            foreach (var device in Devices)
            {
                if (device.Channel == channel)
                {
                    io = device;
                    break;
                }
            }

            return io;
        }

        public static int GetChannel(int id)
        {
            int channel = -1;
            foreach (var device in Devices)
            {
                if (device.Id == id)
                {
                    channel = device.Channel;
                    break;
                }
            }

            return channel;
        }

        public static VirtualIo GetDevice(string portName)
        {
            VirtualIo io = null;
            foreach (var device in Devices)
            {
                var sy = SerialSy.GetDevice(device.Channel);
                if (sy != null)
                {
                    if (sy.PortName == portName)
                    {
                        io = device;
                        break;
                    }
                }
            }

            return io;
        }


        public int Di
        {
            get { return Values[0]; }
            set
            {
                if (Values[0] != value)
                {
                    int oldValue = Values[0];
                    Values[0] = value;
                    Scheme.Sdi.IoBlock.Post(value);
                    

                    if (DiChanged != null)
                    {
                        DiChanged(this, new SignalEventArgs(value, oldValue));
                    }
                }
            }
        }

        public int Do
        {
            get { return Values[1]; }
            set
            {
                if (Values[1] != value)
                {
                    int oldValue = Values[1];
                    Values[1] = value;

                    Scheme.Sdo.IoBlock.Post(value);
                    if (DoChanged != null)
                    {
                        DoChanged(this, new SignalEventArgs(value, oldValue));
                    }
                }
            }
        }       

        public bool GetDiStatus()
        {
            ushort inputsta = 0;
            if (SerialSy.SyPortsStatus[Sy.Channel])
            {
                bool isAvail = Sy.DiReadLine(ref inputsta);
                if (!isAvail)
                {
                    LogMessage(string.Format("CCD{0}读取DI失败", Id));
                    return false;
                }
                Di = (int)inputsta;
                return true;
            }

            return false;

        }


        public void StartWatch()
        {
            if (Sy != null)
            {
                ScDiTimer = new System.Timers.Timer(100);
                ScDiTimer.AutoReset = false;
                ScDiTimer.Elapsed += (s, e) => WatchDi(ScDiTimer);
                ScDiTimer.Start();
            }
        }

        public void WatchDi(System.Timers.Timer timer)
        {
            timer.Stop();
            GetDiStatus();
            timer.Start();
        }

        public void WatchDo(System.Timers.Timer timer)
        {
            timer.Stop();
            GetDoStatus();
            timer.Start();
        }

        public bool GetDoStatus()
        {
            ushort outputsta = 0;
            if (SerialSy.SyPortsStatus[Sy.Channel])
            {
                bool isAvail = Sy.DoReadBackLine(ref outputsta);
                if (!isAvail)
                {
                    LogMessage(string.Format("CCD{0}读取DO失败", Id));
                    return false;
                }
                Do = (int)outputsta;
                return true;
            }
            return false;
        }

        private void LogMessage(string message)
        {
            MessageLogger.LogMessage(message);
        }

        public int SetPort(string portName, string message = "")
        {
            int port = Scheme.Sdo.Port[portName];
            Scheme.Sdo.Io[portName].SetPort();
            return SetPort(port, message);
        }

        public int ResetPort(string portName, string message = "")
        {
            int port = Scheme.Sdo.Port[portName];
            Scheme.Sdo.Io[portName].ResetPort();
            return ResetPort(port, message);
        }

        public int SetPort(int port, string message = "")
        {
            Do = Do | (1 << port);
            return Do;
        }

        public int ResetPort(int port, string message = "")
        {
            Do = Do & (~(1 << port));
            return Do;
        }

        public void ResetAll()
        {
            string[] names = { "Done", "Ok", "Ng" };
            foreach (var name in names)
            {
                ResetPort(name);
            }
        }

        public void SetOk()
        {
            ResetPort("Ng");
            SetPort("Ok");          
        }

        public void SetNg()
        {
            ResetPort("Ok");
            SetPort("Ng");
        }

        public void SetDone()
        {
            SetPort("Done");
            LogMessage(string.Format("反馈拍照完成"));
          //  Task.Delay(300).ContinueWith(x => ResetPort("Done"));
        }

        public void ResetDone()
        {
            ResetPort("Done");
            LogMessage(string.Format("复位拍照完成信号"));
        }

        internal void ResetLastStatus()
        {
            ResetPort("Ok");
            ResetPort("Ng");
            ResetPort("Done");

        }
    }
}

