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
    
    public partial class SUB_DIC_KindTabTwo
    {
        public SUB_DIC_KindTabTwo()
        {
            this.SUB_ActionTab2 = new HashSet<SUB_ActionTab2>();
            this.SUB_FormTab2 = new HashSet<SUB_FormTab2>();
        }
    
        public long Id { get; set; }
        public string Code { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public string IndexCode { get; set; }
    
        public virtual ICollection<SUB_ActionTab2> SUB_ActionTab2 { get; set; }
        public virtual ICollection<SUB_FormTab2> SUB_FormTab2 { get; set; }
    }
}