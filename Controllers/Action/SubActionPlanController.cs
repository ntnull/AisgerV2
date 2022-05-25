using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Controllers.Subject;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Action;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;


namespace Aisger.Controllers.Action
{
    public class SubActionPlanController : AActionController
    {
        //
        // GET: /RegisterForm/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var list = new SubActionPlanRepository().GetListCurrentByUser(MyExtensions.GetCurrentUserId());
            return View(list);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [HttpGet]
        public ActionResult SelectApplication(string searchTerm, long userId, int pageSize, int pageNum)
        {

            var founder = new SubActionPlanRepository().GetTerm(searchTerm, userId);

            Select2PagedResult pagedAttendees = AttendeesToSelect2Format(founder, founder.Count);

            return new JsonpResult
            {
                Data = pagedAttendees,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        private static Select2PagedResult AttendeesToSelect2Format(IEnumerable<TermSearch> attendees, int totalAttendees)
        {
            var jsonAttendees = new Select2PagedResult { Results = new List<Select2Result>() };

            foreach (var a in attendees)
            {
                jsonAttendees.Results.Add(new Select2Result { id = a.Term , text = a.Term });
            }
            jsonAttendees.Total = totalAttendees;

            return jsonAttendees;
        }

        [GerNavigateLogger]
        public ActionResult Create()
        {
            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return View();
            var model = new SUB_ActionPlan
            {
                StatusId = 1,
                UserId = currentUserId,
                ReportYear = new SubActionPlanRepository().CreateReportYear(currentUserId),
                Editor = currentUserId,
                SEC_User1 =  new SecUserRepository().GetById(currentUserId.Value),
                BeginPlanYear = DateTime.Now.Year,
                EndPlanYear = DateTime.Now.Year+4
            };

           
            FillViewBag(model);
            return View(model);
        }
        [HttpPost]
        public virtual ActionResult GetInfoReportYear(long id, int year, long modelId)
        {

            var subForm =
                new SubActionPlanRepository().GetAll()
                    .FirstOrDefault(e => e.ReportYear == year && e.UserId == id && e.Id != modelId);
            bool status = subForm != null;
            if (modelId > 0 && year > 2010 && year < 2030)
            {
                var model = new SubFormRepository().GetById(modelId);
                model.ReportYear = year;
                new SubFormRepository().SaveOrUpdate(model, id);

            }
            return Json(new { Success = status });

        }
        [HttpPost]
        public virtual ActionResult UpdateModel(string code, long modelId, long userId, long editorId, long recordId, int year, string fieldName, string fieldValue, long typeId)
        {
            var filter = new SubActionPlanRepository().UpdateModel(code, modelId, userId, recordId, year, fieldName, fieldValue, editorId, typeId);
            return Json(new { Success = true, formId = filter.ModelId, fromRecordId = filter.RecordId, unique = filter.Unique });
        }
        [HttpPost]
        public virtual ActionResult DeleteRecord(string code, long recordId)
        {
            new SubActionPlanRepository().DeleteRecord(code, recordId);
            return Json(new { Success = true });
        }

        [HttpGet]
        public ActionResult Send(long id)
        {
            var repository = new SubActionPlanRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            model.StatusId = CodeConstManager.STATUS_SEND_ID;
            model.SendDate = DateTime.Now;
            var history = new SUB_ActionHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                ActionId = model.Id,
                StatusId = CodeConstManager.STATUS_SEND_ID,
                UserId = MyExtensions.GetCurrentUserId()
            };
            model.SUB_ActionHistory.Add(history);
            new SubActionPlanRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        public ActionResult DoOperation(long id)
        {
            var repository = new SubActionPlanRepository();
            var model = repository.GetPreamble(id);

            bool IsSuccess = true;
            string preambleXml = string.Empty;
            try
            {
                preambleXml = SerializeHelper.SerializeDataContract<SUB_ActionPlan>(model);
                preambleXml = preambleXml.Replace("utf-16", "utf-8");
            }
            catch (Exception e)
            {
                IsSuccess = false;
            }

            return Json(new
            {
                IsSuccess,
                preambleXml
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SignForm(long preambleId, string xmlAuditForm)
        {
            var isSuccess = true;
            var repository = new SubActionPlanRepository();
            var model = repository.GetById(preambleId);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            model.StatusId = CodeConstManager.STATUS_SEND_ID;
            model.SendDate = DateTime.Now;
            model.DesignNote = null;
            var history = new SUB_ActionHistory()
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                ActionId = model.Id,
                XmlSign = xmlAuditForm,
                IsSign = true,
                StatusId = CodeConstManager.STATUS_SEND_ID,
                UserId = MyExtensions.GetCurrentUserId(),
                Note = "Отчет предоставлен. Дата отправки:" + DateTime.Now
            };
            //            model.SUB_FormHistory.Add(history);
            new SubActionPlanRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SubActionPlanRepository().SaveHistory(history);
            return Json(new
            {
                isSuccess,
            }, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            var repository = new SubActionPlanRepository();
            var model = repository.GetById(id);
            model.AttachFiles = new List<string>();


            FillViewBag(model);
            return View("Create", model);
        }

        [HttpGet]
        public ActionResult ShowComment(long modelId, string nameTable, int colIndex, int rowIndex)
        {
            var repository = new SubActionPlanRepository();
            SUB_ActionComment model = repository.GetComments(modelId, nameTable, colIndex, rowIndex);
           
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult SaveComment(long modelId, string nameTable, int colIndex, int rowIndex, bool isError, string comment, long rowId, string fieldName, string fieldValue)
        {

            new SubActionPlanRepository().SaveComment(modelId, nameTable, colIndex, rowIndex, isError, comment, rowId, fieldName, fieldValue, MyExtensions.GetCurrentUserId());

            return Json(new { Success = true });

        }

        [HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult ShowDetails(long id)
        {
            var repository = new SubActionPlanRepository();
            var model = repository.GetById(id);
            model.AttachFiles = new List<string>();


            FillViewBag(model);
            return View(model);
        }
      
    }
}

