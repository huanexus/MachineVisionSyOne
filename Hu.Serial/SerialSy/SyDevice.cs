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

        public int DiPortCount { get { return 6; } }
        public int DoPortCount { get { return 6; } }

        public int ConnectNum { get { return Device.GetConnectNum(); } }
        public int SlaveIP { get { return Device.SlaveIP; } }

        object mLocker = new object();

        public SyDevice(int ccdId, string portName)
        {
            CcdId = ccdId;
            PortName = portName;
            Device = new SyInfo(Channel, PortName);
        }

        private bool mDiReadLine(ref ushort inputsta)
        {
            lock(mLocker)
            {
                return SYMVDIO.SY_MV_DO_ReadBackLine(ConnectNum, SlaveIP, ref inputsta);
            }
        }

        private bool mDoReadBackLine(ref ushort inputsta)
        {
            lock (mLocker)
            {
                return SYMVDIO.SY_MV_DO_ReadBackLine(ConnectNum, SlaveIP, ref inputsta);
            }
        }

        private bool mDoWritePort(int port, ushort status)
        {
            lock(mLocker)
            {
                return SYMVDIO.SY_MV_DO_WritePort(ConnectNum, SlaveIP, port, status);
            }            
        }

        private bool mDiReadPort(int port, ref ushort status)
        {
            lock(mLocker)
            {
                return SYMVDIO.SY_MV_DI_ReadPort(ConnectNum, SlaveIP, port, ref status);
            }
        }

        public bool DiReadLine(ref ushort inputsta)
        {
            return mDiReadLine(ref inputsta);
        }

        public bool DoReadBackLine(ref ushort inputsta)
        {
            return mDoReadBackLine(ref inputsta);
        }

        public bool DoWritePort(int port, bool status)
        {
            return mDoWritePort(port, status ? (ushort)1 : (ushort)0);
        }

        public bool DiReadPort(int port, ref ushort status)
        {
            return mDiReadPort(port, ref status);
        }
    }
}
