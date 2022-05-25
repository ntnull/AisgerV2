using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{
    [MetadataType(typeof(MapProjectMetaData))]
    public partial class MAP_Project : IObject
    {

        [Display(Name = "ProjectName", ResourceType = typeof(ResourceSetting))]
        public string ProjectName
        {
            get
            {
                if (MAP_Application != null)
                {
                    return MAP_Application.ProjectName;
                }
                return null;
            }
        }

        [Display(Name = "TotalCost", ResourceType = typeof(ResourceSetting))]
        public double? TotalCost
        {
            get
            {
                if (MAP_Application != null)
                {
                    return MAP_Application.TotalCost;
                }
                return null;
            }
        }

         [Display(Name = "EscoName", ResourceType = typeof(ResourceSetting))]
        public string EscoName
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
        [Display(Name = "RecipientName", ResourceType = typeof(ResourceSetting))]
        public string RecipientName
        {
            get
            {
                if (MAP_Application != null)
                {
                    return MAP_Application.SEC_User1.ApplicationName;
                }
                return null;
            }
        }

        [Display(Name = "RegDate", ResourceType = typeof(ResourceSetting))]
        public string RegDateStr
        {
            get { return DateHelper.GetDate(RegDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    RegDate = dateTemp.Value;
                }
            }
        }

        [Display(Name = "BeginDate", ResourceType = typeof(ResourceSetting))]
        public string BeginDateStr
        {
            get { return DateHelper.GetDate(BeginDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    BeginDate = dateTemp.Value;
                }
            }
        }
        [Display(Name = "EndDate", ResourceType = typeof(ResourceSetting))]
        public string EndDateStr
        {
            get { return DateHelper.GetDate(EndDate); }
            set
            {
                var dateTemp = DateHelper.GetDate(value);
                if (dateTemp != null)
                {
                    EndDate = dateTemp.Value;
                }
            }
        }
    }
    public class MapProjectMetaData
    {
          [Display(Name = "SourceBudject", ResourceType = typeof(ResourceSetting))]
        public string SourceBudject { get; set; }

         [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
          public Nullable<long> ApplicationId { get; set; }

         [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
          public Nullable<long> EscoId { get; set; }
    }
}