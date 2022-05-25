using System;
using System.ComponentModel.DataAnnotations;
using Aisger.Models.Entity;

namespace Aisger.Models
{
    public interface IBaseDictionary : IObject
    {

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameRu", ResourceType = typeof(ResourceSetting))]
        string NameRu { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameKz", ResourceType = typeof(ResourceSetting))]
        string NameKz { get; set; }
    }

    public interface IEntityDictionary : IEntity
    {
         string Code { get; set; }
         string NameRu { get; set; }
         string NameKz { get; set; }
         System.DateTime CreateDate { get; set; }
         Nullable<System.DateTime> EditDate { get; set; }
         bool IsDeleted { get; set; }
    }

    public class UnMappedDictionary
    {
        public UnMappedDictionary()
        {
        }

        public UnMappedDictionary(long id, string nameRu)
        {
            ID = id;
            NAME_RU = nameRu;
        }

        public UnMappedDictionary(long id, string nameRu, string code)
        {
            ID = id;
            NAME_RU = nameRu;
            CODE = code;
        }

        public long ID { get; set; }

        public string NAME_RU { get; set; }
        public string CODE { get; set; }
    }
}