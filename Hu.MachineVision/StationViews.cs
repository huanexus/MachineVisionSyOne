using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Windows.Forms;

using Hu.MachineVision.VisionPro;

namespace Hu.MachineVision
{
   public static class StationViews
    {
       public static DataGridView GetDgv(string name, int ccdId)
       {
           DataGridView dgv = new DataGridView();
           switch(name)
           {
               case "Data":
                   dgv = StationViewData.Stations[ccdId][ccdId];
                   break;
               case "RawData":
                   dgv = StationViewRawData.Stations[ccdId][ccdId];
                   break;
               case "CompValue":
                   dgv = StationViewCompValue.Stations[ccdId][ccdId];
                   break;
               case "RefValue":
                   dgv = StationViewRefValue.Stations[ccdId][ccdId];
                   break;
               default:
                   break;
           }
           return dgv;
       }
    }
}
