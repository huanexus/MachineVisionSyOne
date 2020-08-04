using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdParams
    {

        [Column("ccdId")]
        [NotNull]
        public int CcdId { get; set; }

        [Column("brandId")]
        [NotNull]
        public int BrandId { get; set; }

        [Column("paramId")]
        [NotNull]
        public int ParamId { get; set; }

        [Column("name")]
        [NotNull]
        public string Name { get; set; }

        [Column("data")]
        [NotNull]
        public double Data { get; set; }
    }
}
