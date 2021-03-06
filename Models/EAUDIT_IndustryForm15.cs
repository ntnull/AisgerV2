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
    
    public partial class EAUDIT_IndustryForm15
    {
        public long Id { get; set; }
        public string VehicleNameTypeYear { get; set; }
        public Nullable<int> VehicleQuantity { get; set; }
        public string CapacityAndSeating { get; set; }
        public string FuelType { get; set; }
        public string SpecificFuelConsumtionPassport { get; set; }
        public Nullable<double> CurrentYearMileage { get; set; }
        public Nullable<double> CurrentYearVolumeOfCargo { get; set; }
        public Nullable<double> FuelConsumedAmount { get; set; }
        public string FuelConsumtionMesure { get; set; }
        public Nullable<double> FuelConsumtionSpecific { get; set; }
        public Nullable<double> FuelAmount { get; set; }
        public Nullable<double> FuelLosses { get; set; }
        public string Note { get; set; }
        public long refPreamble { get; set; }
        public Nullable<long> refOwnedFacility { get; set; }
        public Nullable<System.DateTime> createdate { get; set; }
        public Nullable<System.DateTime> editdate { get; set; }
        public Nullable<long> authorid { get; set; }
        public string authorlogin { get; set; }
        public bool isdeleted { get; set; }
    
        public virtual EAUDIT_OwnedFacility EAUDIT_OwnedFacility { get; set; }
        public virtual EAUDIT_Preamble EAUDIT_Preamble { get; set; }
    }
}
