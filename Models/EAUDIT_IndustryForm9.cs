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
    
    public partial class EAUDIT_IndustryForm9
    {
        public long Id { get; set; }
        public string BoilerType { get; set; }
        public Nullable<int> YearOfCommissioning { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<double> Performance { get; set; }
        public Nullable<double> Pressure { get; set; }
        public Nullable<double> GrossEfficiencyLastTest { get; set; }
        public Nullable<double> EfficiencyPassport { get; set; }
        public Nullable<double> SpecificFuelConsumption { get; set; }
        public Nullable<double> AnnualFuelConsumption { get; set; }
        public Nullable<double> AnnualHeatProduction { get; set; }
        public string Note { get; set; }
        public long refPreamble { get; set; }
        public Nullable<long> refOwnedFacility { get; set; }
        public string PerformanceStr { get; set; }
        public string PressureStr { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> editdate { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
        public bool isdeleted { get; set; }
    
        public virtual EAUDIT_OwnedFacility EAUDIT_OwnedFacility { get; set; }
        public virtual EAUDIT_Preamble EAUDIT_Preamble { get; set; }
    }
}