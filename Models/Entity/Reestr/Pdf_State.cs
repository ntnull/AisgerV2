using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Reestr
{
    public class Pdf_State
    {
        public long oblast_id { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public bool finished { get; set; } = false;
    }
}