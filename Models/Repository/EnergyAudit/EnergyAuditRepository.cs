using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Constants;
using NPOI.POIFS.Storage;
using WebGrease.Css.Extensions;
using System.Data.Entity;

namespace Aisger.Models.Repository.EnergyAudit
{
    public class EnergyAuditRepository : AObjectRepository<EAUDIT_Preamble>, IDisposable
    {
        public IEnumerable<EAUDIT_Preamble> GetPreambleList()
        {
            return GetQueryByDescending(e => !e.IsDeleted, true, e => e.Id).ToList();
        }

        public IEnumerable<EAUDIT_Preamble> GetPreambleList(long? auditorId)
        {
            if (auditorId.HasValue)
                return GetQueryByDescending(e => !e.IsDeleted && e.refAuditor == auditorId, true, e => e.Id).ToList();
            return GetPreambleList();
        }

        public IEnumerable<EAUDIT_Preamble> GetPreambleList(string statusCode)
        {
            if (!string.IsNullOrEmpty(statusCode))
                return GetQueryByDescending(e => !e.IsDeleted && e.EAUDIT_DIC_Statuses.Code == statusCode, true, e => e.Id).ToList();
            return GetPreambleList();
        }

        public IEnumerable<EAUDIT_Preamble> GetPreambleList(List<string> statusCodes)
        {
            if (statusCodes != null && statusCodes.Count > 0)
                return GetQueryByDescending(e => !e.IsDeleted && statusCodes.Contains(e.EAUDIT_DIC_Statuses.Code), true, e => e.Id).ToList();
            return GetPreambleList();
        }

        public EAUDIT_Preamble GetPreamble(long id)
        {
            var context = CreateDatabaseContext(false);
            var preamble = context.EAUDIT_Preamble.Include(p => p.EAUDIT_AttachedFiles)
                .Include(p => p.EAUDIT_Building)
                .Include(p => p.EAUDIT_BuildingForm1)
                .Include(p => p.EAUDIT_BuildingForm2)
                .Include(p => p.EAUDIT_BuildingForm3)
                .Include(p => p.EAUDIT_BuildingForm4)
                .Include(p => p.EAUDIT_BuildingForm5)
                .Include(p => p.EAUDIT_BuildingForm6)
                .Include(p => p.EAUDIT_BuildingForm7)
                .Include(p => p.EAUDIT_BuildingForm9)
                .Include(p => p.EAUDIT_IndustryBuildingForm1)
                .Include(p => p.EAUDIT_IndustryForm1)
                .Include(p => p.EAUDIT_IndustryForm2)
                .Include(p => p.EAUDIT_IndustryForm3)
                .Include(p => p.EAUDIT_IndustryForm4)
                .Include(p => p.EAUDIT_IndustryForm5)
                .Include(p => p.EAUDIT_IndustryForm6)
                .Include(p => p.EAUDIT_IndustryForm7)
                .Include(p => p.EAUDIT_IndustryForm8)
                .Include(p => p.EAUDIT_IndustryForm9)
                .Include(p => p.EAUDIT_IndustryForm10)
                .Include(p => p.EAUDIT_IndustryForm11)
                .Include(p => p.EAUDIT_IndustryForm12)
                .Include(p => p.EAUDIT_IndustryForm13)
                .Include(p => p.EAUDIT_IndustryForm14)
                .Include(p => p.EAUDIT_IndustryForm15)
                .Include(p => p.EAUDIT_IndustryForm16)
                .Include(p => p.EAUDIT_IndustryForm17)
                .Include(p => p.EAUDIT_IndustryForm18)
                .Include(p => p.EAUDIT_IndustryForm19)
                .Include(p => p.EAUDIT_OwnedFacility)
                .AsNoTracking().FirstOrDefault(e => e.Id == id);
            return preamble;
        }

