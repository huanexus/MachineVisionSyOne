using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using System.Drawing;
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

       public static bool CheckResultData(this DataGridView dgv, int nRow, int nColumn)
       {
           bool result = true;
           int row1 = 1;
           int row2 = 0;
           int row0 = 2;

           double x1 = 0;
           double x2 = 0;
           double x0 = 0;

           int ColumnP1 = 1;
           int column = 2;
           for (int i = 0; i < nRow; i++)
           {
               for (int j = 0; j < nColumn; j++)
               {
                   row2 = 1 + 4 * i;
                   row1 = 2 + 4 * i;
                   row0 = 3 + 4 * i;
                   column = ColumnP1 + j;
                   x2 = (double)dgv.Rows[row2].Cells[column].Value;
                   x1 = (double)dgv.Rows[row1].Cells[column].Value;
                   x0 = (double)dgv.Rows[row0].Cells[column].Value;
                   if (x0 > x1 && x0 < x2)
                   {
                       dgv.Rows[row0].Cells[column].Style.ForeColor = Color.White;
                       dgv.Rows[row0].Cells[column].Style.BackColor = Color.Green;
                   }
                   else
                   {
                       dgv.Rows[row0].Cells[column].Style.ForeColor = Color.White;
                       dgv.Rows[row0].Cells[column].Style.BackColor = Color.Red;
                       result = false;
                   }
               }
           }
           return result;
       }
    }
}
