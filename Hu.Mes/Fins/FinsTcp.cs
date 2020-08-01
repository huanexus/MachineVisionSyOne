using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using mcOMRON;

namespace Hu.Mes.Fins
{

    public class FinsTcp
    {
        public static Dictionary<string, OmronPLC> Servers { get; set; }
        public static Dictionary<string, int> Ports { get; set; }

        public string IP { get; set; }
        public int Port { get { return Ports[IP];} }

        public OmronPLC PLC { get { return Servers[IP]; } }

        public FinsAddressDM AddressDM { get; set; }

        static FinsTcp()
        {
            Servers = new Dictionary<string, OmronPLC>();
            Ports = new Dictionary<string, int>();
        }

        public FinsTcp(string ip = "192.168.250.22", int port = 9600)
        {
            IP = ip;            
            if(!Servers.ContainsKey(ip))
            {
                OmronPLC plc = new OmronPLC(mcOMRON.TransportType.Tcp);
                mcOMRON.tcpFINSCommand tcpCommand = ((mcOMRON.tcpFINSCommand)plc.FinsCommand);
                tcpCommand.SetTCPParams(System.Net.IPAddress.Parse(ip), Convert.ToInt32(port));                
                plc.Connect();
                Servers[ip] = plc;
                Ports[ip] = port;
            }

            AddressDM = new FinsAddressDM();

            AddressDM["Ok"] = 30;
            AddressDM["Ng"] = 30;
            AddressDM["Online"] = 31;
            AddressDM["Timestamp"] = 30040;            
        }

        public bool WriteDM(ushort position, ushort value)
        {
            if(!PLC.Connected)
            {
                PLC.Connect();
            }
            return PLC.WriteDM(position, value);
        }

        public  bool ReadDM(ushort position, ref ushort value)
        {
            if(!PLC.Connected)
            {
                PLC.Connect();
            }
            return PLC.ReadDM(position, ref value);  
        }

        public bool WriteOk()
        {
            return WriteDM(AddressDM["OK"], 2);
        }

        public bool WriteNg()
        {
            return WriteDM(AddressDM["NG"], 4);
        }

        public bool WriteOnline()
        {
            return WriteDM(AddressDM["Online"], 1);
        }

        public bool WriteTimestamp(long timestamp)
        {
            byte[] timestampBytes = BitConverter.GetBytes(timestamp);
            ushort[] data = new ushort[4];
            int address0 = AddressDM["Timestamp"];
            bool bSuccess = true;
            for (int i = 0; i < 4; i++)
            {
                data[i] = BitConverter.ToUInt16(timestampBytes, i * 2);
                if(!WriteDM((ushort)(i + address0), data[i]))
                {
                    bSuccess = false;
                }      
            }

            return bSuccess;
        }

        public long ReadTimestamp()
        {
            ushort[] data = new ushort[4];
            byte[] dataBytes = new byte[8];
            int address0 = AddressDM["Timestamp"];
            bool bSuccess = true;            

            for (int i = 0; i < 4; i++)
            {
                bool _bSuccess = ReadDM((ushort)(address0 + i), ref data[i]);
                if(!_bSuccess)
                {
                    bSuccess = false;
                }

                byte[] bytes = BitConverter.GetBytes(data[i]);
                bytes.CopyTo(dataBytes, 2 * i);
            }

            long timestamp = BitConverter.ToInt64(dataBytes, 0);
            if(!bSuccess)
            {
                timestamp = 0;
            }

            return timestamp;            
        }        
    }

    public class FinsAddressDM
    {
        public Dictionary<string, ushort> Address { get; set; }

        public FinsAddressDM()
        {
            Address = new Dictionary<string, ushort>();            
        }

        public ushort this[string name]
        {
            get { return Address[name]; }
            set { Address[name] = (ushort)value; }
        }
    }
}
