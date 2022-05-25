#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace Aisger.Models
{
    /// <summary>
    ///     Шаблон для справочников
    /// </summary>
    public class DictionaryMetaData
    {
        [Required(ErrorMessageResourceType = typeof (ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameRu", ResourceType = typeof (ResourceSetting))]
        public string NameRu { get; set; }

    }
}