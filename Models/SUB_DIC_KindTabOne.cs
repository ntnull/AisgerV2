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
    
    public partial class SUB_DIC_KindTabOne
    {
        public SUB_DIC_KindTabOne()
        {
            this.SUB_ActionTab1 = new HashSet<SUB_ActionTab1>();
            this.SUB_FormTab1 = new HashSet<SUB_FormTab1>();
        }
    
        public long Id { get; set; }
        public string Code { get; set; }
        public string NameRu { get; set; }
        public string NameKz { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public string IndexCode { get; set; }
    
        public virtual ICollection<SUB_ActionTab1> SUB_ActionTab1 { get; set; }
        public virtual ICollection<SUB_FormTab1> SUB_FormTab1 { get; set; }
    }
}
