using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;

namespace Aisger.Models
{
      [MetadataType(typeof(RstApplicationMetaData))]
    public partial class RST_Application : IObject
    {
        public IList<string> AttachFiles { get; set; }

        public List<RST_Reestr> RstReestrs { get; set; }

        public bool IsExistYear { get; set; }

          public string OblastName
          {
              get
              {
                  if (DIC_Kato == null)
                  {
                      return null;
                  }
                  return DIC_Kato.NameRu;
              }
          }
          public string ApplicationName
          {
              get
              {
                  if (SEC_User == null)
                  {
                      return null;
                  }
                  return SEC_User.ApplicationName;
              }
          }
          
    }

    public class RstApplicationMetaData
    {
         [Display(Name = "ReportYear", ResourceType = typeof(ResourceSetting))]
//         [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public int ReportYear { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
         public Nullable<long> Oblast { get; set; }
    }
}