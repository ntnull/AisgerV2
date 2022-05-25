using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Xml.Serialization;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{
    [MetadataType(typeof(SubActionPlanMetaData))]
    [Serializable]
    [XmlRoot()]
    public partial class SUB_ActionPlan : IObject
    {
        [OnSerializing]
        internal void OnSerializedMethod(StreamingContext context)
        {
            SUB_ActionComment = null;
            SUB_ActionHistory = null;
            SUB_ActionRecordHistory = null;
            foreach (var e in SUB_ActionTab1)
            {
                e.SUB_ActionPlan = null;
            }
            foreach (var e in SUB_ActionTab2)
            {
                e.SUB_ActionPlan = null;
            }
            foreach (var e in SUB_ActionTab3)
            {
                e.SUB_ActionPlan = null;
            }
          
        }
        public string SubjectName
        {
            get
            {
                if (SEC_User1 != null)
                {
                    return SEC_User1.ApplicationName;
                }
                return null;
            }
        }

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
        public List<SUB_DIC_KindTabOne> SubDicKindTabOnes { get; set; }

        public List<SUB_ActionTab1> SubFormTab1s { get; set; }
        public List<SUB_ActionTab2> SubFormTab2s { get; set; }
        public List<SUB_ActionTab3> SubFormTab3s { get; set; }
    }
    public partial class SUB_ActionHistory : IObject
    {
        public IList<string> AttachFiles { get; set; }
    }
    public class SubActionPlanMetaData
    {
        [Display(Name = "ReportPeriod", ResourceType = typeof(ResourceSetting))]
        public int ReportYear { get; set; }
    }

    public class SubActionCommonFilter
    {
        public int? ReportYear { get; set; }
        public string BINIIN { get; set; }
        public string IDK { get; set; }
        public string SubjectName { get; set; }
        public string Adress { get; set; }

        public List<string> Oblasts { get; set; }
        public MultiSelectList OblastList { get; set; }

        public List<string> Statuses { get; set; }
        public MultiSelectList StatusList { get; set; }
        public long? UserId { get; set; }

        public long? SendId { get; set; }
        public long? ExcludedId { get; set; }
        public IEnumerable<SUB_ActionPlanFilter> SubActionPlanFilters { get; set; }
    }
    public class SUB_ActionPlanFilter 
    {
        public long Id { get; set; }
        public string BINIIN { get; set; }
        public string IDK { get; set; }
        public string OblastName { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
        public string StatusName { get; set; }
        public DateTime? SendDate { get; set; }
        public long? UserId { get; set; }
        public long? StatusId { get; set; }
        public int StatusIndex { get; set; }
        public bool IsBack { get; set; }

    }
}