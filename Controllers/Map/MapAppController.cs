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
using NPOI.SS.Formula.Functions;

namespace Aisger.Controllers.Map
{
    public class MapAppController : AMapController
    {
        //
        // GET: /MapApp/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var collection = new List<MAP_Application>();
            var list = new MapApplicationRepository().GetCollectionList().Where(e => e.StatusId != CodeConstManager.REG_STATUS_REESTR_ID);
            foreach (var mapApplication in list)
            {
                if (mapApplication.SEC_User1 != null)
                {
                    var userKind =
                        mapApplication.SEC_User1.SEC_UserKind.FirstOrDefault(
                            e => e.KindId == CodeConstManager.KIND_USER_MAP);
                    if (userKind != null && userKind.IsBlocked!=null)
                    {
                        mapApplication.IsBlocked = userKind.IsBlocked.Value;
                    }
                }
               
                collection.Add(mapApplication);
            }
            return View(list);
        }
        public ActionResult LoadFile(string id, string filename)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            var dir = Server.MapPath("~/uploads/mapapp/" + id);
            if (!Directory.Exists(dir))
            {
                return RedirectToAction("Index");
            }

            var files = Directory.GetFiles(dir);
            if (files.Length == 0)
            {
                return RedirectToAction("Index");
            }
            var fullname = (from file in files let split = file.Split('\\') let name = split.Length > 0 ? split[split.Length - 1] : file where filename == name select file).FirstOrDefault() ??
                              files[0];
            var fi = new FileInfo(fullname);
            return File(fi.FullName, GetContentType(fi.Name), fi.Name);

        }
        [HttpPost]
        public virtual ActionResult SendNotification(long modelId, string note)
        {
            var dir = Server.MapPath("~/uploads/mapapp/" + modelId + "/");
               var filesload = new string[] {};
               if (Directory.Exists(dir))
               {
                    filesload = Directory.GetFiles(dir);
               }

               new SendMessageManager().SendMapDesign(modelId,note, filesload);
            
            
            return Json(new { Success = true });

        }
        public ActionResult GetFileUploader(long preambleId)
        {
            var auditRepository = new MapApplicationRepository();
            var model = auditRepository.GetById(preambleId);
            if (model == null)
            {
                return null;
            }
            if (model.DesignAttachFiles == null)
            {
                model.DesignAttachFiles = new List<string>();
            }

            var dir = Server.MapPath("~/uploads/mapapp/" + model.Id + "/");
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');
                    string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    model.DesignAttachFiles.Add(name);
                }
            }
            return PartialView("_UploadFilesView", model);
        }
        [HttpPost]
        public ActionResult FileUpload(long id, IEnumerable<HttpPostedFileBase> files)
        {
            string path = Server.MapPath("~/uploads/mapapp/" + id + "/");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var repository = new MapApplicationRepository();
            var preamble = repository.GetById(id);


            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(Path.Combine(path, file.FileName));
                }
            }
            repository.Update(preamble);
            var eauditModel = repository.GetById(id);
            return RedirectToAction("Design", "MapApp", new { id = id });
        }
        public JsonResult FileRemove(long id, string filename)
        {
            string path = Server.MapPath("~/uploads/mapapp/" + id + "/");
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');

                    var name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    var exist = name == filename;
                    if (!exist) continue;
                    if (System.IO.File.Exists(file))
                    {
                        System.IO.File.Delete(file);
                    }
                }


            }

            return Json(new
            {
                IsSuccess = true,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
