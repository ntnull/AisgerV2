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
    
    public partial class SEC_UserKind
    {
        public long Id { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<long> KindId { get; set; }
        public Nullable<bool> IsBlocked { get; set; }
        public Nullable<System.DateTime> DateEdit { get; set; }
        public string ReasonBlocked { get; set; }
    
        public virtual DIC_KindUser DIC_KindUser { get; set; }
        public virtual SEC_User SEC_User { get; set; }
    }
}
