//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aisger.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RST_Report
    {
        public RST_Report()
        {
            this.RST_ReportReestr = new HashSet<RST_ReportReestr>();
        }
    
        public long Id { get; set; }
        public Nullable<int> ReportYear { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<bool> isactive { get; set; }
    
        public virtual ICollection<RST_ReportReestr> RST_ReportReestr { get; set; }
        public virtual SEC_User SEC_User { get; set; }
    }
}
