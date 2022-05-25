using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models
{
    public class EauditAttachment1
    {
        public EauditAttachment1()
        {
            IndustryForm1BaseYear = DateTime.Now.Year - 1;
            IndustryForm1CurrentYear = DateTime.Now.Year;
        }

        public bool IsReadOnly { get; set; }

        public EAUDIT_Preamble Preamble { get; set; }

        public EAUDIT_Preamble SignedEauditPreamble { get; set; }

        public long? RefOwnedFacilities { get; set; }

        private List<EAUDIT_OwnedFacility> _ownedFacilities;
        public List<EAUDIT_OwnedFacility> OwnedFacilities
        {
            get
            {
                if (Preamble != null && _ownedFacilities == null)
                {
                    _ownedFacilities = Preamble.EAUDIT_OwnedFacility.ToList();
                }

                return _ownedFacilities;
            }
            set { _ownedFacilities = value; }
        }
        public List<SelectListItem> OwnedFacilityList
        {
            get
            {
                var ownedFacilityList = new List<SelectListItem>();
                if (OwnedFacilities != null && OwnedFacilities.Any())
                    ownedFacilityList = OwnedFacilities.Select(ow => new SelectListItem()
                    {
                        Text = ow.Name,
                        Value = ow.Id.ToString(),
                        Selected = false
                    }).ToList();
                if (Preamble != null)
                {
                    ownedFacilityList.Add(new SelectListItem()
                    {
                        Text = Preamble.EauditObjectName,
                        Value = "null",
                        Selected = true
                    });
                    if (!RefOwnedFacilities.HasValue)
                        RefOwnedFacilities = null;
                }
                

                return ownedFacilityList;
            }
        }

        public List<SelectListItem> OwnedFacilityOnlyList
        {
            get
            {
                var ownedFacilityList = new List<SelectListItem>();
                if (OwnedFacilities != null && OwnedFacilities.Any())
                {
                    ownedFacilityList = OwnedFacilities.Select(ow => new SelectListItem()
                    {
                        Text = ow.Name,
                        Value = ow.Id.ToString(),
                        Selected = false
                    }).ToList();
                    var oFacility = OwnedFacilities.FirstOrDefault();
                    if (!RefOwnedFacilities.HasValue && oFacility != null)
                        RefOwnedFacilities = oFacility.Id;
                }
                


                return ownedFacilityList;
            }
        }
        
        public int IndustryForm1BaseYear { get; set; }
        public int IndustryForm1CurrentYear { get; set; }
        public List<EAUDIT_IndustryForm1> IndustryForm1Rows { get; set; }
        public List<EAUDIT_IndustryForm2> IndustryForm2Rows { get; set; }
        public List<EAUDIT_IndustryForm3> IndustryForm3Rows { get; set; }
        
        public long? RefShop { get; set; }

        private List<EAUDIT_IndustryForm4_Shop> _industryForm4ShopRows;

        public List<EAUDIT_IndustryForm4_Shop> IndustryForm4ShopRows
        {
            get
            {
                if (Preamble != null && _industryForm4ShopRows == null)
                {
                    _industryForm4ShopRows = Preamble.EAUDIT_IndustryForm4_Shop.ToList();
                }

                return _industryForm4ShopRows;
            }
            set
            {
                _industryForm4ShopRows = value;
            }
        }

        public List<SelectListItem> IndustryForm4ShopList
        {
            get
            {
                var shopList = new List<SelectListItem>();
                if (IndustryForm4ShopRows != null && IndustryForm4ShopRows.Any())
                {
                    shopList = IndustryForm4ShopRows.Select(sh => new SelectListItem()
                    {
                        Text = string.IsNullOrEmpty(sh.ShortName) ?  sh.Name : sh.ShortName,
                        Value = sh.Id.ToString(),
                        Selected = false
                    }).ToList();
                    var form4Shop = IndustryForm4ShopRows.FirstOrDefault();
                    if (!RefShop.HasValue && form4Shop != null)
                        RefShop = form4Shop.Id;
                }
                return shopList;
            }
        }

        public List<EAUDIT_IndustryForm4_ShopValues> IndustryForm4ShopValuesRows { get; set; }
        public List<EAUDIT_IndustryForm4> IndustryForm4Rows { get; set; }
        
        public List<EAUDIT_IndustryForm5> IndustryForm5Rows { get; set; }
        public List<EAUDIT_IndustryForm6> IndustryForm6Rows { get; set; }
        public List<EAUDIT_IndustryForm7> IndustryForm7Rows { get; set; }
        public List<EAUDIT_IndustryForm8> IndustryForm8Rows { get; set; }
        public List<EAUDIT_IndustryForm9> IndustryForm9Rows { get; set; }
        public List<EAUDIT_IndustryForm10> IndustryForm10Rows { get; set; }
        public List<EAUDIT_IndustryForm11> IndustryForm11Rows { get; set; }
        public List<EAUDIT_IndustryForm12> IndustryForm12Rows { get; set; }
        public List<EAUDIT_IndustryForm13> IndustryForm13Rows { get; set; }
        public List<EAUDIT_IndustryForm14> IndustryForm14Rows { get; set; }
        public List<EAUDIT_IndustryForm15> IndustryForm15Rows { get; set; }
        public List<EAUDIT_IndustryForm16> IndustryForm16Rows { get; set; }
        public List<EAUDIT_IndustryForm17> IndustryForm17Rows { get; set; }
        public List<EAUDIT_IndustryForm18> IndustryForm18Rows { get; set; }
        public List<EAUDIT_IndustryForm19> IndustryForm19Rows { get; set; }

        public List<SelectListItem> DicUnitList { get; set; }

        public List<EAUDIT_FieldComments> FieldComments { get; set; }
    }
}