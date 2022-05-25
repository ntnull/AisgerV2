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
    
    public partial class SUB_ActionPlan
    {
        public SUB_ActionPlan()
        {
            this.SUB_ActionComment = new HashSet<SUB_ActionComment>();
            this.SUB_ActionHistory = new HashSet<SUB_ActionHistory>();
            this.SUB_ActionTab1 = new HashSet<SUB_ActionTab1>();
            this.SUB_ActionTab2 = new HashSet<SUB_ActionTab2>();
            this.SUB_ActionTab3 = new HashSet<SUB_ActionTab3>();
            this.SUB_ActionRecordHistory = new HashSet<SUB_ActionRecordHistory>();
        }
    
        public long Id { get; set; }
        public int ReportYear { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<long> Editor { get; set; }
        public string Note { get; set; }
        public Nullable<System.DateTime> SendDate { get; set; }
        public Nullable<long> StatusId { get; set; }
        public Nullable<System.DateTime> DesignDate { get; set; }
        public string DesignNote { get; set; }
        public bool IsPlan { get; set; }
        public Nullable<int> BeginPlanYear { get; set; }
        public Nullable<int> EndPlanYear { get; set; }
        public bool IsBack { get; set; }
        public bool IsConfirmPlan { get; set; }
    
        public virtual SEC_User SEC_User { get; set; }
        public virtual SEC_User SEC_User1 { get; set; }
        public virtual ICollection<SUB_ActionComment> SUB_ActionComment { get; set; }
        public virtual ICollection<SUB_ActionHistory> SUB_ActionHistory { get; set; }
        public virtual SUB_DIC_Status SUB_DIC_Status { get; set; }
        public virtual ICollection<SUB_ActionTab1> SUB_ActionTab1 { get; set; }
        public virtual ICollection<SUB_ActionTab2> SUB_ActionTab2 { get; set; }
        public virtual ICollection<SUB_ActionTab3> SUB_ActionTab3 { get; set; }
        public virtual ICollection<SUB_ActionRecordHistory> SUB_ActionRecordHistory { get; set; }
    }
}