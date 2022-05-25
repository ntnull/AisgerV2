using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Constants;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.EnergyAudit;
using OfficeOpenXml.FormulaParsing.Utilities;

namespace Aisger.Controllers.Audit
{
    public class IndustryAttachment1Controller : Controller
    {
        public ActionResult GetOwnedFacilityView(long preambleId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;


            List<EAUDIT_OwnedFacility> ownedFacilities = null;

            if (preamble != null && preamble.EAUDIT_OwnedFacility.Count > 0)
                ownedFacilities = preamble.EAUDIT_OwnedFacility.ToList();
            else
                ownedFacilities = new List<EAUDIT_OwnedFacility>()
                {
                    new EAUDIT_OwnedFacility()
                };

            attachment1.OwnedFacilities = ownedFacilities;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.OwnedFacilities)
            {
                foreach (var pInfo in typeof(EAUDIT_OwnedFacility).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.OwnedFacility
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }

            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);
            return PartialView("~/Views/EnergyAudit/Preamble/_OwnedFacilities.cshtml", attachment1);
        }

        public ActionResult GetIndustryShopView(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;

            EAUDIT_OwnedFacility ownedFacility = null;
            if (ofacility == null)
                ownedFacility = preamble.EAUDIT_OwnedFacility.FirstOrDefault();
            else
                ownedFacility = preamble.EAUDIT_OwnedFacility.FirstOrDefault(of => of.Id == ofacility);

            attachment1.RefOwnedFacilities = ownedFacility != null ? ownedFacility.Id : ofacility;
            List<EAUDIT_IndustryForm4_Shop> shops = null;
            if (ownedFacility != null && ownedFacility.EAUDIT_IndustryForm4_Shop.Any())
                 shops = ownedFacility.EAUDIT_IndustryForm4_Shop.ToList();
            else if (preamble.EAUDIT_IndustryForm4_Shop.Any())
            {
                shops = preamble.EAUDIT_IndustryForm4_Shop.ToList();
            }
            else
            {
                shops = new List<EAUDIT_IndustryForm4_Shop>()
                {
                    new EAUDIT_IndustryForm4_Shop()
                }; 
            }

            attachment1.IndustryForm4ShopRows = shops;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form3 in attachment1.IndustryForm4ShopRows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm4_Shop).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm4
                        , form3.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);
            
            return PartialView("~/Views/EnergyAudit/IndustryForm4/_IndustryShopView.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm1View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm1> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm1.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm1.Where(form1 => form1.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm1.Where(form1 => form1.refOwnedFacility == null).ToList();
            }

            #region FORM 1 FILL
                var typeRepository = new EAuditDicTypeRepository();
                var types = typeRepository
                    .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm1, true, tr => tr.PosIndex ?? 0)
                    .ToList();
                attachment1.IndustryForm1Rows = new List<EAUDIT_IndustryForm1>();
                int innerOrder = 1;
                foreach (var type in types)
                {
                    var form1 = new EAUDIT_IndustryForm1()
                    {
                        refPreamble = preambleId,
                        refTypeResource = type.Id,
                        EAUDIT_DIC_TypeResource = type,
                        Id = 0,
                        EAUDIT_Preamble = preamble,
                        InnerOrder = innerOrder++,
                        RowSpan = 0,
                    };

                    // unit
                    if (type.DIC_Unit != null)
                    {
                        form1.DIC_Units = new List<DIC_Unit>();
                        type.DIC_Unit.PosIndex = 1;
                        form1.DIC_Units.Add(type.DIC_Unit);
                    }
                    if (type.DIC_Unit1 != null)
                    {
                        type.DIC_Unit1.PosIndex = 2;
                        form1.DIC_Units.Add(type.DIC_Unit1);

                        form1.RowSpan = 0;
                    }

                    // init row with header by data 
                    var typeId = type.Id;
                    if (industryRows != null &&
                        industryRows.Any(ir => ir.refPreamble == preambleId
                                               && ir.refTypeResource == typeId
                                               && string.IsNullOrEmpty(ir.ProductName)))
                    {
                        var indFormList = industryRows.Where(ir => ir.refPreamble == preambleId
                                                                 && ir.refTypeResource == typeId &&
                                                                 string.IsNullOrEmpty(ir.ProductName)).ToList();
                        for (int i = 0; i < indFormList.Count(); i++)
                        {
                            EAUDIT_IndustryForm1 row = null;
                            var form1New = (EAUDIT_IndustryForm1)form1.Clone();
                            if (form1.DIC_Units != null && form1.DIC_Units.Any())
                            {
                                foreach (var dUnit in form1.DIC_Units.OrderBy(du => du.PosIndex))
                                {
                                    form1New = (EAUDIT_IndustryForm1)form1.Clone();
                                    row = indFormList.FirstOrDefault(if1 =>
                                    {
                                        var unit = dUnit;
                                        return unit != null && if1.Unit == unit.NameRu;
                                    });
                                    if (row != null)
                                    {
                                        form1New.Volume = row.Volume;
                                        form1New.BaseYearVolume = row.BaseYearVolume;
//                                        form1New.BaseYear = row.BaseYear;
//                                        form1New.CurrentYear = row.CurrentYear;
                                        form1New.Note = row.Note;
                                        form1New.ProductName = row.ProductName;
                                        form1New.refDicUnit = row.refDicUnit;
                                        form1.InnerOrder = innerOrder++; //i++;
                                        form1.Unit = string.IsNullOrEmpty(row.Unit) ? dUnit.NameRu : row.Unit;
                                        form1New.Id = row.Id;
                                        attachment1.IndustryForm1Rows.Add(form1New);
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                row = indFormList[i];
                                if (row != null)
                                {
                                    form1New.Volume = row.Volume;
                                    form1New.BaseYearVolume = row.BaseYearVolume;
//                                    form1New.BaseYear = row.BaseYear;
//                                    form1New.CurrentYear = row.CurrentYear;
                                    form1New.Note = row.Note;
                                    form1New.ProductName = row.ProductName;
                                    form1New.refDicUnit = row.refDicUnit;
                                    form1.InnerOrder = innerOrder++; //i++;
                                    form1.Unit = row.Unit;
                                    form1New.Id = row.Id;
                                    attachment1.IndustryForm1Rows.Add(form1New);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (form1.DIC_Units != null && form1.DIC_Units.Count == 2)
                        {
                            var form1New = (EAUDIT_IndustryForm1)form1.Clone();
                            form1New.RowSpan = 0;
                            form1New.InnerOrder = innerOrder++;
                            attachment1.IndustryForm1Rows.Add(form1New);
                        }
                        attachment1.IndustryForm1Rows.Add(form1);    
                    }

                    // command row
                    if ((type.Code != "02-0"))
                    {
                        attachment1.IndustryForm1Rows.Add(new EAUDIT_IndustryForm1()
                        {
                            EAUDIT_DIC_TypeResource = type,
                            IsCommand = true,
                            InnerOrder = 1000
                        });
                    }
                }
                if (industryRows != null)
                {
                    foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId 
                        && !string.IsNullOrEmpty(ir.ProductName)).OrderBy(r => r.Id))
                    {
                        var form1 = new EAUDIT_IndustryForm1()
                        {
                            refPreamble = preambleId,
                            refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                            EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                            Id = source.Id,
//                            BaseYear = source.BaseYear,
//                            CurrentYear = source.CurrentYear,
                            EAUDIT_Preamble = preamble,
                            BaseYearVolume = source.BaseYearVolume,
                            Volume = source.Volume,
                            Note = source.Note,
                            ProductName = source.ProductName,
                            refDicUnit = source.refDicUnit,
                            Unit = source.Unit,
                            InnerOrder = innerOrder++
                        };
                        attachment1.IndustryForm1Rows.Add(form1);

//                        if (form1.BaseYear.HasValue)
//                            attachment1.IndustryForm1BaseYear = form1.BaseYear.Value;
//                        if (form1.CurrentYear.HasValue)
//                            attachment1.IndustryForm1CurrentYear = form1.CurrentYear.Value;
                    }
                }
                attachment1.FieldComments = new List<EAUDIT_FieldComments>();
                foreach (var form1 in attachment1.IndustryForm1Rows)
                {
                    form1.RowSpan +=
                        attachment1.IndustryForm1Rows.Count(
                            ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form1.EAUDIT_DIC_TypeResource.PosIndex);
                    foreach (var pInfo in typeof (EAUDIT_IndustryForm1).GetProperties())
                    {
                        var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm1
                            , form1.Id
                            , pInfo.Name);
                        if (fieldComment != null)
                            attachment1.FieldComments.Add(fieldComment);
                    }
                }
            #endregion

            attachment1.DicUnitList = new List<SelectListItem>();
            var dicrepository = new DicUnitRepository();
            foreach (var dUnit in dicrepository.GetQuery(du => !du.IsDeleted, false, null).ToList())
            {
                attachment1.DicUnitList.Add(new SelectListItem()
                {
                    Text = dUnit.NameRu,
                    Value = dUnit.Id.ToString()
                });
            }

            attachment1.IndustryForm1Rows = attachment1.IndustryForm1Rows.OrderBy(row => row.EAUDIT_DIC_TypeResource.PosIndex)
                .ThenBy(row => row.EAUDIT_DIC_TypeResource.Code)
                .ThenBy(row => row.InnerOrder).ToList();

            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm1/_IndustryForm1View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm2View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm2> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm2.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm2.Where(form2 => form2.refOwnedFacility == ofacility.Value).OrderBy(f2 => f2.Id).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm2.Where(form2 => form2.refOwnedFacility == null).OrderBy(f2 => f2.Id).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm2, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm2Rows = new List<EAUDIT_IndustryForm2>();

            #region Fill FORM 2
            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form2 = new EAUDIT_IndustryForm2()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };
                if (type.DIC_Unit != null)
                {
                    form2.Unit = type.DIC_Unit == null ? string.Empty : type.DIC_Unit.NameRu;
                    type.DIC_Unit.PosIndex = 1;
                }

                if (type.Code == "02-0" && false)
                {
                    //                    form2.RowSpan = industryRows != null
                    //                        ? industryRows.Count(ir => ir.EAUDIT_DIC_TypeResource.Code == "02-0") + 1
                    //                        : form2.RowSpan + 1;

                    attachment1.IndustryForm2Rows.Add(new EAUDIT_IndustryForm2()
                    {
                        EAUDIT_DIC_TypeResource = type,
                        EAUDIT_Preamble = preamble,
                        // IsAdditionalRow = true,
                        IsCommand = true,
                        InnerOrder = 1000
                    });
                }

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form2.Quantity = row.Quantity;
                        form2.DeviceType = row.DeviceType;
                        form2.DeviceQuantity = row.DeviceQuantity;
                        form2.Unit = row.Unit;
                        form2.Note = row.Note;
                        form2.Name = row.Name;
                        form2.Id = row.Id;
                        form2.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                var arrayNotExtendableRowCodes = new[] { "01-0", /*"02-0",*/ "05-0" };
                if (!arrayNotExtendableRowCodes.Contains(form2.EAUDIT_DIC_TypeResource.Code))
                {
                    attachment1.IndustryForm2Rows.Add(new EAUDIT_IndustryForm2()
                    {
                        EAUDIT_Preamble = preamble,
                        EAUDIT_DIC_TypeResource = type,
                        IsCommand = true,
                        InnerOrder = 1000
                    });
                }

                attachment1.IndustryForm2Rows.Add(form2);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form2 = new EAUDIT_IndustryForm2()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        Name = source.Name,
                        Unit = source.Unit,
                        DeviceQuantity = source.DeviceQuantity,
                        DeviceType = source.DeviceType,
                        Quantity = source.Quantity,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false
                    };
                    attachment1.IndustryForm2Rows.Add(form2);
                }
            }

            #endregion
            
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form2 in attachment1.IndustryForm2Rows)
            {
                form2.RowSpan = attachment1.IndustryForm2Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form2.EAUDIT_DIC_TypeResource.PosIndex);

                foreach (var pInfo in typeof(EAUDIT_IndustryForm2).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm2
                        , form2.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm2/_IndustryForm2View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm3View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm3> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm3.Any())
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm3.Where(f3 => f3.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm3.Where(f3 => f3.refOwnedFacility == null).ToList();
            else 
                industryRows = new List<EAUDIT_IndustryForm3>()
                {
                    new EAUDIT_IndustryForm3()
                };

            attachment1.IndustryForm3Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();

            foreach (var form3 in attachment1.IndustryForm3Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm3).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm3
                        , form3.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            attachment1.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm3/_IndustryForm3View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm4View(long preambleId, long? shopId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefShop = shopId;
            List<EAUDIT_IndustryForm4> industryRows = null;
            if (!shopId.HasValue)
            {
                var shopFirst = preamble.EAUDIT_IndustryForm4_Shop.OrderBy(sh => sh.Id).FirstOrDefault();
                if (shopFirst != null)
                {
                    attachment1.RefShop = shopId = shopFirst.Id;
                }
            }

            if (preamble != null && preamble.EAUDIT_IndustryForm4_Shop.Count > 0)
            {
                if (shopId.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm4.Where(
                            f => f.EAUDIT_IndustryForm4_ShopValues.Any(sh => sh.refShop == shopId)).OrderBy(f => f.Id).ToList();
            }
            else
            {
                return Content("Необходимо внести информацию о цехах, участках, производствах");
            }

            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm4, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm4Rows = new List<EAUDIT_IndustryForm4>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm4()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;

                        var shopValue = row.EAUDIT_IndustryForm4_ShopValues.FirstOrDefault(
                            shv => shv.refShop == attachment1.RefShop && shv.refTypeResource == typeId);
                        if (shopValue != null)
                        {
                            form.Quantity = shopValue.Quantity;
                            form.Power = shopValue.Power;
                        }
                    }
                }

//                var arrayExtendableRowCodes = new[] { "01-1", "01-2", "01-3", "01-4", "01-5", "01-6", "01-7", "01-8" };
//                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
//                {
//                    attachment1.IndustryForm4Rows.Add(new EAUDIT_IndustryForm4()
//                    {
//                        EAUDIT_Preamble = preamble,
//                        EAUDIT_DIC_TypeResource = type,
//                        IsCommand = true,
//                        InnerOrder = 1000
//                    });
//                }

                attachment1.IndustryForm4Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value))
                {
                    var form = new EAUDIT_IndustryForm4()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        Name = source.Name,
                        Quantity = source.Quantity,
                        Power = source.Power,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm4Rows.Add(form);
                }
            }

            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment1.IndustryForm4Rows)
            {
                form.RowSpan = attachment1.IndustryForm4Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);

                foreach (var pInfo in typeof(EAUDIT_IndustryForm3).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm4
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm4/_IndustryForm4View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm5View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm5> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm5.Count > 0)
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm5.Where(f => f.refOwnedFacility == ofacility.Value).OrderBy(f => f.Id).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm5.Where(f => f.refOwnedFacility == null).OrderBy(f => f.Id).ToList();
            else
                industryRows = new List<EAUDIT_IndustryForm5>()
                {
                    new EAUDIT_IndustryForm5()
                };

            attachment1.IndustryForm5Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm5Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm5).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm5
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm5/_IndustryForm5View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm6View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;
            List<EAUDIT_IndustryForm6> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm6.Count > 0)
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm6.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm6.Where(f => f.refOwnedFacility == null).ToList();
            else
                industryRows = new List<EAUDIT_IndustryForm6>()
                {
                    new EAUDIT_IndustryForm6()
                };

            attachment1.IndustryForm6Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();

            foreach (var form in attachment1.IndustryForm6Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm6).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm6
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm6/_IndustryForm6View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm7View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;
            
            List<EAUDIT_IndustryForm7> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm7.Count > 0)
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm7.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm7.Where(f => f.refOwnedFacility == null).ToList();
            else
                industryRows = new List<EAUDIT_IndustryForm7>()
                {
                    new EAUDIT_IndustryForm7()
                };

            attachment1.IndustryForm7Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm7Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm7).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm7
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm7/_IndustryForm7View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm8View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm8> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm8.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm8.Where(f8 => f8.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm8.Where(f8 => f8.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm8, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm8Rows = new List<EAUDIT_IndustryForm8>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form8 = new EAUDIT_IndustryForm8()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form8.Id = row.Id;
                        form8.TotalConsumption = row.TotalConsumption;
                        form8.RegulatoryConsumptionValue = row.RegulatoryConsumptionValue;
                        form8.RegulatoryConsumptionPercent = row.RegulatoryConsumptionPercent;
                        form8.Note = row.Note;
                        form8.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

//                var arrayNotExtendableRowCodes = new[] { "01-0", /*"02-0",*/ "05-0" };
//                if (!arrayNotExtendableRowCodes.Contains(form11.EAUDIT_DIC_TypeResource.Code))
//                {
//                    attachment1.IndustryForm8Rows.Add(new EAUDIT_IndustryForm2()
//                    {
//                        EAUDIT_Preamble = preamble,
//                        EAUDIT_DIC_TypeResource = type,
//                        IsCommand = true,
//                        InnerOrder = 1000
//                    });
//                }

                attachment1.IndustryForm8Rows.Add(form8);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form8 = new EAUDIT_IndustryForm8()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        TotalConsumption = source.TotalConsumption,
                        RegulatoryConsumptionPercent = source.RegulatoryConsumptionPercent,
                        RegulatoryConsumptionValue = source.RegulatoryConsumptionValue,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm8Rows.Add(form8);
                }
            }

            // rowSpan count
            foreach (var form8 in attachment1.IndustryForm8Rows)
            {
                form8.RowSpan = attachment1.IndustryForm8Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form8.EAUDIT_DIC_TypeResource.PosIndex);
            }
            
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm8Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm8).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm8
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm8/_IndustryForm8View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm9View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm9> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm9.Count > 0)
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm9.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm9.Where(f => f.refOwnedFacility == null).ToList();
            else
                industryRows = new List<EAUDIT_IndustryForm9>()
                {
                    new EAUDIT_IndustryForm9()
                };

            attachment1.IndustryForm9Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm9Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm9).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm9
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm9/_IndustryForm9View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm10View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm10> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm10.Count > 0)
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm10.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm10.Where(f => f.refOwnedFacility == null).ToList();
            else
                industryRows = new List<EAUDIT_IndustryForm10>()
                {
                    new EAUDIT_IndustryForm10()
                };

            attachment1.IndustryForm10Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm10Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm10).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm10
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm10/_IndustryForm10View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm11View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm11> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm11.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm11.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm11.Where(f => f.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm11, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm11Rows = new List<EAUDIT_IndustryForm11>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm11()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.FactValueHeating = row.FactValueHeating;
                        form.FactValueForcedVentilation = row.FactValueForcedVentilation;
                        form.FactValueHotWaterSupply = row.FactValueHotWaterSupply;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                {
                    attachment1.IndustryForm11Rows.Add(new EAUDIT_IndustryForm11()
                    {
                        EAUDIT_Preamble = preamble,
                        EAUDIT_DIC_TypeResource = type,
                        IsCommand = true,
                        InnerOrder = 1000
                    });
                }

                attachment1.IndustryForm11Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_IndustryForm11()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        Name = source.Name,
                        FactValueHeating = source.FactValueHeating,
                        FactValueForcedVentilation = source.FactValueForcedVentilation,
                        FactValueHotWaterSupply = source.FactValueHotWaterSupply,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm11Rows.Add(form);
                }
            }

            // rowSpan count
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm11Rows)
            {
                form.RowSpan = attachment1.IndustryForm11Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_IndustryForm11).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm11
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm11/_IndustryForm11View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm12View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm12> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm12.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm12.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm12.Where(f => f.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm12, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm12Rows = new List<EAUDIT_IndustryForm12>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm12()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.ParameterHeatTransferAgent = row.ParameterHeatTransferAgent;
                        form.ParameterPressure = row.ParameterPressure;
                        form.ParameterTemperature = row.ParameterTemperature;
                        form.TotalConsumption = row.TotalConsumption;
                        form.ConsumtionRegularLossValue = row.ConsumtionRegularLossValue;
                        form.ConsumtionRegularLossPercent = row.ConsumtionRegularLossPercent;
                        form.Losses = row.Losses;
                        form.Condensate = row.Condensate;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

//                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
//                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
//                {
//                    attachment1.IndustryForm11Rows.Add(new EAUDIT_IndustryForm11()
//                    {
//                        EAUDIT_Preamble = preamble,
//                        EAUDIT_DIC_TypeResource = type,
//                        IsCommand = true,
//                        InnerOrder = 1000
//                    });
//                }

                attachment1.IndustryForm12Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_IndustryForm12()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        ParameterHeatTransferAgent = source.ParameterHeatTransferAgent,
                        ParameterPressure = source.ParameterPressure,
                        ParameterTemperature = source.ParameterTemperature,
                        TotalConsumption = source.TotalConsumption,
                        ConsumtionRegularLossValue = source.ConsumtionRegularLossValue,
                        ConsumtionRegularLossPercent = source.ConsumtionRegularLossPercent,
                        Losses = source.Losses,
                        Condensate = source.Condensate,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm12Rows.Add(form);
                }
            }

            // rowSpan count
            
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm12Rows)
            {
                form.RowSpan = attachment1.IndustryForm12Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_IndustryForm12).GetProperties())
                { 
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm12
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm12/_IndustryForm12View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm13View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm13> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm13.Count > 0)
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm13.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm13.Where(f => f.refOwnedFacility == null).ToList();
            else
                industryRows = new List<EAUDIT_IndustryForm13>()
                {
                    new EAUDIT_IndustryForm13()
                };

            attachment1.IndustryForm13Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm13Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm13).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm13
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm13/_IndustryForm13View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm14View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm14> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm14.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm14.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm14.Where(f => f.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm14, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm14Rows = new List<EAUDIT_IndustryForm14>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm14()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.TotalConsumption = row.TotalConsumption;
                        form.RegulatoryConsumtion = row.RegulatoryConsumtion;
                        form.EnergyLoss = row.EnergyLoss;
                        form.Efficiency = row.Efficiency;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                var arrayExtendableRowCodes = new[] { "01-0" };
                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                {
                    attachment1.IndustryForm14Rows.Add(new EAUDIT_IndustryForm14()
                    {
                        EAUDIT_Preamble = preamble,
                        EAUDIT_DIC_TypeResource = type,
                        IsCommand = true,
                        InnerOrder = 1000
                    });
                }

                attachment1.IndustryForm14Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_IndustryForm14()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        Name = source.Name,
                        TotalConsumption = source.TotalConsumption,
                        RegulatoryConsumtion = source.RegulatoryConsumtion,
                        EnergyLoss = source.EnergyLoss,
                        Efficiency = source.Efficiency,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm14Rows.Add(form);
                }
            }

            // rowSpan count
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm14Rows)
            {
                form.RowSpan = attachment1.IndustryForm14Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_IndustryForm14).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm14
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);
            
            return PartialView("~/Views/EnergyAudit/IndustryForm14/_IndustryForm14View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm15View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;
            List<EAUDIT_IndustryForm15> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm15.Count > 0)
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm15.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm15.Where(f => f.refOwnedFacility == null).ToList();
            else
                industryRows = new List<EAUDIT_IndustryForm15>()
                {
                    new EAUDIT_IndustryForm15()
                };

            attachment1.IndustryForm15Rows = industryRows;
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm15Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryForm15).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm15
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm15/_IndustryForm15View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm16View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm16> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm16.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm16.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm16.Where(f => f.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm16, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm16Rows = new List<EAUDIT_IndustryForm16>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm16()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.TotalConsumption = row.TotalConsumption;
                        form.RegularConsumption = row.RegularConsumption;
                        form.LossesInavoidable = row.LossesInavoidable;
                        form.LossesFact = row.LossesFact;
                        form.SpecificConsumption = row.SpecificConsumption;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                var arrayExtendableRowCodes = new[] { "01-0" };
                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                {
                    attachment1.IndustryForm16Rows.Add(new EAUDIT_IndustryForm16()
                    {
                        EAUDIT_Preamble = preamble,
                        EAUDIT_DIC_TypeResource = type,
                        IsCommand = true,
                        InnerOrder = 1000
                    });
                }

                attachment1.IndustryForm16Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_IndustryForm16()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Name = source.Name,
                        Note = source.Note,
                        TotalConsumption = source.TotalConsumption,
                        RegularConsumption = source.RegularConsumption,
                        LossesInavoidable = source.LossesInavoidable,
                        LossesFact = source.LossesFact,
                        SpecificConsumption = source.SpecificConsumption,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm16Rows.Add(form);
                }
            }

            // rowSpan count
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm16Rows)
            {
                form.RowSpan = attachment1.IndustryForm16Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_IndustryForm16).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm16
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);
            
            return PartialView("~/Views/EnergyAudit/IndustryForm16/_IndustryForm16View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm17View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm17> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm17.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm17.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm17.Where(f => f.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm17, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm17Rows = new List<EAUDIT_IndustryForm17>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm17()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.Unit = row.Unit;
                        form.Value = row.Value;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

//                var arrayExtendableRowCodes = new[] { "01-0", "02-0" };
//                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
//                {
//                    attachment1.IndustryForm17Rows.Add(new EAUDIT_IndustryForm17()
//                    {
//                        EAUDIT_Preamble = preamble,
//                        EAUDIT_DIC_TypeResource = type,
//                        IsCommand = true,
//                        InnerOrder = 1000
//                    });
//                }

                attachment1.IndustryForm17Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_IndustryForm17()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        Name = source.Name,
                        Value = source.Value,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm17Rows.Add(form);
                }
            }

            // rowSpan count
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm17Rows)
            {
                form.RowSpan = attachment1.IndustryForm17Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_IndustryForm17).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm17
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);
            
            return PartialView("~/Views/EnergyAudit/IndustryForm17/_IndustryForm17View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm18View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm18> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm18.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm18.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm18.Where(f => f.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm18, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm18Rows = new List<EAUDIT_IndustryForm18>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm18()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.Unit = row.Unit;
                        form.ActualConsumtion = row.ActualConsumtion;
                        form.EstimatedConsumtionCurrent = row.EstimatedConsumtionCurrent;
                        form.EstimatedConsumtion2 = row.EstimatedConsumtion2;
                        form.EstimatedConsumtion3 = row.EstimatedConsumtion3;
                        form.EstimatedConsumtion4 = row.EstimatedConsumtion4;
                        form.EstimatedConsumtion5 = row.EstimatedConsumtion5;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                var arrayExtendableRowCodes = new[] {"01-1", "01-2", "01-3", "02-1", "03-1", "03-2", "03-3"};
                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                {
                    attachment1.IndustryForm18Rows.Add(new EAUDIT_IndustryForm18()
                    {
                        EAUDIT_Preamble = preamble,
                        EAUDIT_DIC_TypeResource = type,
                        IsCommand = true,
                        InnerOrder = 1000
                    });
                }

                attachment1.IndustryForm18Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_IndustryForm18()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        Name = source.Name,
                        Unit = source.Unit,
                        ActualConsumtion = source.ActualConsumtion,
                        EstimatedConsumtionCurrent = source.EstimatedConsumtionCurrent,
                        EstimatedConsumtion2 = source.EstimatedConsumtion2,
                        EstimatedConsumtion3 = source.EstimatedConsumtion3,
                        EstimatedConsumtion4 = source.EstimatedConsumtion4,
                        EstimatedConsumtion5 = source.EstimatedConsumtion5,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm18Rows.Add(form);
                }
            }

            // rowSpan count
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm18Rows)
            {
                form.RowSpan = attachment1.IndustryForm18Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_IndustryForm18).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm18
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm18/_IndustryForm18View.cshtml", attachment1);
        }

        public ActionResult GetIndustryForm19View(long preambleId, long? ofacility, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment1 = new EauditAttachment1();
            attachment1.Preamble = preamble;
            attachment1.RefOwnedFacilities = ofacility;

            List<EAUDIT_IndustryForm19> industryRows = null;

            if (preamble != null && preamble.EAUDIT_IndustryForm19.Count > 0)
            {
                if (ofacility.HasValue)
                    industryRows = preamble.EAUDIT_IndustryForm19.Where(f => f.refOwnedFacility == ofacility.Value).ToList();
                else
                    industryRows = preamble.EAUDIT_IndustryForm19.Where(f => f.refOwnedFacility == null).ToList();
            }
            var typeRepository = new EAuditDicTypeRepository();
            var types = typeRepository
                .GetQuery(tr => tr.FormCode == EnergyAuditFormConsts.IndustryForm19, true, tr => tr.PosIndex ?? 0)
                .ToList();
            attachment1.IndustryForm19Rows = new List<EAUDIT_IndustryForm19>();

            foreach (var type in types)
            {
                //int rowSpan = types.Count(t => t.PosIndex == type.PosIndex);

                var form = new EAUDIT_IndustryForm19()
                {
                    refPreamble = preambleId,
                    refTypeResource = type.Id,
                    EAUDIT_DIC_TypeResource = type,
                    Id = 0,
                    EAUDIT_Preamble = preamble,
                    RowSpan = 0,
                    InnerOrder = 0,
                    IsAdditionalRow = false,
                    IsCommand = false
                };

                var typeId = type.Id;
                if (industryRows != null)
                {
                    foreach (var row in industryRows.Where(ir => ir.refPreamble == preambleId
                          && ir.refTypeResource == typeId && (ir.IsAdditionalRow == null || !ir.IsAdditionalRow.Value)))
                    {
                        form.Id = row.Id;
                        form.Name = row.Name;
                        form.Expenses = row.Expenses;
                        form.SavingResourceInNatural = row.SavingResourceInNatural;
                        form.SavingResourceInValue = row.SavingResourceInValue;
                        form.ImplementationDeadline = row.ImplementationDeadline;
                        form.PaybackPeriod = row.PaybackPeriod;
                        form.Note = row.Note;
                        form.IsAdditionalRow = row.IsAdditionalRow;
                    }
                }

                var arrayExtendableRowCodes = new[] { "01-1", "01-2", "01-3", "01-4", "01-5", "01-6", "01-7", "01-8" };
                if (arrayExtendableRowCodes.Contains(form.EAUDIT_DIC_TypeResource.Code))
                {
                    attachment1.IndustryForm19Rows.Add(new EAUDIT_IndustryForm19()
                    {
                        EAUDIT_Preamble = preamble,
                        EAUDIT_DIC_TypeResource = type,
                        IsCommand = true,
                        InnerOrder = 1000
                    });
                }

                attachment1.IndustryForm19Rows.Add(form);
            }

            if (industryRows != null)
            {
                foreach (var source in industryRows.Where(ir => ir.refPreamble == preambleId
                               && (ir.IsAdditionalRow != null && ir.IsAdditionalRow.Value)))
                {
                    var form = new EAUDIT_IndustryForm19()
                    {
                        refPreamble = preambleId,
                        refTypeResource = source.EAUDIT_DIC_TypeResource.Id,
                        EAUDIT_DIC_TypeResource = source.EAUDIT_DIC_TypeResource,
                        Id = source.Id,
                        EAUDIT_Preamble = preamble,
                        Note = source.Note,
                        Name = source.Name,
                        Expenses = source.Expenses,
                        SavingResourceInNatural = source.SavingResourceInNatural,
                        SavingResourceInValue = source.SavingResourceInValue,
                        ImplementationDeadline = source.ImplementationDeadline,
                        PaybackPeriod = source.PaybackPeriod,
                        IsAdditionalRow = source.IsAdditionalRow,
                        InnerOrder = 1,
                        IsCommand = false,
                    };
                    attachment1.IndustryForm19Rows.Add(form);
                }
            }

            // rowSpan count
            attachment1.FieldComments = new List<EAUDIT_FieldComments>();
            foreach (var form in attachment1.IndustryForm19Rows)
            {
                form.RowSpan = attachment1.IndustryForm19Rows
                    .Count(ifr => ifr.EAUDIT_DIC_TypeResource.PosIndex == form.EAUDIT_DIC_TypeResource.PosIndex);
                foreach (var pInfo in typeof(EAUDIT_IndustryForm19).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryForm19
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment1.FieldComments.Add(fieldComment);
                }
            }
            InitAttachment1AdditionalFields(isReadOnly, ref attachment1);

            return PartialView("~/Views/EnergyAudit/IndustryForm19/_IndustryForm19View.cshtml", attachment1);
        }

        public JsonResult UpdateOwnedFacility(EAUDIT_OwnedFacility formOf)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(formOf, EnergyAuditFormConsts.OwnedFacility);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formOf.Id,
            });
        }

        public JsonResult UpdateShop(EAUDIT_IndustryForm4_Shop formShop)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(formShop, EnergyAuditFormConsts.IndustryForm4Shop);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formShop.Id,
            });
        }

        public JsonResult UpdateForm1(EAUDIT_IndustryForm1 form1)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form1, EnergyAuditFormConsts.IndustryForm1);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form1.Id,
            });
        }

        public JsonResult SaveForm1(long refPreamble, int baseyear, int currentyear)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.UpdateForm1(refPreamble,baseyear, currentyear);
            return Json(new
            {
                IsSuccess = isSuccess,
            });
        }

        public JsonResult UpdateForm2(EAUDIT_IndustryForm2 form2)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form2, EnergyAuditFormConsts.IndustryForm2);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form2.Id,
            });
        }

        public JsonResult UpdateForm3(EAUDIT_IndustryForm3 form3)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form3, EnergyAuditFormConsts.IndustryForm3);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form3.Id,
            });
        }

        public JsonResult UpdateForm4(EAUDIT_IndustryForm4 form4)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm4(form4);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form4.Id,
            });
        }

        public JsonResult UpdateForm5(EAUDIT_IndustryForm5 form5)
        {
            
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form5, EnergyAuditFormConsts.IndustryForm5);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form5.Id,
            });
        }

        public JsonResult UpdateForm6(EAUDIT_IndustryForm6 form6)
        {

            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form6, EnergyAuditFormConsts.IndustryForm6);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form6.Id,
            });
        }

        public JsonResult UpdateForm7(EAUDIT_IndustryForm7 form7)
        {

            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form7, EnergyAuditFormConsts.IndustryForm7);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form7.Id,
            });
        }

        public JsonResult UpdateForm8(EAUDIT_IndustryForm8 form8)
        {

            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form8, EnergyAuditFormConsts.IndustryForm8);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form8.Id,
            });
        }

        public JsonResult UpdateForm9(EAUDIT_IndustryForm9 form9)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form9, EnergyAuditFormConsts.IndustryForm9);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form9.Id,
            });
        }

        public JsonResult UpdateForm10(EAUDIT_IndustryForm10 form10)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form10, EnergyAuditFormConsts.IndustryForm10);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form10.Id,
            });
        }

        public JsonResult UpdateForm11(EAUDIT_IndustryForm11 form11)
        {

            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form11, EnergyAuditFormConsts.IndustryForm11);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form11.Id,
            });
        }

        public JsonResult UpdateForm12(EAUDIT_IndustryForm12 form12)
        {

            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form12, EnergyAuditFormConsts.IndustryForm12);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form12.Id,
            });
        }

        public JsonResult UpdateForm13(EAUDIT_IndustryForm13 form13)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form13, EnergyAuditFormConsts.IndustryForm13);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form13.Id,
            });
        }

        public JsonResult UpdateForm14(EAUDIT_IndustryForm14 form14)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form14, EnergyAuditFormConsts.IndustryForm14);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form14.Id,
            });
        }

        public JsonResult UpdateForm15(EAUDIT_IndustryForm15 form15)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form15, EnergyAuditFormConsts.IndustryForm15);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form15.Id,
            });
        }

        public JsonResult UpdateForm16(EAUDIT_IndustryForm16 form16)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form16, EnergyAuditFormConsts.IndustryForm16);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form16.Id,
            });
        }

        public JsonResult UpdateForm17(EAUDIT_IndustryForm17 form17)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form17, EnergyAuditFormConsts.IndustryForm17);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form17.Id,
            });
        }

        public JsonResult UpdateForm18(EAUDIT_IndustryForm18 form18)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form18, EnergyAuditFormConsts.IndustryForm18);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form18.Id,
            });
        }

        public JsonResult UpdateForm19(EAUDIT_IndustryForm19 form19)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryForm(form19, EnergyAuditFormConsts.IndustryForm19);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = form19.Id,
            });
        }

        public ActionResult Delete(long id, string industryFormCode)
        {
            var repository = new EnergyAuditRepository();
            repository.DeleteForm(id, industryFormCode);
            return Json(new { IsSuccess = true });
        }

        private void InitAttachment1AdditionalFields(bool? isReadOnly, ref EauditAttachment1 attachment1)
        {
            attachment1.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            if (Session["SignedValue"] != null)
                attachment1.SignedEauditPreamble = (EAUDIT_Preamble)Session["SignedValue"];
        }
    }
}