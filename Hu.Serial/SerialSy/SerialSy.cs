using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

using Nett;

namespace Vision.SerialSy
{
    public class SerialSy
    {
        public static string[] PortNames { get; set; }
        public static List<string> SyPortNames { get; set; }
        public static List<int> SlaveIPs { get; set; }
        public static List<SerialSy> Devices { get; set; }     
        public static int DeviceNum { get { return SyPortNames.Count; } }

        public static List<bool> SyPortsStatus { get; set; }
        private static int m_indexStyle = VisionAppConfig.IndexStyle;

        public string PortName { get; set; }
        public int PortNum { get; set; }
        public int ConnectNum { get; set; }
        public int SlaveIP { get; set; }       
        public int Channel { get; set; }

        object DeviceLocker = new object();
        static SerialSy()
        {
            SyPortNames = new List<string>();
            SlaveIPs = new List<int>();
            SyPortsStatus = new List<bool>();
            int slaveIP = 0;       

            PortNames = SerialPort.GetPortNames();

            TomlTable toml = Toml.ReadFile("Parameters.toml");
            TomlTable serialConfig = toml.Get<TomlTable>("Serial");

            int[] serialNo = serialConfig.Get<int[]>("SerialNo");
            PortNames = serialNo.Select(p => string.Format("COM{0}", p)).ToArray();            
            
            foreach (var port in PortNames)
            {
                if (IsSyPort(port, ref slaveIP))
                {
                    SyPortsStatus.Add(true);
                    SyPortNames.Add(port);
                    SlaveIPs.Add(slaveIP);
                }
                else
                {
                    SyPortsStatus.Add(false);
                    SyPortNames.Add(port);
                    SlaveIPs.Add(-1);
                }
            }
            
            Devices = new List<SerialSy>();            
            for(int i = 0; i < DeviceNum; i++)
            {
                Devices.Add(new SerialSy(i));               
            }
        }

        public SerialSy(int channel)
        {
            Channel = channel;
            PortName = SyPortNames[channel];
            PortNum = GetPortNum(PortName);
            ConnectNum = channel + 1;
            SlaveIP = SlaveIPs[channel];
        }

        public static int GetChannel(string portName)
        {
            int channel = -1;
            foreach(var device in Devices)
            {
                if(device.PortName == portName)
                {
                    channel = device.Channel;
                    break;
                }
            }
            return channel;
        }

        public static SerialSy GetDevice(int channel)
        {
            SerialSy sy = null;
            foreach (var device in Devices)
            {
                if (device.Channel == channel)
                {
                    sy = device;
                    break;
                }
            }
            return sy;
        }

        public static int GetPortNum(string portName)
        {
            string portString = portName.Substring(3);
            int portNum = int.Parse(portString);
            return portNum;
        }

        public static bool IsSyPort(string portName, ref int slaveIP)
        {
            int connectNum = 0;
            int portNum = GetPortNum(portName);
            int countIP = 0;
            int[] allIP = new int[256];

            bool result = false;
            bool isAvail = false;

            isAvail = SYMVDIO.SY_MV_DIO_Set_PortNum_Config(connectNum, portNum);
            if (isAvail)
            {
                SYMVDIO.SY_MV_DIO_Get_AllSlaveIP(connectNum, allIP, ref countIP);
                if (countIP > 0)
                {
                    slaveIP = allIP[0];
                    result = true;
                }
                SYMVDIO.SY_MV_DIO_Disconnect(connectNum);
            }

            return result;
        }

        public bool DiReadLine(ref ushort inputsta)
        {
            lock (DeviceLocker)
            {
                return SYMVDIO.SY_MV_DI_ReadLine(ConnectNum, SlaveIP, ref inputsta);
            }
        }

        public bool DoReadBackLine(ref ushort inputsta)
        {
            lock (DeviceLocker)
            {
                return SYMVDIO.SY_MV_DO_ReadBackLine(ConnectNum, SlaveIP, ref inputsta);
            }
        }

        private bool WritePort(int port, ushort status, string message = "")
        {
            bool result = SYMVDIO.SY_MV_DO_WritePort(ConnectNum, SlaveIP, port, status);
            if (result)
            {
                LogMessage(message);
            }
            else
            {
                LogMessage(string.Format("端口设置不成功,{0},{1}", port, status));
            }

            return result;
        }

        public bool WritePort(int port, bool status, string message = "")
        {
            bool isSuccess = false;
            lock (DeviceLocker)
            {
                isSuccess = WritePort(port, status ? (ushort)1 : (ushort)0, message);
            }

            return isSuccess;
        }

        public bool WriteLine(ushort status, string message = "")
        {
            bool result = false;

            lock(DeviceLocker)
            {
                result = SYMVDIO.SY_MV_DO_WriteLine(ConnectNum, SlaveIP, status);
            }            

            if (result)
            {
                LogMessage(message);
            }
            return result;
        }

        public bool WriteLine(bool status, string message)
        {
            bool isSuccess = false;

            lock (DeviceLocker)
            {
                isSuccess = WriteLine(status ? (ushort)1 : (ushort)0, message);
            }

            return isSuccess;
        }
 
        public void SetPortNumConfig()
        {
            var isAvail = SYMVDIO.SY_MV_DIO_Set_PortNum_Config(ConnectNum, PortNum);
            string message = isAvail ? string.Format("设置串口{0}成功", PortName) : string.Format("设置串口{0}失败", PortName);
            LogMessage(message);
        }

        public bool Connect()
        {
            SetPortNumConfig();
            bool isAvail = SYMVDIO.SY_MV_DIO_ComPort_Connect(ConnectNum);
            bool bSerial = false;

            if (isAvail)
            {
                LogMessage(string.Format("连接串口{0}成功", PortName));
                isAvail = SYMVDIO.SY_MV_DIO_Slave_Connect(ConnectNum, SlaveIP);
                if (isAvail)
                {
                    LogMessage(string.Format("连接模块{0}成功", Channel + m_indexStyle));
                    isAvail = SYMVDIO.SY_MV_DIO_Slave_Init(ConnectNum, SlaveIP);
                    if (isAvail)
                    {
                        LogMessage(string.Format("初始化模块{0}成功", Channel + m_indexStyle));
                        bSerial = true;
                    }
                    else
                    {
                        LogMessage(string.Format("初始化模块{0}失败", Channel + m_indexStyle));
                    }
                }
                else
                {
                    LogMessage(string.Format("连接模块{0}失败", Channel + m_indexStyle));
                }
            }
            else
            {
                LogMessage(string.Format("连接串口{0}失败", PortName));
            }

            return bSerial;
        }

        public bool Disconnect()
        {
            bool isAvail = false;
            lock (DeviceLocker)
            {
                isAvail = SYMVDIO.SY_MV_DIO_Disconnect(ConnectNum);
            }

            if (isAvail)
            {
                LogMessage(string.Format("断开串口{0}成功", PortName));                
            }
            else
            {
                LogMessage(string.Format("断开串口{0}失败", PortName));
            }

            return isAvail;
        }

        private void LogMessage(string message)
        {
            MessageLogger.LogMessage(message);
        }
    }
}
