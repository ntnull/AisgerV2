using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models
{
    public class EauditAttachment2
    {
        public EauditAttachment2()
        {
            
        }
        public EAUDIT_Preamble Preamble { get; set; }

        public EAUDIT_Preamble SignedEauditPreamble { get; set; }

        public long? RefBuilding { get; set; }

        private List<EAUDIT_Building> _buildings;
        public List<EAUDIT_Building> Buildings
        {
            get
            {
                if (Preamble != null && _buildings == null)
                {
                    _buildings = Preamble.EAUDIT_Building.ToList();
                }

                return _buildings;
            }
            set { _buildings = value; }
        }

        public List<SelectListItem> BuildingList
        {
            get
            {
                var buildingList = new List<SelectListItem>();
                if (Buildings != null && Buildings.Any())
                    buildingList = Buildings.Select(b => new SelectListItem()
                    {
                        Text = b.Name,
                        Value = b.Id.ToString(),
                        Selected = false
                    }).ToList();
                /*
                if (Preamble != null)
                {
                    buildingList.Add(new SelectListItem()
                    {
                        Text = Preamble.EauditObjectName,
                        Value = "null",
                        Selected = true
                    });
                    if (!RefBuilding.HasValue)
                        RefBuilding = null;
                }
                */
                return buildingList;
            }
        }
        
        public List<EAUDIT_BuildingForm1> BuildingForm1Rows { get; set; }
        public List<EAUDIT_BuildingForm2> BuildingForm2Rows { get; set; }
        public List<EAUDIT_BuildingForm3> BuildingForm3Rows { get; set; }
        public List<EAUDIT_BuildingForm4> BuildingForm4Rows { get; set; }
        public List<EAUDIT_BuildingForm5> BuildingForm5Rows { get; set; }
        public List<EAUDIT_BuildingForm6> BuildingForm6Rows { get; set; }
        public List<EAUDIT_BuildingForm7> BuildingForm7Rows { get; set; }
//        public List<EAUDIT_BuildingForm8> BuildingForm8Rows { get; set; }
        public List<EAUDIT_BuildingForm9> BuildingForm9Rows { get; set; }
        

        public List<SelectListItem> DicUnitList { get; set; }
        public List<EAUDIT_FieldComments> FieldComments { get; set; }

        public bool IsReadOnly { get; set; }
    }
}