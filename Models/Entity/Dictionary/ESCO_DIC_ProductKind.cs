using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    [MetadataType(typeof(EscoDicProductKindMetaData))]
    public partial class ESCO_DIC_ProductKind : IObject
    {
        public int CountProduct
        {
            get { return ESCO_DIC_Product.Count; }

        }
        public List<string> Wastes { get; set; }
        public MultiSelectList WastList { get; set; }
    }

    public class EscoDicProductKindMetaData
    {
      [Display(Name = "NameGroup", ResourceType = typeof(ResourceSetting))]
      public string NameGroup { get; set; }
    }

}