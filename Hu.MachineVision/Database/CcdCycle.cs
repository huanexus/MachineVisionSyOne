using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdCycle
    {
        [Column("id")]
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("ccdId")]
        public int CcdId { get; set; }

        [Column("brandId")]
        public int BrandId { get; set; }

        [Column("cycleAll")]
        public int All { get; set; }
        [Column("cycleOk")]
        public int Ok { get; set; }
        [Column("cycleNg")]
        public int Ng {get;  set; }
    }
}
