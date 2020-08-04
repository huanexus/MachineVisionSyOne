using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdDi
    {
        [Column("ccdId")]
        [NotNull]
        public int CcdId { get; set; }

        [Column("name")]
        [NotNull]
        public string Name { get; set; }

        [Column("port")]
        [NotNull]
        public int Port { get; set; }
    }

    public class CcdDo
    {
        [Column("ccdId")]
        [NotNull]
        public int CcdId { get; set; }

        [Column("name")]
        [NotNull]
        public string Name { get; set; }

        [Column("port")]
        [NotNull]
        public int Port { get; set; }
    }

    public class CcdSerial
    {
        [Column("ccdId")]
        [NotNull]
        [Unique]
        public int CcdId { get; set; }

        [Column("comPort")]
        [NotNull]
        public int ComPort { get; set; }
    }
}
