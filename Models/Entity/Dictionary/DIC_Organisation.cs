using System;
using System.ComponentModel.DataAnnotations;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    [MetadataType(typeof(DicOrganizatoinMetaData))]
    public partial class DIC_Organization : IObject
    {
    }

    public class DicOrganizatoinMetaData
    {
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameRu", ResourceType = typeof(ResourceSetting))]
        public string NameRu { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameKz", ResourceType = typeof(ResourceSetting))]
        public string NameKz { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "Address", ResourceType = typeof(ResourceSetting))]
        public string Address { get; set; }

        [Display(Name = "ShortNameRu", ResourceType = typeof(ResourceSetting))]
        public string ShortNameRu { get; set; }

         [Display(Name = "ShortNameKz", ResourceType = typeof(ResourceSetting))]
        public string ShortNameKz { get; set; }

        [Display(Name = "Oblast", ResourceType = typeof(ResourceSetting))]
        public long Oblast { get; set; }

        [Display(Name = "RegionCity", ResourceType = typeof(ResourceSetting))]
        public Nullable<long> Region { get; set; }
        [Display(Name = "SubRegion", ResourceType = typeof(ResourceSetting))]
        public Nullable<long> SubRegion { get; set; }

        [Display(Name = "Village", ResourceType = typeof(ResourceSetting))]
        public Nullable<long> Village { get; set; }
    }
}