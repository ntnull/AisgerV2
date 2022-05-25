using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    [MetadataType(typeof(COLLECTOR_Cmdevice.COLLECTOR_CmdeviceMetaData))]
    public partial class COLLECTOR_Cmdevice : IObject
    {
        private string _indicatorType;
        public string IndicatorType
        {
            get
            {
                if (this.COLLECTOR_DIC_CmdeviceTypes != null)
                    _indicatorType = this.COLLECTOR_DIC_CmdeviceTypes.NameRu;

                return _indicatorType;
            }
            set
            {
                _indicatorType = value;
            }
        }

        public List<SelectListItem> IndicatorTypeList { get; set; }
        public List<SelectListItem> DicUnitList { get; set; }

        public class COLLECTOR_CmdeviceMetaData
        {
            [Display(Name = "Code", ResourceType = typeof(ResourceSetting))]
            public string Code { get; set; }

            [Display(Name = "NameRu", ResourceType = typeof(ResourceSetting))]
            public string NameRu { get; set; }

            [Display(Name = "NameKz", ResourceType = typeof(ResourceSetting))]
            public string NameKz { get; set; }

            [Display(Name = "IndicatorType", ResourceType = typeof(ResourceSetting))]
            public Nullable<long> refIndicatorType { get; set; }
            public System.DateTime CreateDate { get; set; }
            public Nullable<System.DateTime> EditDate { get; set; }
            public bool IsDeleted { get; set; }

            [Display(Name = "Description", ResourceType = typeof(ResourceSetting))]
            public string Description { get; set; }

            [Display(Name = "DicUnit", ResourceType = typeof(ResourceSetting))]
            public Nullable<long> refUnit { get; set; }

            [Display(Name = "Address", ResourceType = typeof(ResourceSetting))]
            public string Address { get; set; }
        }
    }

}