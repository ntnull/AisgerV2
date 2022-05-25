using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Aisger.Models.Entity;
using Aisger.Validation;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Aisger.Models
{
    [MetadataType(typeof (SEC_UserMetaData))]
    public partial class SEC_User : IObject
    {
        [Required(ErrorMessageResourceType = typeof (ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "ConfirmPwd", ResourceType = typeof (ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public String ConfirmPwd { get; set; }

        public bool IsConfirm { get; set; }

        [Display(Name = "ApplicationName", ResourceType = typeof (ResourceSetting))]
        public string ApplicationName
        {
            get
            {
                if (!IsGuest || TypeApplicationId == 5 || string.IsNullOrEmpty(JuridicalName))
                {
                    return FullName;
                }
                return JuridicalName;
            }
        }

        [Display(Name = "fioSertificate", ResourceType = typeof (ResourceSetting))]
        public string FullName
        {
            get { return LastName + " " + FirstName + " " + SecondName; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var fio = value.Split(' ');
                    if (fio.Length > 1)
                    {
                        LastName = fio[0];
                    }
                    if (fio.Length > 2)
                    {
                        FirstName = fio[1];
                    }
                    if (fio.Length == 3)
                    {
                        SecondName = fio[2];
                    }
                }
            }
        }

        public string TypeNames
        {
            get { return string.Join(",", SEC_UserKind.Select(e => e.DIC_KindUser.NameRu)); }
        }

        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }

        public string ContactInfo
        {
            get
            {
                var contactInfo = "эл. адрес:" + Email + ", моб. тел:" + Mobile + ", раб. тел:" + WorkPhone;

                if (!string.IsNullOrEmpty(InternalPhone))
                {
                    contactInfo = contactInfo + " (вн. номер:" + InternalPhone + ")";
                }
                return contactInfo;
            }
        }

        public string RoleName
        {
            get
            {
                if (SEC_Roles != null)
                {
                    return SEC_Roles.NameRu;
                }
                return null;
            }
        }
        public string DepartmentName
        {
            get
            {
                if (DIC_Department != null)
                {
                    return DIC_Department.NameRu;
                }
                return null;
            }
        }

        [Display(Name = "KatoLocation", ResourceType = typeof (ResourceSetting))]
        public string JuridicalKato { get; set; }

        [Display(Name = "KatoLocation", ResourceType = typeof (ResourceSetting))]
        public string FactKato { get; set; }

        private DateTime? _planedAuditDateTime;

        [XmlIgnore]
        [IgnoreDataMember]
        public DateTime PlanedAuditDatetime
        {
            get
            {
                if (_planedAuditDateTime == null)
                {
                    if (this.EAUDIT_Preamble != null && this.EAUDIT_Preamble.Any())
                        _planedAuditDateTime = this.EAUDIT_Preamble.Max(p => p.CreateDate).AddYears(4);
                    else
                        _planedAuditDateTime = this.CreateDate.AddYears(5);
                }
                return _planedAuditDateTime.Value;
            }
            set { _planedAuditDateTime = value; }
        }

		public bool IsChecked
		{
			get { return false; }
			//get { return this.EAUDIT_AuditorReestr != null; }
		}
    }

    public class SEC_UserMetaData
      {
//          [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
          [Display(Name = "login", ResourceType = typeof(ResourceSetting))]
          [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
          public string Login { get; set; }

          [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
          [Display(Name = "pwd", ResourceType = typeof(ResourceSetting))]
          [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
          public string Pwd { get; set; }

          [Display(Name = "Role", ResourceType = typeof(ResourceSetting))]
          [MyCustomValidation("RolesId", ErrorMessageResourceType = typeof(ResourceSetting),
              ErrorMessageResourceName = "ChooseValue")]
          public long? RolesId { get; set; }

           [Display(Name = "FirstName", ResourceType = typeof(ResourceSetting))]
          public string FirstName { get; set; }
           [Display(Name = "SureName", ResourceType = typeof(ResourceSetting))]
          public string LastName { get; set; }

          [Display(Name = "SecondName", ResourceType = typeof(ResourceSetting))]
          public string SecondName { get; set; }

		  [Display(Name = "WorkPhoneOwner", ResourceType = typeof(ResourceSetting))]
          public string WorkPhone{ get; set; }

          [Display(Name = "InternalPhone", ResourceType = typeof(ResourceSetting))]
          public string InternalPhone
          { get; set; }

          [Display(Name = "Mobile", ResourceType = typeof(ResourceSetting))]
          public string Mobile
          { get; set; }

          [Display(Name = "Organisation", ResourceType = typeof(ResourceSetting))]
//          [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
          public long? OrganizationId { get; set; }

          [Display(Name = "DIC_Department", ResourceType = typeof(ResourceSetting))]
//          [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
          public long? DeparmentId { get; set; }

          [Display(Name = "urlSite", ResourceType = typeof(ResourceSetting))]
          public string urlSite { get; set; }

          [Display(Name = "Post", ResourceType = typeof(ResourceSetting))]
          public string Post { get; set; }
      }
}