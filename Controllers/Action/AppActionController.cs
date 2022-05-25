using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Helpers;
using Aisger.Models;
using Aisger.Models.Repository.Action;
using Aisger.Models.Repository.Dictionary;
using Aisger.Utils;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Action
{
    public class AppActionController : AActionController
    {
        //
        // GET: /AppForm/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var oblasts = new List<string>();
            foreach (var code in CodeConstManager.OBLAST_CODES)
            {
                if (MyExtensions.CheckRight(code))
                {
                    oblasts.Add(code);
                }
            }
            if (oblasts.Count > 0)
            {
                var list =
                    new SubActionPlanRepository().GetListByOblast(oblasts);
                return View(list);
            }
            var currentList = new List<SUB_ActionPlan>();
            return View(currentList);
        }

        public ActionResult ShowHistory(long actionId)
        {
            var list = new SubActionPlanRepository().GetReestrReportByUserId(actionId);
            if (list == null)
            {
                list = new List<SUB_ActionHistory>();
            }
            return View(list);
        }
        public ActionResult SendApplication(long id)
        {
            var repository = new SubActionPlanRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return Json(new { Success = false });
            }
            model.IsBack = true;
            model.DesignDate = DateTime.Now;
            var history = new SUB_ActionHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                StatusId = model.StatusId,
                UserId = MyExtensions.GetCurrentUserId()
            };
            model.SUB_ActionHistory.Add(history);
            new SubActionPlanRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            new SendMessageManager().SendSubActionPlan(model);
            return Json(new { Success = true });
        }

        [HttpGet]
        public ActionResult BackSend(long id)
        {
            var repository = new SubActionPlanRepository();
            var model = repository.GetById(id);
            if (model == null)
            {
                return RedirectToAction("Index");
            }
            model.IsBack = true;
            model.DesignDate = DateTime.Now;
            var history = new SUB_ActionHistory
            {
                CreateDate = DateTime.Now,
                RegDate = DateTime.Now,
                ActionId = model.Id,
                StatusId = model.StatusId,
                UserId = MyExtensions.GetCurrentUserId()
            };
            model.SUB_ActionHistory.Add(history);
            new SubActionPlanRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }
      
        [HttpGet]
        public ActionResult Design(long id)
        {
            ViewBag.SubReadonly = true;

            var repository = new SubActionPlanRepository();
            var model = repository.GetById(id);
            model.AttachFiles = new List<string>();
            model.DesignDate = DateTime.Now;
            FillViewBag(model);
            FillHistory(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Design(SUB_ActionPlan model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (files == null)
                {
                    files = new List<HttpPostedFileBase>();
                }
                var httpPostedFileBases = files as HttpPostedFileBase[] ?? files.ToArray();
                var name =
                    (from file in httpPostedFileBases
                     where file != null && file.ContentLength > 0
                     select file.FileName).FirstOrDefault();
                if (name != null)
                {
                    var clientfile = name.Split('\\');
                    name = clientfile.Length > 0 ? clientfile[clientfile.Length - 1] : name;
                }

                var history = new SUB_ActionHistory
                {
                    ActionId = model.Id,
                    StatusId = model.StatusId,
                    UserId = MyExtensions.GetCurrentUserId(),
                    RegDate = DateTime.Now
                };
                new SubActionPlanRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                new SubActionPlanRepository().SaveHistory(history);

                var dirpath = Server.MapPath("~/uploads/actionhsitory/" + history.Id);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
                var oldfiles = Directory.GetFiles(dirpath);

                foreach (var file in oldfiles)
                {
                    var fullname = file.Split('\\');

                    name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    var exist = model.AttachFiles.Any(existfile => name == existfile);
                    if (exist) continue;
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }

                foreach (var file in httpPostedFileBases)
                {
                    if (file == null || file.ContentLength <= 0) continue;
                    var uploadFileName = Path.GetFileName(file.FileName);
                    if (uploadFileName == null) continue;
                    var uploadFilePathAndName = Path.Combine(dirpath, uploadFileName);
                    ImageUtility.WriteFileFromStream(file.InputStream, uploadFilePathAndName);
                }

                return RedirectToAction("CommonView");
            }
            FillHistory(model);
            ViewBag.SubReadonly = true;
            FillViewBag(model);
            return View(model);

        }

        public ActionResult CommonView(int? year, string idk, string biniin, string adress, string owner, string status, string oblast,   long? sendId)
        {
            var filter = new SubActionCommonFilter();
            filter.ReportYear = year;
            filter.SendId = sendId;
            if (!string.IsNullOrEmpty(biniin))
            {
                filter.BINIIN = biniin;
            }
            if (!string.IsNullOrEmpty(idk))
            {
                filter.IDK = idk;
            }
            if (!string.IsNullOrEmpty(adress))
            {
                filter.Adress = adress;
            }
            if (!string.IsNullOrEmpty(owner))
            {
                filter.SubjectName = owner;
            }
            if (!string.IsNullOrEmpty(status))
            {
                filter.Statuses = new List<string>();
                var statuslList = status.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Statuses.Add(s);
                }
            }
          
            if (!string.IsNullOrEmpty(oblast))
            {
                filter.Oblasts = new List<string>();
                var statuslList = oblast.Split(',');
                foreach (var s in statuslList)
                {
                    filter.Oblasts.Add(s);
                }
            }
          
            FillCommonViewBag(filter);
            filter.SubActionPlanFilters = new SubActionPlanRepository().GetCommonReestrsByFilter(filter);

            // FillViewFiltertBag(filter);
            return View(filter);
        }
        public MultiSelectList GetStatus(IList<string> selectedValues)
        {
            var plants = new SubDicStatusRepository().GetAll().Where(e => (e.Id != CodeConstManager.REG_STATUS_REESTR_ID));
            return new MultiSelectList(plants, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }
        public MultiSelectList GetOblasList(IList<string> selectedValues)
        {
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            return new MultiSelectList(listanimal, "Id", CultureHelper.GetDictionaryName("NameRu"), selectedValues);
        }

        private void FillCommonViewBag(SubActionCommonFilter filter)
        {
            var repository = new SubActionPlanRepository();

            filter.StatusList = GetStatus(filter.Statuses);
            filter.OblastList = GetOblasList(filter.Oblasts);
            var listanimal = repository.GetYears();
            if (filter.ReportYear == null)
            {
                filter.ReportYear = (int?)listanimal.Max(e => e.ID);
            }
            ViewData["Years"] = new SelectList(listanimal, "ID",
                                                 "NAME_RU", filter.ReportYear);

        }

    }
}
