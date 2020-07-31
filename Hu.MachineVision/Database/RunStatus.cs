using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
   public class RunStatus
    {
        [Column("name")]
        [NotNull]
        [Unique]
        public string Name { get; set; }

        [Column("data")]
        [NotNull]
        public int Data { get; set; }    
    }
}
