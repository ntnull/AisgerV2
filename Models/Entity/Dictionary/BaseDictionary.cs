using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using Aisger.Helpers;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{
    public class BaseDictionary
    {
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameRu", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public global::System.String NameRu
        { get; set; }

        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        [Display(Name = "NameKz", ResourceType = typeof(ResourceSetting))]
        [DisplayFormat(ConvertEmptyStringToNull = false, NullDisplayText = "")]
        public global::System.String NameKz
        { get; set; }

    }
    public class BaseDictionaryWhithUnit : BaseDictionary
    {
     /*  [Required(ErrorMessageResourceType = typeof(ResourceSetting),
            ErrorMessageResourceName = "ChooseValue")]*/
        public long? UnitId { get; set; }

    }
    public class SimlpeDictionary
    {
        public SimlpeDictionary(string id, string nameRu)
        {
            Id = id;
            NameRu = nameRu;
        }

        public string Id { get; set; }
        public string NameRu { get; set; }
    }

    public class SimlpeLongDictionary
    {
        public SimlpeLongDictionary(long id, string nameRu)
        {
            Id = id;
            NameRu = nameRu;
        }

        public long Id { get; set; }
        public string NameRu { get; set; }
    }
    [MetadataType(typeof(BaseDictionary))]
    public partial class RST_DIC_Status : IEntityDictionary
    {
        public string Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                    return NameRu;
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                    return NameKz;
                return string.Empty;
            }
        }
    }
    [MetadataType(typeof(BaseDictionary))]
    public partial class SEC_DIC_EventType : IEntityDictionary
    {
    }

    [MetadataType(typeof(BaseDictionary))]
    public partial class DIC_OKED : IEntityDictionary
    {
        public string FullName
        {
            get { return "[" + Code + "] - "+ NameRu; }
        }
        public string ParentName { get; set; }
        public bool IsCodeIncorect   { get; set; }

        public long IdVal { get; set; }
    }

    public abstract class ABaseDictionary 
    {

      

    }

    [MetadataType(typeof(BaseDictionary))]
    public partial class RST_DIC_Reason : IBaseDictionary
    {
        public string Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                    return NameRu;
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                    return NameKz;
                return string.Empty;
            }
        }
    }
     [MetadataType(typeof(BaseDictionary))]
    public partial class DIC_Unit : IBaseDictionary
     {
         public string Name
         {
             get
             {
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                     return NameRu;
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                     return NameKz;
                 return string.Empty;
             }
         }
     }
     [MetadataType(typeof(BaseDictionary))]
     public partial class DIC_TypeApplication : IEntityDictionary
     {
         public string Name
         {
             get
             {
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                     return NameRu;
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                     return NameKz;
                 return string.Empty;
             }
         }
     }
     [MetadataType(typeof(BaseDictionary))]
     public partial class SUB_DIC_TypeCounter : IBaseDictionary
     {
         public string Name
         {
             get
             {
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                     return NameRu;
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                     return NameKz;
                 return string.Empty;
             }
         }
     }
   
    

    [MetadataType(typeof(SubDicTypeResourceMetaData))]
    public partial class SUB_DIC_TypeResource : IBaseDictionary
    {
        public string Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                    return NameRu;
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                    return NameKz;
                return string.Empty;
            }
        }
        public string UnitName
        {
            get
            {
                return DIC_Unit != null ? DIC_Unit.Name : null;
            }
        }
    }
    public class SubDicTypeResourceMetaData : BaseDictionary
    {
        /*  [Required(ErrorMessageResourceType = typeof(ResourceSetting),
               ErrorMessageResourceName = "ChooseValue")]*/
        public long? UnitId { get; set; }

         [Display(Name = "KeofTut", ResourceType = typeof(ResourceSetting))]
        public Nullable<float> Keof { get; set; }

    }
   
    [MetadataType(typeof(BaseDictionaryWhithUnit))]
    public partial class SUB_DIC_KindResource : IBaseDictionary
    {
        public string UnitName
        {
            get
            {
                return DIC_Unit != null ? DIC_Unit.NameRu : null;
            }
        }
        public string Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                    return NameRu;
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                    return NameKz;
                return string.Empty;
            }
        }
    }

     [MetadataType(typeof(BaseDictionary))]
    public partial class SUB_DIC_Status : IEntityDictionary
    {
    }

     [MetadataType(typeof(BaseDictionary))]
     public partial class SUB_DIC_KindTabOne : IEntityDictionary
     {
           public string Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                    return NameRu;
                if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                    return NameKz;
                return string.Empty;
            }
        }
     }
     [MetadataType(typeof(BaseDictionary))]
     public partial class SUB_DIC_KindTabTwo : IEntityDictionary
     {
         public string Name
         {
             get
             {
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Ru)
                     return NameRu;
                 if (Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName == CultureHelper.Kk)
                     return NameKz;
                 return string.Empty;
             }
         }
     }
     [MetadataType(typeof(BaseDictionary))]
     public partial class DIC_KindUser : IEntityDictionary
     {
     }
     public partial class SEC_UserKind : IEntity
     {
     }
     [MetadataType(typeof(BaseDictionary))]
     public partial class MAP_DIC_Status : IEntityDictionary
     {
     }

     public partial class EAUDIT_DIC_TypeResource : IEntityDictionary
     {
     }

     public partial class EAUDIT_DIC_Statuses : IEntityDictionary
     {
     }
}