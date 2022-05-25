using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Aisger.Models.Entity;
using Aisger.Utils;

namespace Aisger.Models
{
   [MetadataType(typeof(RstReestrMetaData))]
    public partial class RST_Reestr : IObject
    {
        public bool EditCollection { get; set; }
        public RST_ReestrHistory LastHistory { get; set; }

       public string OblastName
       {
           get {
               if (RST_Application != null && RST_Application.DIC_Kato != null)
               {
                   return RST_Application.DIC_Kato.NameRu;
               }
               return null;
           }
       }
    
       public int? ReportYear
       {
           get
           {
               if (RST_Application != null )
               {
                   return RST_Application.ReportYear;
               }
               return null;
           }
       }
       [Display(Name = "RegDate", ResourceType = typeof(ResourceSetting))]
       public string EditDateStr
       {
           get { return DateHelper.GetDate(EditDate); }
           set
           {
               var dateTemp = DateHelper.GetDate(value);
               if (dateTemp != null)
               {
                   EditDate = dateTemp.Value;
               }
           }
       }

       public List<RST_ReestrHistory> RstReestrHistories { get; set; }

       public IList<string> AttachFiles { get; set; }

       public StatusReestr StatusReestr { get; set; }

       public string TemplOwnerName { get; set; }

	   //----проверить в бд (sec_user)
	   public int IsExistSecUser { get; set; }
    }

    public class CheckReestr
    {
        public StatusReestr StatusReestr { get; set; }
        public string OwnerName { get; set; }
        public string Address { get; set; }
    }

    public class RstReestrMetaData
    {
        [Display(Name = "KadastrNumber", ResourceType = typeof(ResourceSetting))]
//        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string KadastNumber { get; set; }

        [Display(Name = "ObjectName", ResourceType = typeof(ResourceSetting))]
//        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
     
        public string ObjectName { get; set; }

        [Display(Name = "Address", ResourceType = typeof(ResourceSetting))]
//        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
    
        public string Address { get; set; }

        [Display(Name = "biniinOwner", ResourceType = typeof(ResourceSetting))]
//        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
        public string BINIIN { get; set; }


        [Display(Name = "OwnerName", ResourceType = typeof(ResourceSetting))]
        [Required(ErrorMessageResourceType = typeof(ResourceSetting), ErrorMessageResourceName = "NotEmpty")]
    
        public string OwnerName { get; set; }

	
    }

    public partial class RST_ReestrHistory : IEntity
    {
        public IList<string> AttachFiles { get; set; }
    }

	public enum StatusReestr
	{
		EMPTY_REESTR = 0,
		NEW_REESTR = 1,
		INCLULDE_REESTR = 2,
		EXCLUDE_REESTR = 3,
		DUPLICATE_IIN = 4,				  //----реестрде бар
		DUPLICATE_IIN_EXIST_REESTR = 5,   //---- реестрде жок
		NOT_BINIIN=6					  //---- бин или иин жок
	}
}