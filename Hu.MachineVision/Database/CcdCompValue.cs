using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
   public class CcdCompValue
    {
       [Column("ccdId")]
       [NotNull]
       public int CcdId { get; set; }
       [Column("brandId")]
       [NotNull]
       public int BrandId { get; set; }

       [Column("item")]
       [NotNull]
       public int Item { get; set; }

       [Column("Name")]
       public string Name { get; set; }

       [Column("label")]
       public string Label { get; set; }

       [Column("k")]
       [NotNull]
       public double K { get; set; }

       [Column("b")]
       [NotNull]
       public double B { get; set; }

       [Column("r0")]
       [NotNull]
       public double R0 { get; set; }
       [Column("r1")]
       [NotNull]
       public double R1 { get; set; }
       [Column("r2")]
       [NotNull]
       public double R2 { get; set; }
    }
}
