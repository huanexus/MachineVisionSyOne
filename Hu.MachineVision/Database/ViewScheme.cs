using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net.Attributes;

namespace Hu.MachineVision.Database
{
    public class ViewScheme
    {
        [Column("ccdId")]
        [NotNull]
        public int CcdId { get; set; }

        [Column("brandId")]
        [NotNull]
        public int BrandId { get; set; }

        [Column("viewId")]
        [NotNull]
        public int ViewId { get; set; }

        [Column("recordId")]
        public int RecordId { get; set; }

        [Column("category")]
        public int Category { get; set; }

        [Column("textEn")]
        public string TextEn { get; set; }

        [Column("textCn")]

        public string TextCn { get; set; }
    }
}
