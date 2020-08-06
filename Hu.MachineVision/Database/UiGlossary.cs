using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class UiGlossary
    {

        [Column("name")]
        [Unique]
        
        public string Name { get; set; }

        [Column("textEn")]
        public string TextEn { get; set; }

        [Column("textCn")]
        public string TextCn { get; set; }
    }
}
