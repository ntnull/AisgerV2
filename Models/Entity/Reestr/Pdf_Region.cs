using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Reestr
{
    public class Pdf_Region
    {
        public long? obl_id { get; set; }
        public string obl_nameru { get; set; }
        public int? cnt_all { get; set; }
        public int? cnt_from { get; set; }
        public int? cnt_pdf { get; set; }
        public DateTime? gen_date { get; set; }
        public string obl_kato { get; set; }

        public bool start { get; set; } = false;
        public bool end { get; set; } = false;
    }
}