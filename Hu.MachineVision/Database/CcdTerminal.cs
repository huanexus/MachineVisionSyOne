using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdTerminal
    {
        [Column("ccdId")]
        [NotNull]
        public int CcdId { get; set; }

        [Column("partId")]
        [NotNull]
        public int PartId { get; set; }

        [Column("image")]
        public int Image { get; set; }

        [Column("row")]
        public int Row { get; set; }

        [Column("column")]
        public int Column { get; set; }
    }
}
