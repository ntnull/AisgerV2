using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Aisger.Models.Entity;
using Aisger.Utils;
using Aisger.Models.Repository;

namespace Aisger.Models
{
    [MetadataType(typeof(SubFormMetaData))]
    [Serializable]
    [XmlRoot()]
    public partial class SUB_Form : IObject
    {
        [OnSerializing]
        internal void OnSerializedMethod(StreamingContext context)
        {

            this.SUB_FormComment = null;
            this.SUB_FormHistory = null;
            this.SUB_FormTab1 = null;
            this.SUB_FormTab2 = null;
            this.SUB_FormTab3 = null;
            this.SUB_FormRecordHistory = null;
            this.RST_ReportReestr = null;
            this.SEC_User1.SUB_Form = null;
            this.SEC_User1.DIC_Department = null;
            this.SEC_User1.DIC_Kato = null;
            this.SEC_User1.DIC_Kato1 = null;
            this.SEC_User1.DIC_Kato2 = null;
            this.SEC_User1.DIC_Kato3 = null;
            this.SEC_User1.DIC_OKED = null;
//             this.SEC_User1.DIC_OKED.DIC_OKED1 = null;
//             this.SEC_User1.DIC_OKED.DIC_OKED2 = null;
//             this.SEC_User1.DIC_OKED.ESCO_DIC_ProductKindOked = null;
//             this.SEC_User1.DIC_OKED.SEC_User = null;
//             this.SEC_User1.DIC_OKED.SEC_UserOked = null;
            this.SEC_User1.DIC_Organization = null;
            this.SEC_User1.DIC_TypeApplication = null;
            this.SEC_User1.ESCO_DIC_ProductKind = null;
            this.SEC_User1.MAP_Application = null;
            this.SEC_User1.MAP_Application1 = null;
            this.SEC_User1.MAP_ApplicationHistory = null;
            this.SEC_User1.MAP_Project = null;
            this.SEC_User1.MAP_Project1 = null;
            this.SEC_User1.RST_Application = null;
            this.SEC_User1.RST_Executed = null;
            this.SEC_User1.RST_ReestrHistory = null;
            this.SEC_User1.RST_ReestrReportHistory = null;
            this.SEC_User1.RST_ReestrReportHistory1 = null;
            this.SEC_User1.RST_Report = null;
            this.SEC_User1.RST_ReportReestr = null;
            this.SEC_User1.RST_ReportReestr1 = null;
            this.SEC_User1.SEC_JurEvent = null;
            this.SEC_User1.SEC_Roles = null;
            this.SEC_User1.SEC_UserKind = null;
            this.SEC_User1.SEC_UserOked = null;
            this.SEC_User1.SUB_ActionComRecord = null;
            this.SEC_User1.SUB_ActionHistory = null;
            this.SEC_User1.SUB_ActionPlan = null;
            this.SEC_User1.SUB_ActionPlan1 = null;
            this.SEC_User1.SUB_ActionRecordHistory = null;
            this.SEC_User1.SUB_DIC_Event = null;
            this.SEC_User1.SUB_FormComRecord = null;
            this.SEC_User1.SUB_Form1 = null;
            this.SEC_User1.SUB_FormHistory = null;
            this.SEC_User1.SUB_FormRecordHistory = null;
            this.SEC_User1.Pwd = null;

            foreach (var e in SUB_Form2Record)
            {
                e.SUB_Form = null;
            }
            foreach (var e in SUB_FormKadastr)
            {
                e.SUB_Form = null;
            }
            foreach (var e in SUB_Form3Record)
            {
                e.SUB_Form = null;
            }
            foreach (var e in SUB_Form4Record)
            {
                e.SUB_Form = null;
            }
            foreach (var e in SUB_Form5Record)
            {
                e.SUB_Form = null;
            }
            foreach (var e in SUB_Form6Record)
            {
                e.SUB_Form = null;
            }
            foreach(var e in SUB_Form2Gu)
            {
                e.SUB_Form = null;
            }
            foreach (var e in SUB_Form3Gu)
            {
                e.SUB_Form = null;
            }
        }

        public SUB_ActionPlan SubActionPlan { get; set; }

        public string SubjectName
        {
            get
            {
                if (RST_ReportReestr != null)
                {
                    return RST_ReportReestr.FirstOrDefault().usrjuridicalname;
                }
                return null;
            }
        }

        //public string SubjectName
        //{
        //    get
        //    {
        //        if (SEC_User1 != null)
        //        {
        //            return SEC_User1.ApplicationName;
        //        }
        //        return null;
        //    }
        //}

        public string SubjectOblast
        {
            get
            {
                if (SEC_User1 != null && SEC_User1.DIC_Kato != null)
                {
                    return SEC_User1.DIC_Kato.NameRu;
                }
                return null;
            }
        }

