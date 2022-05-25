using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    [MetadataType(typeof(SEC_RolesMetaData))]
    public partial class SEC_Roles : IObject
    {
        public List<RigthSave> SaveRigthtIds { get; set; }
    }
    public class RigthSave
    {
        public long Id { get; set; }
        public bool Edit { get; set; }
    }
    public class SEC_RolesMetaData
    {
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "Name", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public global::System.String NameRu
        { get; set; }
    }
}