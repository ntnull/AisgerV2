using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Map;
using Aisger.Models.Repository.Security;
using Aisger.Utils;

namespace Aisger.Controllers.Map
{
    public class MapProjectController : ACommonController
    {
        //
        // GET: /MapProject/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View(new MapProjectRepository().GetCollectionList());
        }
        public ActionResult Delete(long id)
        {
            new MapProjectRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [GerNavigateLogger]
        public ActionResult Create()
        {
            var currentUserId = MyExtensions.GetCurrentUserId();
            if (currentUserId == null) return View();
            var model = new MAP_Project {RegDate = DateTime.Now};
            FillViewBag(model);
            return View(model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            var repository = new MapProjectRepository();
            var model = repository.GetById(id);
            FillViewBag(model);
            return View("Create", model);
        }
        [HttpPost]
        public virtual ActionResult GetInfoApp(long id)
        {

            var model =
                new MapApplicationRepository().GetById(id);
            var escoName = "";
            if (model.SEC_User1 != null)
            {
                escoName = model.SEC_User1.ApplicationName;
            }
            return Json(new { model.TotalCost, RecipientName = escoName });

        }
        private void FillViewBag(MAP_Project model)
        {
           ViewData["EscoList"] = new SelectList(new MapProjectRepository().GetEscoList(), "Id", "ApplicationName",model.EscoId);
           ViewData["AppList"] = new SelectList(new MapProjectRepository().GetAppList(model), "Id", "ProjectName", model.ApplicationId);
        }

        [HttpPost]
        public ActionResult Create(MAP_Project model, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                new MapProjectRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                if (model.ApplicationId != null)
                {

                    var app = new MapApplicationRepository().GetById(model.ApplicationId.Value);
                    var status =
                        new MapDicStatusRepository().GetAll().FirstOrDefault(e => e.Code == CodeConstManager.MAP_STATUS_PROJECT);
                    if (status != null)
                    {
                        app.StatusId = status.Id;
                        new MapApplicationRepository().SaveOrUpdate(app, MyExtensions.GetCurrentUserId());
                    }
                }
                return RedirectToAction("Index");
            }
            FillViewBag(model);
            return View("Create", model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult ShowDetails(long id)
        {
            var repository = new MapProjectRepository();
            var model = repository.GetById(id);
            if (model.MAP_Application == null)
            {
                model.MAP_Application = new MAP_Application();
            }
            if (model.MAP_Application.AttachFiles == null)
            {
                model.MAP_Application.AttachFiles = new List<string>();
            }
            model.MAP_Application.MapApplicationProducts = new List<MAP_ApplicationProduct>();
            model.MAP_Application.ProjectPowers = new List<MAP_ApplicationProduct>();
            model.MAP_Application.InKinds = new List<MAP_ApplicationProduct>();
            model.MAP_Application.InValueTerms = new List<MAP_ApplicationProduct>();
            foreach (var activity in model.MAP_Application.MAP_ApplicationProduct)
            {
                switch (activity.Disrciminator)
                {
                    case CodeConstManager.DISC_PRODUCT:
                        {
                            model.MAP_Application.MapApplicationProducts.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_POWER:
                        {
                            model.MAP_Application.ProjectPowers.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_KIND:
                        {
                            model.MAP_Application.InKinds.Add(activity);
                            break;
                        }
                    case CodeConstManager.DISC_IN_VALUE_TERM:
                        {
                            model.MAP_Application.InValueTerms.Add(activity);
                            break;
                        }
                }
            }
            model.MAP_Application.MapApplicationEvents = new List<MAP_ApplicationEvent>();
            foreach (var activity in model.MAP_Application.MAP_ApplicationEvent)
            {
                model.MAP_Application.MapApplicationEvents.Add(activity);
            }
            var dir = Server.MapPath("~/uploads/maps/" + model.MAP_Application.Id + "/");
            model.MAP_Application.AttachFiles = new List<string>();
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');
                    string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    model.MAP_Application.AttachFiles.Add(name);
                }
            }
            if (model.MAP_Application.MapApplicationProducts.Count == 0)
            {
                model.MAP_Application.MapApplicationProducts.Add(new MAP_ApplicationProduct());
            }
            if (model.MAP_Application.MapApplicationEvents.Count == 0)
            {
                model.MAP_Application.MapApplicationEvents.Add(new MAP_ApplicationEvent());
            }
            return View(model);
        }
    }
}
