using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Entity.Map
{
	
	public class MAP_ApplicationEE2Info
    {
        public long Id { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }

        public long SecUserId { get; set; }

        [Display(Name = "EE2DetailsName", ResourceType = typeof(ResourceSetting))]
        public int DicDetailsId { get; set; }

        public string DicDetails_Name { get; set; }

        [Display(Name = "EE2DetailsParameter", ResourceType = typeof(ResourceSetting))]
        public int DicDetailModelsId { get; set; }

        public string DicDetailModels_Name { get; set; }

        [Display(Name = "CountOfFixtures", ResourceType = typeof(ResourceSetting))]
        public Nullable<double> CountOfFixtures { get; set; }

        [Display(Name = "CountOfLamps", ResourceType = typeof(ResourceSetting))]
        public Nullable<double> CountOfLamps { get; set; }
        
        [Display(Name = "Power", ResourceType = typeof(ResourceSetting))]
        public Nullable<double> Power { get; set; }

        [Display(Name = "CPRA", ResourceType = typeof(ResourceSetting))]
        public string CPRA { get; set; }

        [Display(Name = "AggregatePower", ResourceType = typeof(ResourceSetting))]
        public Nullable<double> AggregatePower { get; set; }

        [Display(Name = "AverageTariff", ResourceType = typeof(ResourceSetting))]
        public Nullable<double> AverageTariff { get; set; }

        [Display(Name = "WorkingHours", ResourceType = typeof(ResourceSetting))]
        public string WorkingHours { get; set; }

        [Display(Name = "MaintenanceCosts", ResourceType = typeof(ResourceSetting))]
        public Nullable<double> MaintenanceCosts { get; set; }

        public bool IsDeleted { get; set; }
        
        [Display(Name = "MaintenanceCosts", ResourceType = typeof(ResourceSetting))]
        public string files { get; set; }

        [Display(Name = "sComment", ResourceType = typeof(ResourceSetting))]
        public string Comments { get; set; }

        public List<SelectListItem> DicDetailsList { get; set; }
        public List<SelectListItem> DicDetailModelsList { get; set; }

        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }


    }

   
}