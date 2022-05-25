using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aisger.Models.Entity.Subject
{
    public class AddressEntity
    {
        public long? Oblast { get; set; }

        [Display(Name = "Region", ResourceType = typeof(ResourceSetting))]
        public long? Region { get; set; }

        [Display(Name = "SubRegion", ResourceType = typeof(ResourceSetting))]
        public long? SubRegion { get; set; }

        [Display(Name = "Village", ResourceType = typeof(ResourceSetting))]
        public long? Village { get; set; }
    }
}