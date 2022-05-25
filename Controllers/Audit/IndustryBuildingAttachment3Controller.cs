using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Constants;
using Aisger.Models.Repository.EnergyAudit;

namespace Aisger.Controllers.Audit
{
    public class IndustryBuildingAttachment3Controller : Controller
    {
        public ActionResult GetIndustryBuildingForm1View(long preambleId, bool? isReadOnly)
        {
            var auditRepository = new EnergyAuditRepository();
            EAUDIT_Preamble preamble = auditRepository.GetById(preambleId);
            var attachment3 = new EauditAttachment3();
            attachment3.Preamble = preamble;

            List<EAUDIT_IndustryBuildingForm1> buildings = null;

            if (preamble != null && preamble.EAUDIT_IndustryBuildingForm1.Count > 0)
                buildings = preamble.EAUDIT_IndustryBuildingForm1.ToList();
            else
                buildings = new List<EAUDIT_IndustryBuildingForm1>()
                {
                    new EAUDIT_IndustryBuildingForm1()
                };

            attachment3.IndustryBuildingForm1Rows = buildings;
            attachment3.FieldComments = new List<EAUDIT_FieldComments>();
            // rowSpan count
            foreach (var form in attachment3.IndustryBuildingForm1Rows)
            {
                foreach (var pInfo in typeof(EAUDIT_IndustryBuildingForm1).GetProperties())
                {
                    var fieldComment = auditRepository.GetFieldLastComment(EnergyAuditFormConsts.IndustryBuildingForm1
                        , form.Id
                        , pInfo.Name);
                    if (fieldComment != null)
                        attachment3.FieldComments.Add(fieldComment);
                }
            }
            attachment3.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            InitAttachment3AdditionalFields(isReadOnly, ref attachment3);
            return PartialView("~/Views/EnergyAudit/IndustryBuildingForm1/_IndustryBuildingForm1View.cshtml", attachment3);
        }

        public JsonResult UpdateIndustryBuilding(EAUDIT_IndustryBuildingForm1 formBuilding)
        {
            var energyRepository = new EnergyAuditRepository();
            bool isSuccess = energyRepository.SaveOrUpdateIndustryBuildingForm(formBuilding, EnergyAuditFormConsts.IndustryBuildingForm1);
            return Json(new
            {
                IsSuccess = isSuccess,
                Id = formBuilding.Id,
            });
        }

        public ActionResult Delete(long id, string buildingFormCode)
        {
            var repository = new EnergyAuditRepository();
            repository.DeleteIndustryBuildingForm(id, buildingFormCode);
            return Json(new { IsSuccess = true });
        }

        private void InitAttachment3AdditionalFields(bool? isReadOnly, ref EauditAttachment3 attachment3)
        {
            attachment3.IsReadOnly = !isReadOnly.HasValue || isReadOnly.Value;
            if (Session["SignedValue"] != null)
                attachment3.SignedEauditPreamble = (EAUDIT_Preamble)Session["SignedValue"];
        }
    }
}
