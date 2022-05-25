using Aisger.Models;
using Aisger.Models.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using System.Net.Http;
//using System.Web.Http;
using System.Web.Mvc;
using Aisger.Models.Repository;
using Aisger.Models.Repository.Security;
using System.Text;
using Aisger.Models.Entity.Subject;
using Aisger.Models.Repository.Dictionary;
using Aisger.Helpers;

namespace Aisger.Controllers.Security
{
	
	public class RemoveDuplicateController : ACommonController
    {
        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View();
        }

		public JsonResult GetDublicateBin(string biniin="")
		{
			List<DuplicateBin> ListItems = new List<DuplicateBin>();
			string ErrorMessage = new SecUserRepository().GetDuplicate(ref ListItems, biniin);

			return Json(new
			{
				ErrorMessage,
				ListItems
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetByBiniin(string biniin)
		{
			string ErrorMessage = "";
			var ListItems = new List<DuplicateUser>();

			if (!string.IsNullOrEmpty(biniin))
			{
				ErrorMessage = new SecUserRepository().GetDuplicateByBiniin(biniin, ref ListItems); //GetAll().Where(e => e.BINIIN != null && e.BINIIN.Equals(biniin)).ToList();			
			}

			return Json(new
			{
				ErrorMessage,
				ListItems
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ExecuteDuplicate(long userId, string removedIds,string biniin)
		{
			string ErrorMessage = new SecUserRepository().ExecuteDuplicate(userId, removedIds,biniin);
			return Json(new
			{
				ErrorMessage
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult ChangeBiniin()
		{
			return View();
		}

		public ActionResult GetAllSubject(string biniin, int oblast_id)
		{
			List<ChangeBin> ListItems = new List<ChangeBin>();
			string ErrorMessage = new SecUserRepository().GetAllSubject(ref ListItems, biniin, oblast_id);

			return Json(new
			{
				ErrorMessage,
				ListItems
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult SaveBin(long userId, string biniin, string idk, string juridicalname, string rst_id, bool isHaveGes)
		{
			var ErrorMessage = new SecUserRepository().SaveBin(userId, biniin, idk, juridicalname, rst_id, isHaveGes);
			return Json(new
				{
					ErrorMessage
				}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetDicKato()
		{
			var ListItems = new List<SelectListItem>();
			var repository = new KatoRepository();
			var list = repository.GetKatos(1, true);

			ListItems.Add(new SelectListItem() { Value = "-1", Text = ResourceSetting.sByRepublic });
			foreach (var item in list)
			{
				ListItems.Add(new SelectListItem() { Value = item.Id.ToString(), Text = (CultureHelper.GetCurrentCulture().Equals("kz")) ? item.NameKz : item.NameRu });
			}

			return Json(new
			{
				ListItems
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult RestoreRemovedSubject()
		{
			return View();
		}

		public ActionResult GetRemovedSubjects(string biniin)
		{
			List<ChangeBin> ListItems = new List<ChangeBin>();
			string ErrorMessage = new SecUserRepository().GetRemovedSubjects(ref ListItems, biniin);

			return Json(new
			{
				ErrorMessage,
				ListItems
			}, JsonRequestBehavior.AllowGet);
		}

		public ActionResult RestoreSubject(long userId)
		{
			string ErrorMessage = new SecUserRepository().RestoreSubject(userId);
			return Json(new
			{
				ErrorMessage
			});
		}
    }
}