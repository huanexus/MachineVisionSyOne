using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hu.MachineVision.Database
{
   public static class DbHelper
    {
       public static int GetUiParams(string name)
       {
           return DbScheme.GetUiParams(name);
       }

       public static int GetRunStatus(string name)
       {
           return DbScheme.GetRunStatus(name);
       }
    }
}
