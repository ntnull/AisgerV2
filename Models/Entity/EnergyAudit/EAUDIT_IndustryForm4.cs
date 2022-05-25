using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models
{
    public partial class EAUDIT_IndustryForm4
    {
        public int RowSpan { get; set; }
        public bool IsCommand { get; set; }
        public int InnerOrder { get; set; }
        

        // for saving
        public long RefShop { get; set; }
        public int? Quantity { get; set; }
        public double? Power { get; set; }
    }
}