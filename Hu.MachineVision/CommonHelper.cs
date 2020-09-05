using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Forms;

namespace Hu.MachineVision
{
   public static class CommonHelper
    {
       public static void DgvLayout(this DataGridView dgv)
       {
           dgv.AllowUserToAddRows = false;

           dgv.RowHeadersVisible = false;
           dgv.ColumnHeadersVisible = false;
           
           for (int i = 0; i < dgv.Columns.Count; i++)
           {
               dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
              // dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
              //  dgv.Columns[i].MinimumWidth = 40;
               dgv.Columns[i].Width = 40;
           }

           if (dgv.ColumnCount > 10)
           {
               dgv.Columns[1].Frozen = true;
           }
       }

    }
}
