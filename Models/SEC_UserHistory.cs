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
    
    public partial class SEC_UserHistory
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Pwd { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SecondName { get; set; }
        public string JuridicalName { get; set; }
        public string Post { get; set; }
        public string BINIIN { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string WorkPhone { get; set; }
        public string InternalPhone { get; set; }
        public bool IsGuest { get; set; }
        public bool IsDisabled { get; set; }
        public Nullable<long> OrganizationId { get; set; }
        public Nullable<long> DeparmentId { get; set; }
        public Nullable<long> RolesId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool IsDeleted { get; set; }
        public string Address { get; set; }
        public bool IsCvazy { get; set; }
        public string ResponceFIO { get; set; }
        public string ResponcePost { get; set; }
        public Nullable<long> Oblast { get; set; }
        public Nullable<long> Region { get; set; }
        public Nullable<long> SubRegion { get; set; }
        public Nullable<long> Village { get; set; }
        public long TypeApplicationId { get; set; }
        public bool IsHaveGES { get; set; }
        public string IDK { get; set; }
        public Nullable<long> OkedId { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string Certificate { get; set; }
        public string BankRequisites { get; set; }
        public string FactAddress { get; set; }
        public string urlSite { get; set; }
        public Nullable<long> FactOblast { get; set; }
        public Nullable<long> FactRegion { get; set; }
        public Nullable<long> FactSubRegion { get; set; }
        public Nullable<long> FactVillage { get; set; }
        public Nullable<int> FSCode { get; set; }
        public string Note { get; set; }
        public Nullable<long> UserId { get; set; }
        public Nullable<long> Author { get; set; }
    
        public virtual SEC_User SEC_User { get; set; }
    }
}
