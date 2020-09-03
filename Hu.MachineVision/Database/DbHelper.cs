using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SimpleTCP;




namespace Hu.MachineVision.Database
{
    public static class DbHelper
    {
        static DbHelper()
        {
        }

        private static void LogMessage(string message)
        {
            MachineVision.Ui.UiMainForm.LogMessage(message);
        }


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
