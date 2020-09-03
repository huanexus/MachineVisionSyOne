using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hu.Serial.SerialSy
{
    public class SyDevice
    {
        public int CcdId { get; set; }


        public string PortName { get; set; }

        public SyInfo Device { get; set; }

        public int Channel { get { return CcdId; } }

        public int ConnectNum { get { return Device.GetConnectNum(); } }
        public int SlaveIP { get { return Device.SlaveIP; } }

        object mLocker = new object();

        public SyDevice(int ccdId, string portName)
        {
            CcdId = ccdId;
            PortName = portName;
            Device = new SyInfo(Channel, PortName);
        }

        public bool DiReadLine(ref ushort inputsta)
        {
            lock(mLocker)
            {
                return SYMVDIO.SY_MV_DO_ReadBackLine(ConnectNum, SlaveIP, ref inputsta);
            }
        }

        public bool DoReadBackLine(ref ushort inputsta)
        {
            lock (mLocker)
            {
                return SYMVDIO.SY_MV_DO_ReadBackLine(ConnectNum, SlaveIP, ref inputsta);
            }
        }

        private bool WritePort(int port, ushort status, string message = "")
        {
            bool result = SYMVDIO.SY_MV_DO_WritePort(ConnectNum, SlaveIP, port, status);



            return result;
        }
    }
}
