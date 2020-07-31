using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class CcdInfo
    {   
        [Column("ccdId")]     
        [Unique]
        public int CcdId { get; set; }   
     
        [Column("name")]
        public string Name { get; set; }

        [Column("nameEn")]
        public string NameEn { get; set; }

        [Column("nameCn")]
        public string NameCn { get; set; }
    }
}
