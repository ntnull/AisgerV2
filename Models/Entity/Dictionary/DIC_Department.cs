using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;

namespace Aisger.Models
{
     [MetadataType(typeof(DicDepartmentMetaData))]
    public partial class DIC_Department : IObject
    {
    }
    public class DicDepartmentMetaData
    {
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameRu", ResourceType = typeof(ResourceSetting))]
        public string NameRu { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameKz", ResourceType = typeof(ResourceSetting))]
        public string NameKz { get; set; }

        [Display(Name = "ShortNameRu", ResourceType = typeof(ResourceSetting))]
        public string ShortNameRu { get; set; }

        [Display(Name = "ShortNameKz", ResourceType = typeof(ResourceSetting))]
        public string ShortNameKz { get; set; }

    }
}