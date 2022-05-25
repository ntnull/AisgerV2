using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Aisger.Helpers;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    [MetadataType(typeof(EAUDIT_Preamble.EAUDIT_PreambleMetaData))]
    [Serializable]
    [XmlRoot()]
    public partial class EAUDIT_Preamble : IObject //, IXmlSerializable
    {
        [OnSerializing]
        internal void OnSerializedMethod(StreamingContext context)
        {
            // Setting this as parent property for Child object
            foreach (var e in EAUDIT_Building)
            {
                e.EAUDIT_BuildingForm1 = null;
                e.EAUDIT_BuildingForm2 = null;
                e.EAUDIT_BuildingForm3 = null;
                e.EAUDIT_BuildingForm4 = null;
                e.EAUDIT_BuildingForm5 = null;
                e.EAUDIT_BuildingForm6 = null;
                e.EAUDIT_BuildingForm7 = null;
                e.EAUDIT_BuildingForm9 = null;
                e.EAUDIT_Preamble = null;
            }
            foreach (var e in EAUDIT_BuildingForm1)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_BuildingForm2)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_BuildingForm3)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_BuildingForm4)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_BuildingForm5)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_BuildingForm6)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_BuildingForm7)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_BuildingForm9)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_Building = null;
            }
            foreach (var e in EAUDIT_IndustryBuildingForm1)
            {
                e.EAUDIT_Preamble = null;
            }

            foreach (var e in EAUDIT_OwnedFacility)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_IndustryForm1 = null;
                e.EAUDIT_IndustryForm2 = null;
                e.EAUDIT_IndustryForm3 = null;
                e.EAUDIT_IndustryForm4 = null;
                e.EAUDIT_IndustryForm4_Shop = null;
                e.EAUDIT_IndustryForm5 = null;
                e.EAUDIT_IndustryForm6 = null;
                e.EAUDIT_IndustryForm7 = null;
                e.EAUDIT_IndustryForm8 = null;
                e.EAUDIT_IndustryForm9 = null;
                e.EAUDIT_IndustryForm10 = null;
                e.EAUDIT_IndustryForm11 = null;
                e.EAUDIT_IndustryForm12 = null;
                e.EAUDIT_IndustryForm13 = null;
                e.EAUDIT_IndustryForm14 = null;
                e.EAUDIT_IndustryForm15 = null;
                e.EAUDIT_IndustryForm16 = null;
                e.EAUDIT_IndustryForm17 = null;
                e.EAUDIT_IndustryForm18 = null;
                e.EAUDIT_IndustryForm19 = null;
            }

            foreach (var e in EAUDIT_IndustryForm1)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm2)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm3)
            {
                e.EAUDIT_Preamble = null;
                //e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm4)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm5)
            {
                e.EAUDIT_Preamble = null;
                //e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm6)
            {
                e.EAUDIT_Preamble = null;
                //e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm7)
            {
                e.EAUDIT_Preamble = null;
                // e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm8)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm9)
            {
                e.EAUDIT_Preamble = null;
                //e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm10)
            {
                e.EAUDIT_Preamble = null;
                //e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm11)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm12)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm13)
            {
                e.EAUDIT_Preamble = null;
                //e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm14)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm15)
            {
                e.EAUDIT_Preamble = null;
                // e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm16)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm17)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm18)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm19)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_IndustryForm4_Shop)
            {
                e.EAUDIT_Preamble = null;
                e.EAUDIT_OwnedFacility = null;
            }
            foreach (var e in EAUDIT_FormHistory)
            {
                e.EAUDIT_Preamble = null;
            }
        }
        
//        public XmlSchema GetSchema()
//        {
//            throw new NotImplementedException();
//        }
//
//        public void ReadXml(XmlReader reader)
//        {
//            throw new NotImplementedException();
//        }
//
//        public void WriteXml(XmlWriter writer)
//        {
//            writer.WriteE
//        }

//        [Display(Name = "sContract", ResourceType = typeof(ResourceSetting))]
//        public string EauditObjectName
//        {
//            get
//            {
//                if (this.SEC_User == null)
//                {
//                    return null;
//                }
//                return this.SEC_User.JuridicalName;
//            }
//        }

        [XmlIgnore]
        [IgnoreDataMember]
        public bool IsReadOnly { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public EAUDIT_Preamble SignedEauditPreamble { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public string EauditObjectBINIINName
        {
            get
            {
                if (this.SEC_User == null)
                {
                    return null;
                }
                return string.Format("{0} - {1}", this.SEC_User.BINIIN ,this.SEC_User.JuridicalName);
            }
        }
        
//        [Display(Name = "sAuditor", ResourceType = typeof(ResourceSetting))]
//        public string AuditorName
//        {
//            get
//            {
//                if (this.SEC_User1 == null)
//                {
//                    return null;
//                }
//                return this.SEC_User1.JuridicalName;
//            }
//        }

        [XmlIgnore]
        [IgnoreDataMember]
        public string AuditorBINIINName
        {
            get
            {
                if (this.SEC_User1 == null)
                {
                    return null;
                }
                return string.Format("{0} - {1}", this.SEC_User1.BINIIN, this.SEC_User1.JuridicalName);
            }
        }

        [XmlIgnore]
        [IgnoreDataMember]
        public bool IsCurrentUserAuditor { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public List<SelectListItem> DicStatusList { get; set; }

        /// <summary>
        /// Список операций
        /// </summary>
        [XmlIgnore]
        [IgnoreDataMember]
        public List<SelectListItem> OperatorList { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        [Display(Name = "Status", ResourceType = typeof(ResourceSetting))]
        public string Status
        {
            get
            {
                var status = string.Empty;
                if (this.EAUDIT_DIC_Statuses != null)
                {
                    if (CultureHelper.GetCurrentCulture() == CultureHelper.FieldKz)
                        status = this.EAUDIT_DIC_Statuses.NameKz;
                    else
                    status = this.EAUDIT_DIC_Statuses.NameRu;
                }
                    
                return status;
            }
        }

        public class EAUDIT_PreambleMetaData
        {
            [Display(Name = "sEauditObject", ResourceType = typeof(ResourceSetting))]
            public long refEauditObject { get; set; }

            [Display(Name = "sAuditor", ResourceType = typeof(ResourceSetting))]
            public long refAuditor { get; set; }

            [Display(Name = "sContractDate", ResourceType = typeof(ResourceSetting))]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
            public Nullable<System.DateTime> ContractDate { get; set; }

            [Display(Name = "sContract", ResourceType = typeof(ResourceSetting))]
            public string ContractNumber { get; set; }

            [Display(Name = "sFormOfIncorporation", ResourceType = typeof(ResourceSetting))]
            public string EauditObjectsFormOfIncorporation { get; set; }

            [Display(Name = "Address", ResourceType = typeof (ResourceSetting))]
            public string EauditObjectAddress { get; set; }

            [Display(Name = "sBankRequisites", ResourceType = typeof (ResourceSetting))]
            public string EauditObjectBankrequisites { get; set; }

            [Display(Name = "sHeadOf", ResourceType = typeof (ResourceSetting))]
            public string EauditObjectHead { get; set; }

            [Display(Name = "sFormOfIncorporation", ResourceType = typeof(ResourceSetting))]
            public string AuditorFormOfIncorporation { get; set; }

            [Display(Name = "Address", ResourceType = typeof(ResourceSetting))]
            public string AuditorAddress { get; set; }

            [Display(Name = "sBankRequisites", ResourceType = typeof(ResourceSetting))]
            public string AuditorBankrequisites { get; set; }

            [Display(Name = "sHeadOf", ResourceType = typeof(ResourceSetting))]
            public string AuditorHead { get; set; }

            [Display(Name = "sFinishDate", ResourceType = typeof(ResourceSetting))]
            [DataType(DataType.DateTime)]
            [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
            public Nullable<System.DateTime> FinishDate { get; set; }

            [Display(Name = "sBaseYear", ResourceType = typeof(ResourceSetting))]
            public Nullable<int> BaseYear { get; set; }

            [Display(Name = "sReportYear", ResourceType = typeof(ResourceSetting))]
            public int ReportYear { get; set; }

            [Display(Name = "sContract", ResourceType = typeof(ResourceSetting))]
            public string EauditObjectName { get; set; }

            [Display(Name = "sAuditor", ResourceType = typeof(ResourceSetting))]
            public string AuditorName { get; set; }

            [Display(Name = "Status", ResourceType = typeof(ResourceSetting))]
            public string refDicStatus { get; set; }

            [XmlIgnore()]
            [IgnoreDataMember]
            public ICollection<EAUDIT_AttachedFiles> EAUDIT_AttachedFiles { get; set; }
        }
    }
}