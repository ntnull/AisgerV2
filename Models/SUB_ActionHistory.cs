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
    
    public partial class SUB_ActionHistory
    {
        public long Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> UserId { get; set; }
        public string Note { get; set; }
        public Nullable<long> StatusId { get; set; }
        public Nullable<long> ActionId { get; set; }
        public Nullable<System.DateTime> RegDate { get; set; }
        public string XmlSign { get; set; }
        public Nullable<bool> IsSign { get; set; }
    
        public virtual SEC_User SEC_User { get; set; }
        public virtual SUB_ActionPlan SUB_ActionPlan { get; set; }
        public virtual SUB_DIC_Status SUB_DIC_Status { get; set; }
    }
}
