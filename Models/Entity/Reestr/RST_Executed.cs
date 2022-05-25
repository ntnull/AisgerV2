using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{
    [MetadataType(typeof (RstExecutedMetaData))]

    public partial class RST_Executed : IObject
    {
        public string OwnerName
        {
            get
            {
                if (SEC_User == null)
                {
                    return null;
                }
                return SEC_User.FullName;
            }
        }

        [Display(Name = "DateOrder", ResourceType = typeof (ResourceSetting))]
        public string DateOrderStr
        {
            get { return DateHelper.GetDate(DateOrder); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    DateOrder = dateTemp.Value;
                }
            }
        }
    }

    public class RstExecutedMetaData
    {
        [Display(Name = "NumberOrder", ResourceType = typeof (ResourceSetting))]
        public string NumberOrder { get; set; }

    }
    public class RST_ExecutedFilter
    {
        public int? ReportYear { get; set; }

        [Display(Name = "NumberOrder", ResourceType = typeof(ResourceSetting))]
        public string NumberOrder { get; set; }

        [Display(Name = "DateOrder", ResourceType = typeof(ResourceSetting))]
        public string DateOrderStr
        {
            get { return DateHelper.GetDate(DateOrder); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    DateOrder = dateTemp.Value;
                }
            }
        }
        public List<string> Statuses { get; set; }
        public MultiSelectList StatusList { get; set; }

        public List<string> Expacts { get; set; }
        public MultiSelectList ExpactList { get; set; }
        public DateTime? DateOrder { get; set; }
        public string Note { get; set; }
        public long? UserId { get; set; }

        public string BINIIN { get; set; }
        public string IDK { get; set; }
        public string SubjectName { get; set; }
        public string Adress { get; set; }
        public long SortId { get; set; }

        public List<string> Oblasts { get; set; }
        public MultiSelectList OblastList { get; set; }

        public IEnumerable<RST_ReportReestrFilter> RstReportReestrs { get; set; }
    }
}