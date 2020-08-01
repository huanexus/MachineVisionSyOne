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
            Dictionary<string, string>[] Infos = new Dictionary<string, string>[3];

            for (int i = 0; i < 3; i++)
            {
                Infos[i] = new Dictionary<string, string>();
            }

            Infos[0]["image1"] = "d:\\image\\a.bmp";
            Infos[0]["image2"] = "d:\\image\\b.bmp";
            Infos[1]["x1"] = "3";
            Infos[1]["y1"] = "4";
            Infos[2]["x2"] = "5";
            Infos[2]["y2"] = "6";

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
