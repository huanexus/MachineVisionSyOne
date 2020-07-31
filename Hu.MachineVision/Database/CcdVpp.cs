using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdVpp
    {
        [Column("ccdId")]
        [Unique]
        public int CcdId { get; set; }

        [Column("home")]
        [NotNull]
        public string VppHome { get; set; }


        
        [Column("name")]
        [NotNull]
        public string NamePattern { get; set; }        
        
        [Column("part")]
        [NotNull]
        public string PartPattern { get; set; }
    }
}