        public EAUDIT_Preamble PreambleCutParentRef(EAUDIT_Preamble preamble)
        {
            foreach (var e in preamble.EAUDIT_Building)
            {
                e.EAUDIT_BuildingForm1 = null;
                e.EAUDIT_BuildingForm2 = null;
                e.EAUDIT_BuildingForm3 = null;
                e.EAUDIT_BuildingForm4 = null;
                e.EAUDIT_BuildingForm5 = null;
                e.EAUDIT_BuildingForm6 = null;
                e.EAUDIT_BuildingForm7 = null;
                e.EAUDIT_BuildingForm9 = null;
                e.EAUDIT_Preamble = null;
            }

            foreach (var e in preamble.EAUDIT_BuildingForm1)
            {
                e.EAUDIT_Preamble = null;
            }

            return preamble;
        }

        public void Dispose()
        {
            
        }

        /// <summary>
        /// Industry Forms 
        /// </summary>
        /// <param name="form"></param>
        /// <param name="industryFormCode"></param>
        /// <returns></returns>
        public bool SaveOrUpdateIndustryForm(object form, string industryFormCode)
        {
            bool isSuccess = true;
            switch (industryFormCode)
            {
                case EnergyAuditFormConsts.IndustryForm1:
                    var form1 = (EAUDIT_IndustryForm1)form;
                    AppContext.EAUDIT_IndustryForm1.AddOrUpdate(form1);
                    break;
                case EnergyAuditFormConsts.IndustryForm2:
                    var form2 = (EAUDIT_IndustryForm2) form;
                    AppContext.EAUDIT_IndustryForm2.AddOrUpdate(form2);
                    break;
                case EnergyAuditFormConsts.IndustryForm3:
                    var form3 = (EAUDIT_IndustryForm3)form;
                    AppContext.EAUDIT_IndustryForm3.AddOrUpdate(form3);
                    break;
                case EnergyAuditFormConsts.IndustryForm4:
                    var form4 = (EAUDIT_IndustryForm4)form;
                    AppContext.EAUDIT_IndustryForm4.AddOrUpdate(form4);
                    break;
                case EnergyAuditFormConsts.IndustryForm5:
                    var form5 = (EAUDIT_IndustryForm5)form;
                    AppContext.EAUDIT_IndustryForm5.AddOrUpdate(form5);
                    break;
                case EnergyAuditFormConsts.IndustryForm6:
                    var form6 = (EAUDIT_IndustryForm6)form;
                    AppContext.EAUDIT_IndustryForm6.AddOrUpdate(form6);
                    break;
                case EnergyAuditFormConsts.IndustryForm7:
                    var form7 = (EAUDIT_IndustryForm7)form;
                    AppContext.EAUDIT_IndustryForm7.AddOrUpdate(form7);
                    break;
                case EnergyAuditFormConsts.IndustryForm8:
                    var form8 = (EAUDIT_IndustryForm8)form;
                    AppContext.EAUDIT_IndustryForm8.AddOrUpdate(form8);
                    break;
                case EnergyAuditFormConsts.IndustryForm9:
                    var form9 = (EAUDIT_IndustryForm9)form;
                    AppContext.EAUDIT_IndustryForm9.AddOrUpdate(form9);
                    break;
                case EnergyAuditFormConsts.IndustryForm10:
                    var form10 = (EAUDIT_IndustryForm10)form;
                    AppContext.EAUDIT_IndustryForm10.AddOrUpdate(form10);
                    break;
                case EnergyAuditFormConsts.IndustryForm11:
                    var form11 = (EAUDIT_IndustryForm11)form;
                    AppContext.EAUDIT_IndustryForm11.AddOrUpdate(form11);
                    break;
                case EnergyAuditFormConsts.IndustryForm12:
                    var form12 = (EAUDIT_IndustryForm12)form;
                    AppContext.EAUDIT_IndustryForm12.AddOrUpdate(form12);
                    break;
                case EnergyAuditFormConsts.IndustryForm13:
                    var form13 = (EAUDIT_IndustryForm13)form;
                    AppContext.EAUDIT_IndustryForm13.AddOrUpdate(form13);
                    break;
                case EnergyAuditFormConsts.IndustryForm14:
                    var form14 = (EAUDIT_IndustryForm14)form;
                    AppContext.EAUDIT_IndustryForm14.AddOrUpdate(form14);
                    break;
                case EnergyAuditFormConsts.IndustryForm15:
                    var form15 = (EAUDIT_IndustryForm15)form;
                    AppContext.EAUDIT_IndustryForm15.AddOrUpdate(form15);
                    break;
                case EnergyAuditFormConsts.IndustryForm16:
                    var form16 = (EAUDIT_IndustryForm16)form;
                    AppContext.EAUDIT_IndustryForm16.AddOrUpdate(form16);
                    break;
                case EnergyAuditFormConsts.IndustryForm17:
                    var form17 = (EAUDIT_IndustryForm17)form;
                    AppContext.EAUDIT_IndustryForm17.AddOrUpdate(form17);
                    break;
                case EnergyAuditFormConsts.IndustryForm18:
                    var form18 = (EAUDIT_IndustryForm18)form;
                    AppContext.EAUDIT_IndustryForm18.AddOrUpdate(form18);
                    break;
                case EnergyAuditFormConsts.IndustryForm19:
                    var form19 = (EAUDIT_IndustryForm19)form;
                    AppContext.EAUDIT_IndustryForm19.AddOrUpdate(form19);
                    break;
                case EnergyAuditFormConsts.OwnedFacility:
                    var formOf = (EAUDIT_OwnedFacility)form;
                    AppContext.EAUDIT_OwnedFacility.AddOrUpdate(formOf);
                    break;
                case EnergyAuditFormConsts.IndustryForm4Shop:
                    var formShop = (EAUDIT_IndustryForm4_Shop)form;
                    AppContext.EAUDIT_IndustryForm4_Shop.AddOrUpdate(formShop);
                    break;
            }
            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool SaveOrUpdateIndustryForm4(EAUDIT_IndustryForm4 form)
        {
            bool isSuccess = true;
            var form4 = form;
            if (form.Id != 0)
            {
                var formOld = AppContext.EAUDIT_IndustryForm4.FirstOrDefault(f => f.Id == form.Id);
                if (formOld != null)
                {
                    formOld.Note = form.Note;
                    var shopValue = formOld.EAUDIT_IndustryForm4_ShopValues
                    .FirstOrDefault(shv => shv.refShop == form.RefShop && shv.refTypeResource == form.refTypeResource);
                    if (shopValue != null)
                    {
                        shopValue.Power = form.Power;
                        shopValue.Quantity = form.Quantity;
                    }
                    else
                    {
                        formOld.EAUDIT_IndustryForm4_ShopValues.Add(new EAUDIT_IndustryForm4_ShopValues()
                        {
                            Power = form.Power,
                            Quantity = form.Quantity,
                            refShop = form.RefShop,
                            refTypeResource = form.refTypeResource,
                        });
                    }
                    form4 = formOld;
                }
            }
            else
            {
                form4.EAUDIT_IndustryForm4_ShopValues.Add(new EAUDIT_IndustryForm4_ShopValues()
                {
                    Power = form4.Power,
                    Quantity = form4.Quantity,
                    refShop = form4.RefShop,
                    refTypeResource = form4.refTypeResource,
                });

                var ownedFacility = AppContext.EAUDIT_OwnedFacility.FirstOrDefault(of => of.EAUDIT_IndustryForm4_Shop
                        .Any(sh => sh.Id == form.RefShop));
                if (ownedFacility != null)
                {
                    form4.refOwnedFacility = ownedFacility.Id;
                }
            }
            

            AppContext.EAUDIT_IndustryForm4.AddOrUpdate(form4);
            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool DeleteForm(long id, string industryFormCode)
        {
            bool isSuccess = true;
            try
            {
                switch (industryFormCode)
                {
                    case EnergyAuditFormConsts.IndustryForm1:
                        var industryForm1 = AppContext.EAUDIT_IndustryForm1.FirstOrDefault(e => e.Id == id);
                        if (industryForm1 != null)
                            AppContext.EAUDIT_IndustryForm1.Remove(industryForm1);
                        break;
                    case EnergyAuditFormConsts.IndustryForm2:
                        var industryForm2 = AppContext.EAUDIT_IndustryForm2.FirstOrDefault(e => e.Id == id);
                        if (industryForm2 != null)
                            AppContext.EAUDIT_IndustryForm2.Remove(industryForm2);
                        break;
                    case EnergyAuditFormConsts.IndustryForm3:
                        var industryForm3 = AppContext.EAUDIT_IndustryForm3.FirstOrDefault(e => e.Id == id);
                        if (industryForm3 != null)
                            AppContext.EAUDIT_IndustryForm3.Remove(industryForm3);
                        break;
                    case EnergyAuditFormConsts.IndustryForm4:
                        var industryForm4 = AppContext.EAUDIT_IndustryForm4.FirstOrDefault(e => e.Id == id);
                        if (industryForm4 != null)
                            AppContext.EAUDIT_IndustryForm4.Remove(industryForm4);
                        break;
                    case EnergyAuditFormConsts.IndustryForm5:
                        var industryForm5 = AppContext.EAUDIT_IndustryForm5.FirstOrDefault(e => e.Id == id);
                        if (industryForm5 != null)
                            AppContext.EAUDIT_IndustryForm5.Remove(industryForm5);
                        break;
                    case EnergyAuditFormConsts.IndustryForm6:
                        var industryForm6 = AppContext.EAUDIT_IndustryForm6.FirstOrDefault(e => e.Id == id);
                        if (industryForm6 != null)
                            AppContext.EAUDIT_IndustryForm6.Remove(industryForm6);
                        break;
                    case EnergyAuditFormConsts.IndustryForm7:
                        var industryForm7 = AppContext.EAUDIT_IndustryForm7.FirstOrDefault(e => e.Id == id);
                        if (industryForm7 != null)
                            AppContext.EAUDIT_IndustryForm7.Remove(industryForm7);
                        break;
                    case EnergyAuditFormConsts.IndustryForm8:
                        var industryForm8 = AppContext.EAUDIT_IndustryForm8.FirstOrDefault(e => e.Id == id);
                        if (industryForm8 != null)
                            AppContext.EAUDIT_IndustryForm8.Remove(industryForm8);
                        break;
                    case EnergyAuditFormConsts.IndustryForm9:
                        var industryForm9 = AppContext.EAUDIT_IndustryForm9.FirstOrDefault(e => e.Id == id);
                        if (industryForm9 != null)
                            AppContext.EAUDIT_IndustryForm9.Remove(industryForm9);
                        break;
                    case EnergyAuditFormConsts.IndustryForm10:
                        var industryForm10 = AppContext.EAUDIT_IndustryForm10.FirstOrDefault(e => e.Id == id);
                        if (industryForm10 != null)
                            AppContext.EAUDIT_IndustryForm10.Remove(industryForm10);
                        break;
                    case EnergyAuditFormConsts.IndustryForm11:
                        var industryForm11 = AppContext.EAUDIT_IndustryForm11.FirstOrDefault(e => e.Id == id);
                        if (industryForm11 != null)
                            AppContext.EAUDIT_IndustryForm11.Remove(industryForm11);
                        break;
                    case EnergyAuditFormConsts.IndustryForm12:
                        var industryForm12 = AppContext.EAUDIT_IndustryForm12.FirstOrDefault(e => e.Id == id);
                        if (industryForm12 != null)
                            AppContext.EAUDIT_IndustryForm12.Remove(industryForm12);
                        break;
                    case EnergyAuditFormConsts.IndustryForm13:
                        var industryForm13 = AppContext.EAUDIT_IndustryForm13.FirstOrDefault(e => e.Id == id);
                        if (industryForm13 != null)
                            AppContext.EAUDIT_IndustryForm13.Remove(industryForm13);
                        break;
                    case EnergyAuditFormConsts.IndustryForm14:
                        var industryForm14 = AppContext.EAUDIT_IndustryForm14.FirstOrDefault(e => e.Id == id);
                        if (industryForm14 != null)
                            AppContext.EAUDIT_IndustryForm14.Remove(industryForm14);
                        break;
                    case EnergyAuditFormConsts.IndustryForm15:
                        var industryForm15 = AppContext.EAUDIT_IndustryForm15.FirstOrDefault(e => e.Id == id);
                        if (industryForm15 != null)
                            AppContext.EAUDIT_IndustryForm15.Remove(industryForm15);
                        break;
                    case EnergyAuditFormConsts.IndustryForm16:
                        var industryForm16 = AppContext.EAUDIT_IndustryForm16.FirstOrDefault(e => e.Id == id);
                        if (industryForm16 != null)
                            AppContext.EAUDIT_IndustryForm16.Remove(industryForm16);
                        break;
                    case EnergyAuditFormConsts.IndustryForm17:
                        var industryForm17 = AppContext.EAUDIT_IndustryForm17.FirstOrDefault(e => e.Id == id);
                        if (industryForm17 != null)
                            AppContext.EAUDIT_IndustryForm17.Remove(industryForm17);
                        break;
                    case EnergyAuditFormConsts.IndustryForm18:
                        var industryForm18 = AppContext.EAUDIT_IndustryForm18.FirstOrDefault(e => e.Id == id);
                        if (industryForm18 != null)
                            AppContext.EAUDIT_IndustryForm18.Remove(industryForm18);
                        break;
                    case EnergyAuditFormConsts.IndustryForm19:
                        var industryForm19 = AppContext.EAUDIT_IndustryForm19.FirstOrDefault(e => e.Id == id);
                        if (industryForm19 != null)
                            AppContext.EAUDIT_IndustryForm19.Remove(industryForm19);
                        break;
                    case EnergyAuditFormConsts.OwnedFacility:
                        var formOf = AppContext.EAUDIT_OwnedFacility.FirstOrDefault(e => e.Id == id);
                        if (formOf != null)
                            AppContext.EAUDIT_OwnedFacility.Remove(formOf);
                        break;
                    case EnergyAuditFormConsts.IndustryForm4Shop:
                        var formShop = AppContext.EAUDIT_IndustryForm4_Shop.FirstOrDefault(e => e.Id == id);
                        if (formShop != null)
                            AppContext.EAUDIT_IndustryForm4_Shop.Remove(formShop);
                        break;
                }
                AppContext.SaveChanges();
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        
        public bool UpdateForm1(long refPreamble, int baseyear, int currentyear)
        {
            bool isSuccess = true;
            try
            {
                foreach (var form1 in AppContext.EAUDIT_IndustryForm1.Where(if1 => if1.refPreamble == refPreamble).ToList())
                {
                    var form1Updated = form1;
//                    form1Updated.BaseYear = baseyear;
//                    form1Updated.CurrentYear = currentyear;
                    AppContext.EAUDIT_IndustryForm1.AddOrUpdate(form1Updated);    
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
            }

            return isSuccess;
        }
        
        /// <summary>
        /// Attached file
        /// </summary>
        /// <param name="attachedFile"></param>
        /// <returns></returns>
        public bool SaveAttachedFileData(EAUDIT_AttachedFiles attachedFile)
        {
            bool isSuccess = true;
            try
            {
                AppContext.EAUDIT_AttachedFiles.AddOrUpdate(attachedFile);
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        
        public bool RemoveAttachedFileData(EAUDIT_AttachedFiles attachedFile)
        {
            bool isSuccess = true;
            try
            {
                AppContext.EAUDIT_AttachedFiles.Remove(attachedFile);
                AppContext.SaveChanges();
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        
        /// <summary>
        /// Save or Update Building Forms
        /// </summary>
        /// <param name="form"></param>
        /// <param name="buildingFormCode"></param>
        /// <returns></returns>
        public bool SaveOrUpdateBuildingForm(object form, string buildingFormCode)
        {
            bool isSuccess = true;
            switch (buildingFormCode)
            {
                case EnergyAuditFormConsts.BuildingForm1:
                    var form1 = (EAUDIT_BuildingForm1)form;
                    AppContext.EAUDIT_BuildingForm1.AddOrUpdate(form1);
                    break;
                case EnergyAuditFormConsts.BuildingForm2:
                    var form2 = (EAUDIT_BuildingForm2)form;
                    AppContext.EAUDIT_BuildingForm2.AddOrUpdate(form2);
                    break;
                case EnergyAuditFormConsts.BuildingForm3:
                    var form3 = (EAUDIT_BuildingForm3)form;
                    AppContext.EAUDIT_BuildingForm3.AddOrUpdate(form3);
                    break;
                case EnergyAuditFormConsts.BuildingForm4:
                    var form4 = (EAUDIT_BuildingForm4)form;
                    AppContext.EAUDIT_BuildingForm4.AddOrUpdate(form4);
                    break;
                case EnergyAuditFormConsts.BuildingForm5:
                    var form5 = (EAUDIT_BuildingForm5)form;
                    AppContext.EAUDIT_BuildingForm5.AddOrUpdate(form5);
                    break;
                case EnergyAuditFormConsts.BuildingForm6:
                    var form6 = (EAUDIT_BuildingForm6)form;
                    AppContext.EAUDIT_BuildingForm6.AddOrUpdate(form6);
                    break;
                case EnergyAuditFormConsts.BuildingForm7:
                    var form7 = (EAUDIT_BuildingForm7)form;
                    AppContext.EAUDIT_BuildingForm7.AddOrUpdate(form7);
                    break;
                case EnergyAuditFormConsts.BuildingForm8:
//                    var form9 = (EAUDIT_IndustryForm9)form;
//                    AppContext.EAUDIT_IndustryForm9.AddOrUpdate(form9);
                    break;
                case EnergyAuditFormConsts.BuildingForm9:
                    var form9 = (EAUDIT_BuildingForm9)form;
                    AppContext.EAUDIT_BuildingForm9.AddOrUpdate(form9);
                    break;
                case EnergyAuditFormConsts.Buildings:
                    var formB = (EAUDIT_Building)form;
                    AppContext.EAUDIT_Building.AddOrUpdate(formB);
                    break;
            }
            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool DeleteBuildingForm(long id, string buildingFormCode)
        {
            bool isSuccess = true;
            try
            {
                switch (buildingFormCode)
                {
                    case EnergyAuditFormConsts.BuildingForm1:
                        var form1 = AppContext.EAUDIT_BuildingForm1.FirstOrDefault(e => e.Id == id);
                        if (form1 != null)
                            AppContext.EAUDIT_BuildingForm1.Remove(form1);
                        break;
                    case EnergyAuditFormConsts.BuildingForm2:
                        var form2 = AppContext.EAUDIT_BuildingForm2.FirstOrDefault(e => e.Id == id);
                        if (form2 != null)
                            AppContext.EAUDIT_BuildingForm2.Remove(form2);
                        break;
                    case EnergyAuditFormConsts.BuildingForm3:
                        var form3 = AppContext.EAUDIT_BuildingForm3.FirstOrDefault(e => e.Id == id);
                        if (form3 != null)
                            AppContext.EAUDIT_BuildingForm3.Remove(form3);
                        break;
                    case EnergyAuditFormConsts.BuildingForm4:
                        var form4 = AppContext.EAUDIT_BuildingForm4.FirstOrDefault(e => e.Id == id);
                        if (form4 != null)
                            AppContext.EAUDIT_BuildingForm4.Remove(form4);
                        break;
                    case EnergyAuditFormConsts.BuildingForm5:
                        var form5 = AppContext.EAUDIT_BuildingForm5.FirstOrDefault(e => e.Id == id);
                        if (form5 != null)
                            AppContext.EAUDIT_BuildingForm5.Remove(form5);
                        break;
                    case EnergyAuditFormConsts.BuildingForm6:
                        var form6 = AppContext.EAUDIT_BuildingForm6.FirstOrDefault(e => e.Id == id);
                        if (form6 != null)
                            AppContext.EAUDIT_BuildingForm6.Remove(form6);
                        break;
                    case EnergyAuditFormConsts.BuildingForm7:
                        var form7 = AppContext.EAUDIT_BuildingForm7.FirstOrDefault(e => e.Id == id);
                        if (form7 != null)
                            AppContext.EAUDIT_BuildingForm7.Remove(form7);
                        break;
                    case EnergyAuditFormConsts.BuildingForm8:
//                        var form8 = AppContext.EAUDIT_BuildingForm8.FirstOrDefault(e => e.Id == id);
//                        if (form8 != null)
//                            AppContext.EAUDIT_BuildingForm8.Remove(form8);
                        break;
                    case EnergyAuditFormConsts.BuildingForm9:
                        var form9 = AppContext.EAUDIT_BuildingForm9.FirstOrDefault(e => e.Id == id);
                        if (form9 != null)
                            AppContext.EAUDIT_BuildingForm9.Remove(form9);
                        break;
                    case EnergyAuditFormConsts.Buildings:
                        var formB = AppContext.EAUDIT_Building.FirstOrDefault(e => e.Id == id);
                        if (formB != null)
                            AppContext.EAUDIT_Building.Remove(formB);
                        break;
                }
                AppContext.SaveChanges();
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        
        public bool SaveOrUpdateIndustryBuildingForm(object form, string buildingFormCode)
        {
            bool isSuccess = true;
            switch (buildingFormCode)
            {
                case EnergyAuditFormConsts.IndustryBuildingForm1:
                    var form1 = (EAUDIT_IndustryBuildingForm1)form;
                    AppContext.EAUDIT_IndustryBuildingForm1.AddOrUpdate(form1);
                    break;
            }
            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool DeleteIndustryBuildingForm(long id, string buildingFormCode)
        {
            bool isSuccess = true;
            try
            {
                switch (buildingFormCode)
                {
                    case EnergyAuditFormConsts.IndustryBuildingForm1:
                        var form1 = AppContext.EAUDIT_IndustryBuildingForm1.FirstOrDefault(e => e.Id == id);
                        if (form1 != null)
                            AppContext.EAUDIT_IndustryBuildingForm1.Remove(form1);
                        break;
                }
                AppContext.SaveChanges();
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        
        /// <summary>
        /// Получить коментарии
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="formCode"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public List<EAUDIT_FieldComments> GetFieldComments(string formCode, long rowId, string fieldName)
        {
            var fieldComments = AppContext.EAUDIT_FieldComments.Where(
                fc => fc.RowId == rowId && fc.FormCode == formCode && fc.FieldName == fieldName).ToList();
            return fieldComments;
        }

        /// <summary>
        /// Сохранить комментарии
        /// </summary>
        /// <param name="fieldComment"></param>
        /// <returns></returns>
        public bool SaveFieldComments(EAUDIT_FieldComments fieldComment)
        {
            bool isSuccess = true;
            AppContext.EAUDIT_FieldComments.AddOrUpdate(fieldComment);
            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }
        
        public List<SelectListItem> GetDicStatuses()
        {
            var statuses =AppContext.EAUDIT_DIC_Statuses.Where(ds => !ds.IsDeleted).Select(ds => new SelectListItem()
            {
                Text = ds.NameRu,
                Value = ds.Code
            }).ToList();

            return statuses;
        }

        public List<SelectListItem> GetDicStatuses(List<string> codes)
        {
            var statuses = AppContext.EAUDIT_DIC_Statuses.Where(ds => codes.Contains(ds.Code) && !ds.IsDeleted)
                .Select(ds => new SelectListItem()
            {
                Text = ds.NameRu,
                Value = ds.Code
            }).ToList();

            return statuses;
        }

        public EAUDIT_DIC_Statuses GetDicStatusByCode(string code)
        {
            var status = AppContext.EAUDIT_DIC_Statuses
                .FirstOrDefault(ds => !ds.IsDeleted && ds.Code == code);

            return status;
        }
        
        public EAUDIT_FieldComments GetFieldLastComment(string formCode, long rowId, string fieldName)
        {
            var fieldComment = AppContext.EAUDIT_FieldComments.Where(
                fc => fc.RowId == rowId && fc.FormCode == formCode && fc.FieldName == fieldName)
                .OrderByDescending(fc => fc.DatetimeStamp).FirstOrDefault();
            return fieldComment;
        }

        public bool SaveHistory(EAUDIT_FormHistory formHistory)
        {
            AppContext.EAUDIT_FormHistory.AddOrUpdate(formHistory);
            bool isSuccess = true;
            try
            {
                AppContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception e)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public EAUDIT_FormHistory GetFormHistory(long preambleId)
        {
            var formHistory = AppContext.EAUDIT_FormHistory.Where(fh => fh.refPreamble == preambleId)
                .OrderByDescending(fh => fh.DatetimeStamp)
                .FirstOrDefault();

            return formHistory;
        }
    }
}