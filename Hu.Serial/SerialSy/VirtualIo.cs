using System;
using System.Collections.Generic;

using System.Collections;

using System.Linq;


//using System.Threading.Tasks.Dataflow;

using System.Threading.Tasks;

using Hu.MachineVision.Database;
using Hu.MachineVision.Ui;

namespace Hu.Serial.SerialSy
{

    public partial class VirtualIo
    {
        public string[] Names { get; set; }
        public string[] Labels { get; set; }
        public int[] Values { get; set; }
        public BitArray[] Bits { get; set; }
        public BitArray[] oldBits { get; set; }
        public SyDevice Device { get; set; }
        public int Channel { get; set; }

        public int Id { get { return Channel; } }

        public int DiPortCount { get; set; }
        public int DoPortCount { get; set; }
        public System.Timers.Timer ScDiTimer { get; set; }
        public System.Timers.Timer ScDoTimer { get; set; }
        public static int DeviceNum { get; set; }
        public static List<VirtualIo> Devices { get; set; }

        public event EventHandler<SignalEventArgs> DiChanged;
        public event EventHandler<SignalEventArgs> DoChanged;

        public int[] mDiPorts { get; set; }
        public int TrigCount { get; set; }
        public Dictionary<string, int> DiPorts { get; set; }
        public Dictionary<string, int> DoPorts { get; set; }

        static VirtualIo()
        {
            DeviceNum = DbHelper.GetUiParams("CcdCount");
            Devices = new List<VirtualIo>();
            for (int i = 0; i < DeviceNum; i++)
            {
                Devices.Add(new VirtualIo(i));
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

            var db = DbScheme.Connections["Main"];
            var diQuery = db.Query<CcdDi>("select * from CcdDi where ccdId = ?", Id);
            var doQuery = db.Query<CcdDo>("select * from CcdDo where ccdId = ?", Id);

            DiPorts = new Dictionary<string, int>();
            foreach (var qurey in diQuery)
            {
                DiPorts[qurey.Name] = qurey.Port;
            }

            DoPorts = new Dictionary<string, int>();
            foreach (var qurey in doQuery)
            {
                DoPorts[qurey.Name] = qurey.Port;
            }

            int comPort = db.ExecuteScalar<int>("select comPort from CcdSerial where ccdId = ?", Id);
            string portName = "COM" + comPort;
            Device = new SyDevice(Id, portName);
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


        public static VirtualIo GetDevice(string portName)
        {
            VirtualIo io = null;
            foreach (var device in Devices)
            {
                var sy = device.Device;
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
            var device = Device;
            bool isAvail = device.DiReadLine(ref inputsta);
            if (!isAvail)
            {
                LogMessage(string.Format("CCD{0}读取DI失败", Id));
                return false;
            }
            Di = (int)inputsta;
            return true;
        }


        public void StartWatch()
        {
            if (Device != null)
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
            var device = Device;

            bool isAvail = device.DoReadBackLine(ref outputsta);
            if (!isAvail)
            {
                LogMessage(string.Format("CCD{0}读取DO失败", Id));
                return false;
            }

            Do = (int)outputsta;
            return true;
        }

        private void LogMessage(string message)
        {
         UiMainForm.LogMessage(message);
        }

        public int SetPort(string portName, string message = "")
        {
            int port = DoPorts[portName];
            Device.DoWritePort(port, true);
            return SetPort(port, message);
        }

        public int ResetPort(string portName, string message = "")
        {
            int port = DoPorts[portName];
            Device.DoWritePort(port, false);
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

