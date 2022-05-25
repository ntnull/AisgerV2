using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Validation;

namespace Aisger.Models.Entity.Security
{
    public class SEC_Guest 
    {
       
        public long Id { get; set; }

        [Display(Name = "BININ", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string BINIIN
        { get; set; }

        [Display(Name = "WorkPhoneOwner", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public string WorkPhone
        { get; set; }
        [Display(Name = "InternalPhone", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public string InternalPhone
        { get; set; }

		 [Display(Name = "MobilePhoneLeader", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string Mobile
        { get; set; }
        
        [Display(Name = "Email", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string Email
        { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResourceSetting))]
        public string Address
        { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResourceSetting))]
        public string FactAddress
        { get; set; }

        [Display(Name = "Certificate", ResourceType = typeof(ResourceSetting))]
        public string Certificate
        { get; set; }

        [Display(Name = "ReasonBlocked", ResourceType = typeof(ResourceSetting))]
        public string ReasonBlocked
        { get; set; }

        [Display(Name = "BossFirstName", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string FirstName
        { get; set; }


        [Display(Name = "BossLastName", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string LastName
        { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "pwd", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public string Pwd
        { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "ConfirmPwd", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public string ConfirmPwd
        { get; set; }

        [Display(Name = "BossSecondName", ResourceType = typeof(ResourceSetting))]
        public string SecondName
        { get; set; }

        [Display(Name = "Post", ResourceType = typeof(ResourceSetting))]
        public string Post
        { get; set; }

        [Display(Name = "Organisation", ResourceType = typeof(ResourceSetting))]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string JuridicalName
        { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "ChooseValue")]
        public long TypeApplicationId { get; set; }

         [Display(Name = "ResponceFIO", ResourceType = typeof(ResourceSetting))]
         [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string ResponceFIO
        { get; set; }

         [Display(Name = "ResponcePost", ResourceType = typeof(ResourceSetting))]
         [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string ResponcePost
        { get; set; }
        public bool IsError { get; set; }
         [Display(Name = "IsCvazy", ResourceType = typeof(ResourceSetting))]
        public bool IsCvazy { get; set; }
        public List<string> Wastes { get; set; }
        public MultiSelectList WastList { get; set; }

        public List<string> Kinds { get; set; }
        public MultiSelectList KindList { get; set; }
        public string ErrorMessage { get; set; }

//       [Display(Name = "oblast", ResourceType = typeof(ResourceSetting))]
        public long Oblast { get; set; }

        [Display(Name = "Region", ResourceType = typeof(ResourceSetting))]
        public long Region { get; set; }

        [Display(Name = "SubRegion", ResourceType = typeof(ResourceSetting))]
        public long? SubRegion { get; set; }

         [Display(Name = "Village", ResourceType = typeof(ResourceSetting))]
        public long? Village { get; set; }

         [Display(Name = "IsHaveGES", ResourceType = typeof(ResourceSetting))]
         public bool IsHaveGES  { get; set; }

         [Display(Name = "OkedMain", ResourceType = typeof(ResourceSetting))]
         [MyCustomValidation("OkedId", ErrorMessageResourceType = typeof(ResourceSetting),
             ErrorMessageResourceName = "ChooseValue")]
         public long? OkedId { get; set; }

        [Display(Name = "IDK", ResourceType = typeof(ResourceSetting))]
         public string IDK { get; set; }

        [Display(Name = "urlSite", ResourceType = typeof(ResourceSetting))]
        public string urlSite
        { get; set; }

        [Display(Name = "KindUser", ResourceType = typeof(ResourceSetting))]
        public long? KindId { get; set; }

         [Display(Name = "IsBlokced", ResourceType = typeof(ResourceSetting))]
         public bool IsBlokced { get; set; }

         [Display(Name = "KatoLocation", ResourceType = typeof(ResourceSetting))]
         [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
         public string JuridicalKato { get; set; }

         [Display(Name = "KatoLocation", ResourceType = typeof(ResourceSetting))]      
         public string FactKato { get; set; }

		 [Display(Name = "GerWoEcp", ResourceType = typeof(ResourceSetting))]
		 public bool ger_wo_ecp { get; set; }

        //[Display(Name = "FSCode", ResourceType = typeof(ResourceSetting))]
         public int? FSCode { get; set; }

        public Nullable<long> FactOblast { get; set; }
         public Nullable<long> FactRegion { get; set; }
         public Nullable<long> FactSubRegion { get; set; }
         public Nullable<long> FactVillage { get; set; }
         public string PreviousUrl { get; set; }
		 public Nullable<int> ReportYear { get; set; }
    }
}