        public string ActiveTab { get; set; }
        public string SubjectBin
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.BINIIN;
                }
                return null;
            }
        }

        public string SubjectIDK
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.IDK;
                }
                return null;
            }
        }

        public string SubjectAddress
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.Address;
                }
                return null;
            }
        }

        public string SubjectBoss
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.FullName;
                }
                return null;
            }
        }

        public string SubjectPost
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.Post;
                }
                return null;
            }
        }

        public string IsCvaziStr
        {
            get
            {
                if (SEC_User1 != null)
                {
                    if (SEC_User1.IsCvazy)
                    {
                        return "Да";
                    }
                    return "Нет";
                }
                return null;
            }
        }

        public bool IsCvazy
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.IsCvazy;

                }
                return false;
            }
        }

        public string StatusName
        {
            get
            {
                if (SUB_DIC_Status != null)
                {
                    return SUB_DIC_Status.NameRu;
                }
                return null;
            }
        }
        public string CodeStatus
        {
            get
            {
                if (SUB_DIC_Status != null)
                {
                    return SUB_DIC_Status.Code;
                }
                return null;
            }
        }

        public string _subjectOkeds;

        public string SubjectOkeds
        {
            get
            {
                if (_subjectOkeds == null && this.SEC_User1 != null && this.SEC_User1.SEC_UserOked != null)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (var secUserOked in this.SEC_User1.SEC_UserOked)
                    {
                        builder.Append(secUserOked.DIC_OKED.NameRu).Append(",");
                    }
                    _subjectOkeds = builder.ToString();
                }
                return _subjectOkeds;
            }
            set { _subjectOkeds = value; }
        }

        public string _subjectOked;

        public string SubjectMainOked
        {
            get
            {
                if (_subjectOked == null && this.SEC_User1 != null && this.SEC_User1.DIC_OKED != null)
                {
                    _subjectOked = this.SEC_User1.DIC_OKED.FullName;
                }
                return _subjectOked;
            }
            set { _subjectOked = value; }
        }


        [Display(Name = "RegDate", ResourceType = typeof(ResourceSetting))]
        public string DesignDateStr
        {
            get { return DateHelper.GetDate(DesignDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    DesignDate = dateTemp.Value;
                }
            }
        }

        public IList<string> AttachFiles { get; set; }
        public List<SUB_Form2Record> SubForm2Records { get; set; }
        public List<SUB_Form3Record> SubForm3Records { get; set; }
        public List<SUB_Form4Record> SubForm4Records { get; set; }
        public List<SUB_Form4Record> SubForm4RecordsOther { get; set; }
        public List<SUB_Form5Record> SubForm5Records { get; set; }
        public List<SUB_Form5Record> SubForm5RecordsOther { get; set; }
        public List<SUB_Form6Record> SubForm6Records { get; set; }
        public List<SUB_FormTab1> SubFormTab1s { get; set; }
        public List<SUB_FormTab2> SubFormTab2s { get; set; }
        public List<SUB_FormTab3> SubFormTab3s { get; set; }
        public List<SUB_FormKadastr> SubFormKadastrs { get; set; }
        public List<SUB_DIC_NormEnergy> SubDicNormEnergies { get; set; }
        public List<SUB_DIC_KindTabOne> SubDicKindTabOnes { get; set; }
        public List<string> Wastes { get; set; }
        public MultiSelectList WastList { get; set; }
        public List<SUB_DIC_TypeResource> SubDicTypeResources { get; set; }
        public List<SubFornMessage> SubFornMessages { get; set; }
        public List<sub_dic_energyindicator> SubDicEnergyindicatorList { get; set; }

        public string PreviousUrl { get; set; }
        public SUB_Form SignedSubForm { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public bool IsSigned {
            get { return this.SUB_FormHistory.Any(fh => fh.IsSign != null && fh.IsSign.Value); } }
    }

    public class SubFormMetaData
    {
        [Display(Name = "ReportPeriod", ResourceType = typeof(ResourceSetting))]
        public int ReportYear { get; set; }
    }

    public partial class SUB_FormHistory : IObject
    {
        public IList<string> AttachFiles { get; set; }
    }

    public class SubUpdateField
    {
        public long ModelId { get; set; }
        public long RecordId { get; set; }
        public string Unique { get; set; }
        public bool IsNew { get; set; }

    }

    public class SubFornMessage
    {
        public int TypeMessage { get; set; }
        public int FormIndex { get; set; }
        public string Message { get; set; }
    }
    public class SUB_FormFilter
    {
        public int? ReportYear { get; set; }

        public long? SendId { get; set; }
        public string BINIIN { get; set; }
        public string JuridicalName { get; set; }

        public List<string> Oblasts { get; set; }
        public MultiSelectList OblastList { get; set; }

        public List<string> Statuses { get; set; }
        public MultiSelectList StatusList { get; set; }
        public long? UserId { get; set; }
        public string IDK { get; set; }
        public IEnumerable<SUB_FormRecord> SubFormRecords { get; set; }
    }
    public class SUB_FormRecord : SUB_Form
    {
        public string IDK{ get; set; }
        public string BINIIN { get; set; }
        public string StatusName { get; set; }
        public string OblastName { get; set; }
        public string JuridicalName { get; set; }
    }

}

