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
    
    public partial class SUB_Form5Record
    {
        public long Id { get; set; }
        public Nullable<long> FormId { get; set; }
        public string IndicatorName { get; set; }
        public string RegularStandart { get; set; }
        public string UnitMeasure { get; set; }
        public string CalcFormula { get; set; }
        public Nullable<float> EnergyValue { get; set; }
        public string Note { get; set; }
        public Nullable<long> NormEnergyId { get; set; }
        public Nullable<int> KindIndex { get; set; }
        public Nullable<long> energyindicator_id { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> editdate { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
        public bool isdeleted { get; set; }
        public Nullable<short> TypeOfHeating { get; set; }
    
        public virtual sub_dic_energyindicator sub_dic_energyindicator { get; set; }
        public virtual SUB_DIC_NormEnergy SUB_DIC_NormEnergy { get; set; }
        public virtual SUB_Form SUB_Form { get; set; }
    }
}