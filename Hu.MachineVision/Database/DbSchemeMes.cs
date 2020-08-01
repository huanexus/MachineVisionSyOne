using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Hu.MachineVision.Database
{
    public static class DbSchemeMes
    {

        public static SQLite.Net.SQLiteConnection Db { get; set; }
        static DbSchemeMes()
        {
            Db = DbScheme.Connections["Mes"];
            Db.CreateTable<CcdMes>();
        }

        public static void Create()
        {
            Dictionary<string, string>[] Infos = new Dictionary<string, string>[2];

            for (int i = 0; i < 2; i++)
            {
                Infos[i] = new Dictionary<string, string>();
            }

            Infos[0]["a"] = "1";
            Infos[0]["b"] = "2";
            Infos[1]["a"] = "3";
            Infos[1]["b"] = "4";

            int itemCount = Db.ExecuteScalar<int>("select count(*) from CcdMes");

            if(itemCount < 3)
            {
                AddData(0, Infos);
            }

            
        }

        public static void AddData(int ccdId, Dictionary<string, string>[] data)
        {
            var mesData = new CcdMes()
            {
                time = DateTime.Now,
                eq_id = string.Format("Ccd{0}", ccdId + 1),
                data = JsonConvert.SerializeObject(data),
            };

            Db.InsertOrIgnore(mesData);
        }

    }
}
