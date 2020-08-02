using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

namespace Hu.Serial.SerialSy
{
   public class SYMVDIOHeader32
    {
        const string DLL_FILENAME = "SYMVDIO_D.dll";
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_ComPort_Connect", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_ComPort_Connect(int Connect_Num);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Disconnect", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Disconnect(int Connect_Num);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Set_PortNum_Config", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Set_PortNum_Config(int Connect_Num, int PortNum);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Get_PortNum_Config", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Get_PortNum_Config(int Connect_Num, ref int PortNum);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Slave_Init", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Slave_Init(int Connect_Num, int SlaveIP);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Slave_ConnectSts", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Slave_ConnectSts(int Connect_Num);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Slave_Connect", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Slave_Connect(int Connect_Num, int SlaveIP);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Set_SlaveIP", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Set_SlaveIP(int Connect_Num, int SlaveIP, int NewIP);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Get_SlaveIP", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Get_SlaveIP(int Connect_Num, ref int SlaveIPIndex);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_Get_AllSlaveIP", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_Get_AllSlaveIP(int Connect_Num, int[] AllIP, ref int IPcount);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DIO_SaveParamToFlash", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DIO_SaveParamToFlash(int Connect_Num, int SlaveIP);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DI_ReadPort", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DI_ReadPort(int Connect_Num, int SlaveIP, int DIPortNum, ref UInt16 PortStatus);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DI_ReadLine", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DI_ReadLine(int Connect_Num, int SlaveIP, ref UInt16 PortStatus);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DI_SetIntMode", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DI_SetIntMode(int Connect_Num, int SlaveIP, int DIPortNum, int IntMode);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DI_IntEnable", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DI_IntEnable(int Connect_Num, int SlaveIP, int DIPortNum, int IntEnable);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DI_GetIntStatus", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DI_GetIntStatus(int Connect_Num, int SlaveIP, ref int DIPortNum, ref int IntEvent);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DI_SetFilter", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DI_SetFilter(int Connect_Num, int SlaveIP, UInt16 Timer);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DI_GetFilter", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DI_GetFilter(int Connect_Num, int SlaveIP, ref UInt16 Timer);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DO_WritePort", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DO_WritePort(int Connect_Num, int SlaveIP, int DOPortNum, UInt16 PortStatus);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DO_WriteLine", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DO_WriteLine(int Connect_Num, int SlaveIP, UInt16 PortStatus);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DO_ReadBackPort", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DO_ReadBackPort(int Connect_Num, int SlaveIP, int DOPortNum, ref UInt16 PortStatus);
        [DllImport(DLL_FILENAME, EntryPoint = "SY_MV_DO_ReadBackLine", CallingConvention = CallingConvention.StdCall)]
        public static extern int SY_MV_DO_ReadBackLine(int Connect_Num, int SlaveIP, ref UInt16 PortStatus);
    }
}
