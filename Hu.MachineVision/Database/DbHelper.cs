using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;


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

       public static void SendData(string pd_id)
       {
           var db = DbScheme.GetConnection("Mes");
           var mesData = db.Query<CcdMes>("select * from CcdMes").ToArray();

           foreach(var data in mesData)
           {
               string s = JsonConvert.SerializeObject(data);

               MachineVision.Ui.UiMainForm.LogMessage(s);
           }
       }
    }
}
