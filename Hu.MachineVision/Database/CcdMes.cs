using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdMes
    {
        [Column("pd_id")]
        [PrimaryKey, AutoIncrement]
        public Guid pd_id { get; set; }

        [Column("time")]
        [NotNull]
        public DateTime time { get; set; }

        [Column("eq_id")]
        [NotNull]
        public string eq_id { get; set; }

        [Column("data")]
        [NotNull]
        public string data { get; set; }
    }
}
