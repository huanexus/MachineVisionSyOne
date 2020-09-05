using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks.Dataflow;

namespace Hu.MachineVision
{
   public static class RunParams
    {
       public static Dictionary<int, ActionBlock<int>> CcdGrabBlock { get; set; }
       public static Dictionary<int, ActionBlock<int>> CcdOfflineBlock { get; set; }

       public static Dictionary<int, ActionBlock<int>> CcdCheckBlock { get; set; }

       public static Dictionary<int, ActionBlock<int>> CcdDisplayBlock { get; set; }

       static RunParams()
       {
           CcdGrabBlock = new Dictionary<int, ActionBlock<int>>();
           CcdOfflineBlock = new Dictionary<int, ActionBlock<int>>();
           CcdCheckBlock = new Dictionary<int, ActionBlock<int>>();
           CcdDisplayBlock = new Dictionary<int, ActionBlock<int>>();
       }
    }
}
