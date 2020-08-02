using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hu.Serial.SerialSy
{
    public class SYMVDIOHeader : SYMVDIOHeader64
    {

    }
    public static partial class SYMVDIO
    { 
        public static bool SY_MV_DIO_ComPort_Connect(int connectNum)
        {
            return SYMVDIOHeader.SY_MV_DIO_ComPort_Connect(connectNum) == 1;
        }

        public static bool SY_MV_DIO_Disconnect(int connectNum)
        {
            return SYMVDIOHeader.SY_MV_DIO_Disconnect(connectNum) == 1;
        }

        public static bool SY_MV_DIO_Set_PortNum_Config(int connectNum, int portNum)
        {

            return SYMVDIOHeader.SY_MV_DIO_Set_PortNum_Config(connectNum, portNum) == 1;
        }

        public static bool SY_MV_DIO_Get_PortNum_Config(int connectNum, ref int portNum)
        {
            return SYMVDIOHeader.SY_MV_DIO_Get_PortNum_Config(connectNum, ref portNum) == 1;
        }

        public static bool SY_MV_DIO_Slave_Init(int connectNum, int slaveIP)
        {
            return SYMVDIOHeader.SY_MV_DIO_Slave_Init(connectNum, slaveIP) == 1;
        }

        public static bool SY_MV_DIO_Slave_Connect(int connectNum, int slaveIP)
        {
            return SYMVDIOHeader.SY_MV_DIO_Slave_Connect(connectNum, slaveIP) == 1;
        }

        public static bool SY_MV_DIO_Slave_ConnectSts(int connectNum)
        {
            return SYMVDIOHeader.SY_MV_DIO_Slave_ConnectSts(connectNum) == 1;
        }

        public static bool SY_MV_DIO_Set_SlaveIP(int connectNum, int slaveIP, int newIP)
        {
            return SYMVDIOHeader.SY_MV_DIO_Set_SlaveIP(connectNum, slaveIP, newIP) == 1;
        }

        public static bool SY_MV_DIO_Get_SlaveIP(int connectNum, ref int slaveIP)
        {
            return SYMVDIOHeader.SY_MV_DIO_Get_SlaveIP(connectNum, ref slaveIP) == 1;
        }

        public static bool SY_MV_DIO_SaveParamToFlash(int connectNum, int slaveIP)
        {
            return SYMVDIOHeader.SY_MV_DIO_SaveParamToFlash(connectNum, slaveIP) == 1;
        }

        public static bool SY_MV_DIO_Get_AllSlaveIP(int connectNum, int[] allIP, ref int countIP)
        {
            return SYMVDIOHeader.SY_MV_DIO_Get_AllSlaveIP(connectNum, allIP, ref countIP) == 1;
        }

        public static bool SY_MV_DI_ReadPort(int connectNum, int slaveIP, int diPortNum, ref ushort portStatus)
        {
            return SYMVDIOHeader.SY_MV_DI_ReadPort(connectNum, slaveIP, diPortNum, ref portStatus) == 1;
        }

        public static bool SY_MV_DI_ReadLine(int connectNum, int slaveIP, ref ushort portStatus)
        {
            return SYMVDIOHeader.SY_MV_DI_ReadLine(connectNum, slaveIP, ref portStatus) == 1;
        }

        public static bool SY_MV_DI_SetIntMode(int connectNum, int slaveIP, int diPortNum, int intMode)
        {
            return SYMVDIOHeader.SY_MV_DI_SetIntMode(connectNum, slaveIP, diPortNum, intMode) == 1;
        }

        public static bool SY_MV_DI_IntEnable(int connectNum, int slaveIP, int diPortNum, int intEnable)
        {
            return SYMVDIOHeader.SY_MV_DI_IntEnable(connectNum, slaveIP, diPortNum, intEnable) == 1;
        }

        public static bool SY_MV_DI_GetIntStatus(int connectNum, int slaveIP, ref int diPortNum, ref int intEvent)
        {
            return SYMVDIOHeader.SY_MV_DI_GetIntStatus(connectNum, slaveIP, ref diPortNum, ref intEvent) == 1;
        }

        public static bool SY_MV_DI_SetFilter(int connectNum, int slaveIP, ushort timer)
        {
            return SYMVDIOHeader.SY_MV_DI_SetFilter(connectNum, slaveIP, timer) == 1;
        }

        public static bool SY_MV_DI_GetFilter(int connectNum, int slaveIP, ref ushort timer)
        {
            return SYMVDIOHeader.SY_MV_DI_GetFilter(connectNum, slaveIP, ref timer) == 1;
        }

        public static bool SY_MV_DO_WritePort(int connectNum, int slaveIP, int doPortNum, ushort portStatus)
        {
            return SYMVDIOHeader.SY_MV_DO_WritePort(connectNum, slaveIP, doPortNum, portStatus) == 1;
        }

        public static bool SY_MV_DO_WriteLine(int connectNum, int slaveIP, ushort portStatus)
        {
            return SYMVDIOHeader.SY_MV_DO_WriteLine(connectNum, slaveIP, portStatus) == 1;
        }

        public static bool SY_MV_DO_ReadBackPort(int connectNum, int slaveIP, int doPortNum, ref ushort portStatus)
        {
            return SYMVDIOHeader.SY_MV_DO_ReadBackPort(connectNum, slaveIP, doPortNum, ref portStatus) == 1;
        }

        public static bool SY_MV_DO_ReadBackLine(int connectNum, int slaveIP, ref ushort portStatus)
        {
            return SYMVDIOHeader.SY_MV_DO_ReadBackLine(connectNum, slaveIP, ref portStatus) == 1;
        }
    }
}
