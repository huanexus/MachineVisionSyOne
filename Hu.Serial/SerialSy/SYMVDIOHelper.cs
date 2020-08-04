using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;

using System.Threading.Tasks.Dataflow;

namespace Hu.Serial.SerialSy
{
    public static partial class SYMVDIO
    {
        public static BufferBlock<string> MessageBuffer { get; set; }

        static SYMVDIO()
        {
            MessageBuffer = new BufferBlock<string>();
        }
        public static bool CheckPortStatus(string portName, ref int slaveIP)
        {
            int connectNum = 0;
            int portNum = GetPortNum(portName);
            int countIP = 0;
            int[] allIP = new int[256];

            bool result = false;
            bool isAvail = false;

            isAvail = SY_MV_DIO_Set_PortNum_Config(connectNum, portNum);
            if (isAvail)
            {
                SY_MV_DIO_Get_AllSlaveIP(connectNum, allIP, ref countIP);
                if (countIP > 0)
                {
                    slaveIP = allIP[0];
                    result = true;
                }
                SY_MV_DIO_Disconnect(connectNum);
            }

            LogStatusMessage(string.Format("检查串口{0}", portName), result);
            return result;
        }

        public static bool Connect(this SyDevice device)
        {
            bool result = true;
            bool isAvail = false;

            SyInfo sy = device.Device;

            int connectNum = sy.GetConnectNum();
            string portName = sy.GetPortName();
            int portNum = sy.GetPortNum();
            sy.IsAvail = false;

            isAvail = sy.Check();
            if (!isAvail) result = false;
           

            isAvail = sy.Config();
            if (!isAvail) result = false;

            sy.Connect();
            if (!isAvail) result = false;

            sy.Init();
            if (!isAvail) result = false;

            if (result) sy.IsAvail = true;
            LogStatusMessage(string.Format("连接串口{0}", portName), sy.IsAvail);
            return result;
        }

        public static bool Disconnect(int connectNum)
        {
            bool isAvail = SY_MV_DIO_Disconnect(connectNum);
            LogStatusMessage(string.Format("断开连接{0}", connectNum), isAvail);
            return isAvail;            
        }

        public static bool Disconnect(this SyDevice device)
        {
            return Disconnect(device.ConnectNum);
        }


        public static int GetPortNum(string portName)
        {
            return int.Parse(portName.Substring(3));
        }

        public static bool LogStatusMessage(string message, bool status)
        {
            string s = status ? "成功" : "失败";
            string logMessage = string.Format("{0}, {1}", message, s);
            MessageBuffer.Post(logMessage);
            return status;
        }

    }

    public class SyInfo : ISerialDeviceInfo
    {
        public int Channel { get; set; }

        public string PortName { get; set; }

        public int SlaveIP { get; set; }

        public bool IsAvail { get; set; }

        public static Dictionary<int, SyInfo> Devices { get; set; }

        public static Dictionary<string, int> SyPorts { get; set; }

        static SyInfo()
        {
            Devices = new Dictionary<int, SyInfo>();
            SyPorts = new Dictionary<string, int>();
            SyPorts = GetAllPorts();
        }

        public SyInfo(int channel, string portName)
        {
            Channel = channel;
            PortName = portName;
            SlaveIP = 10;
            IsAvail = false;            
        }

        public static Dictionary<string, int> GetAllPorts()
        {
           var portNames = SerialPort.GetPortNames();           
            Dictionary<string, int> sy = new Dictionary<string,int>();
            int slaveIP = 10;
            bool isAvail = false;

            foreach(var portName in portNames)
            {
                isAvail = SYMVDIO.CheckPortStatus(portName, ref slaveIP);
                if(isAvail)
                {
                    sy.Add(portName, slaveIP);
                }
            }            

            return sy;
        }

        public static SyInfo GetDevice(int channel)
        {
            if(Devices.ContainsKey(channel))
            {
                return Devices[channel];
            }

            return null;
        }

        public static SyInfo AddDevice(int channel, string portName)
        {
            SyInfo device = null;
            if(!Devices.ContainsKey(channel))
            {
                device = new SyInfo(channel, portName);
                Devices[channel] = device;
            }

            return device;
        }

        public SyInfo this[int index]
        {
            get { return GetDevice(index); }
        }

        public int GetConnectNum()
        {
            return Channel + 1;
        }

        public int GetPortNum()
        {
            return int.Parse(PortName.Substring(3)); ;
        }

        public string GetPortName()
        {
            return PortName;
        }
   
        public bool Check()
        {          
            int slaveIP = 10;
            bool isAvail = SYMVDIO.CheckPortStatus(PortName, ref slaveIP);
            if (isAvail) SlaveIP = slaveIP;
            return isAvail;
        }

        public bool Config()
        {
            int connectNum = GetConnectNum();
            int portNum = GetPortNum();
            string portName = GetPortName();
            bool isAvail = SYMVDIO.SY_MV_DIO_Set_PortNum_Config(connectNum, portNum);
            SYMVDIO.LogStatusMessage(string.Format("设置串口{0}", portName), isAvail);
            return isAvail;
        }

        public bool Connect()
        {
            int connectNum = GetConnectNum();
            string portName = GetPortName();            
            bool isAvail = SYMVDIO.SY_MV_DIO_Slave_Connect(connectNum, SlaveIP);
            SYMVDIO.LogStatusMessage(string.Format("连接串口模块{0}", portName), isAvail);
            return isAvail;
        }

        public bool Init()
        {
            int connectNum = GetConnectNum();
            string portName = GetPortName();
            bool isAvail = SYMVDIO.SY_MV_DIO_Slave_Init(connectNum, SlaveIP);
            SYMVDIO.LogStatusMessage(string.Format("初始化串口模块{0}", portName), isAvail);
            return isAvail;
        }
    }
}
