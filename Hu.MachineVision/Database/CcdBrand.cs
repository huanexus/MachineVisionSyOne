using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
   public class CcdBrand
    {
        [Column("brandId")]
        [Unique]
        public int BrandId { get; set; }

        [Column("brand")]
        [Unique]
        public string Brand { get; set; }

        [Column("brandAlias")]
        [Unique]
        public string BrandAlias { get; set; }
    }
}
