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
    
    public partial class SUB_FormTab1
    {
        public long Id { get; set; }
        public Nullable<long> FormId { get; set; }
        public Nullable<long> KindId { get; set; }
        public string Code { get; set; }
        public string Events { get; set; }
        public string Year1 { get; set; }
        public string Year2 { get; set; }
        public string Year3 { get; set; }
        public string Year4 { get; set; }
        public string Year5 { get; set; }
        public Nullable<float> Expend1 { get; set; }
        public Nullable<float> Expend2 { get; set; }
        public Nullable<float> Expend3 { get; set; }
        public Nullable<float> Expend4 { get; set; }
        public Nullable<float> Expend5 { get; set; }
        public string Note { get; set; }
    
        public virtual SUB_DIC_KindTabOne SUB_DIC_KindTabOne { get; set; }
        public virtual SUB_Form SUB_Form { get; set; }
    }
}