using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models
{
    public partial class EAUDIT_IndustryForm1 :ICloneable
    {
        //public long RefDicUnit { get; set; }

        public List<DIC_Unit> DIC_Units { get; set; }

        public bool IsCommand { get; set; }

        public int InnerOrder { get; set; }

        public int RowSpan { get; set; }


        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}