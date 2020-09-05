using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdRoi
    {
        [Column("ccdId")]
        [NotNull]
        public int CcdId { get; set; }

        [Column("brandId")]
        [NotNull]
        public int BrandId { get; set; }

        [Column("imageIndex")]
        [NotNull]
        public int ImageIndex { get; set; }

        [Column("x0")]
        public int X0 { get; set; }
        
        [Column("y0")]
        public int Y0 { get; set; }

        [Column("width")]
        public int Width { get; set; }

        [Column("height")]
        public int Height { get; set; }
    }
}
