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
    
    public partial class RST_ReestrReportHistory
    {
        public long Id { get; set; }
        public System.DateTime RegDate { get; set; }
        public Nullable<long> UserId { get; set; }
        public string Note { get; set; }
        public Nullable<long> StatusId { get; set; }
        public Nullable<long> ReestrId { get; set; }
        public Nullable<long> ReasonId { get; set; }
        public Nullable<int> ReportYear { get; set; }
        public Nullable<long> Oblast { get; set; }
        public Nullable<long> Expectant { get; set; }
        public Nullable<long> Author { get; set; }
        public Nullable<long> ApplicationId { get; set; }
    
        public virtual DIC_Kato DIC_Kato { get; set; }
        public virtual RST_Application RST_Application { get; set; }
        public virtual RST_DIC_Reason RST_DIC_Reason { get; set; }
        public virtual RST_DIC_Reason RST_DIC_Reason1 { get; set; }
        public virtual RST_DIC_StatusHistory RST_DIC_StatusHistory { get; set; }
        public virtual RST_ReportReestr RST_ReportReestr { get; set; }
        public virtual SEC_User SEC_User { get; set; }
        public virtual SEC_User SEC_User1 { get; set; }
        public virtual RST_DIC_Status RST_DIC_Status { get; set; }

    }
}
