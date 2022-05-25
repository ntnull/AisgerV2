using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Security;
using FlowDoc.Models.Repository.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Dictionary
{
    public class DirectoriesController : Controller
    {
        //
        // GET: /Directories/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View();
        }

		#region rst_report (years list)
		public ActionResult TabYears()
		{
			var model = new RstReportRepository().GetRstReport();
			return PartialView("~/Views/Directories/TabYears.cshtml", model);
		}

		public ActionResult SaveIsActive(long id)
		{
			string errorMessage = new RstReportRepository().UpdateRstReportIsActive(id);
			Dictionary<string, object> item = new Dictionary<string, object>();
			item["ErrorMessage"] = errorMessage;
			return Json(item);
		}

		#endregion

		#region 
		public ActionResult TabSubDicKindResource()
		{
			var model = new SubDicKindResourceRepository().GetCollectionList();
			return PartialView("~/Views/Directories/TabSubDicKindResource.cshtml", model);
		}
		#endregion

		#region 
		public ActionResult TabSubDicTypeResource()
		{
			var model = new SubDicTypeResourceRepository().GetCollectionList();
			return PartialView("~/Views/Directories/TabSubDicTypeResource.cshtml", model);
		}
		#endregion

		#region 
		public ActionResult TabDicOrganisation()
		{
			var model = new DicOrganisationRepository().GetAll();
			return PartialView("~/Views/Directories/TabDicOrganisation.cshtml", model);
		}
		#endregion

		#region
		public ActionResult TabDicDepartment()
		{
			var model = new DicDepartmentRepository().GetAll();
			return PartialView("~/Views/Directories/TabDicDepartment.cshtml", model);
		}
		#endregion

		#region 
		public ActionResult TabDicUnit()
		{
			var model = new DicUnitRepository().GetAll();
			return PartialView("~/Views/Directories/TabDicUnit.cshtml", model);
		}
		#endregion

		#region
		public ActionResult TabDicOked()
		{
			var model = new DicOkedRepository().GetAll().Where(o => o.refParent == null).ToList();
			return PartialView("~/Views/Directories/TabDicOked.cshtml", model);
		}
		#endregion

		#region
		public ActionResult TabDicHolidays()
		{
			var model = new DicHolidaysRepository().GetList();
			return PartialView("~/Views/Directories/TabDicHolidays.cshtml", model);
		}
        #endregion

        #region
        public ActionResult TabSubDicEnergyindicator()
        {
            var model = new SubDicEnergyindicatorRepository().GetSubDicEnergyindicatorList();
            return PartialView("~/Views/Directories/TabSubDicEnergyindicator.cshtml", model);
        }

        public ActionResult SubDicEnergyindicatorEdit(int id)
        {
            var model = new SubDicEnergyindicatorRepository().GetSubDicEnergyindicatorById(id);
            return View("SubDicEnergyindicatorCreate",model);
        }

        public ActionResult SubDicEnergyindicatorCreate()
        {
            var model = new sub_dic_energyindicator();
            model.forgu = false;
            return View(model);
        }

        [HttpPost]
        public ActionResult SubDicEnergyindicatorCreate(sub_dic_energyindicator model)
        {
            var errorMessage = new SubDicEnergyindicatorRepository().SaveSubDicEnergyindicator(model);
            if (errorMessage == "")
            {
                return Redirect("Index");
            }
            return View(model);
        }

        public ActionResult SubDicEnergyindicatorDelete(int id)
        {
            var errorMessage = new SubDicEnergyindicatorRepository().DeleteSubDicEnergyindicatorById(id);
            if (errorMessage == "")
            {
                return RedirectToAction("/Index");
            }

            return Redirect("/Index");
        }
        #endregion
    }
}