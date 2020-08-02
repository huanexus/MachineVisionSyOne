using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hu.Serial
{
    public interface ISerialDeviceInfo
    {
        int Channel { get; set; }      

        int GetConnectNum();
        int GetPortNum();
        string GetPortName();
    }


}
