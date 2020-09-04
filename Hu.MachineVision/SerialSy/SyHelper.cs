using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hu.MachineVision.Ui;

namespace Hu.MachineVision.SerialSy
{
   public static class SyHelper
    {

       public static void AddUi(int ccd)
       {
           VirtualIo vIo = VirtualIo.GetDevice(ccd);
           VirtualIoUi io = new VirtualIoUi(vIo);

           var tabs = UiMainForm.MyTabs;

           var tp = tabs["Serial", ccd];

           io.Associate(tp);

           vIo.StartWatch();
       }

    }
}
