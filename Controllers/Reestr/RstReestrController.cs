using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Reestr;
using Aisger.Utils;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Reestr
{
    public class RstReestrController : ACommonController
    {
        //
        // GET: /RstReestr/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View(new RstReestrRepository().GetCollectionList().Where(e=>e.StatusId==CodeConstManager.REG_STATUS_REESTR_ID));
        }
        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Exluded(long id)
        {
            var repository = new RstReestrRepository();
            var model = repository.GetById(id);
            model.EditDate = DateTime.Now;
            if (model.RST_Application != null)
            {
                model.RST_Application.AttachFiles = new List<string>();
                var dir = Server.MapPath("~/uploads/application/" + model.RST_Application.Id + "/");
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir);
                    foreach (var file in files)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        model.RST_Application.AttachFiles.Add(name);
                    }
                }
                if (model.RST_Application.AttachFiles == null)
                {
                    model.RST_Application.AttachFiles = new List<string>();
                }
            }
            FillViewBags(model);
            return View(model);
        }

        private void FillViewBags(RST_Reestr model)
        {
            var listanimal = new RstDicReasonRepository().GetAll().Where(e => (e.IsExcluded));
            ViewData["reasonList"] = new SelectList(listanimal, "Id",
                                                 "NameRu", model.ReasonId);
        }

        [HttpGet]
        public ActionResult ShowFile(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            var list = id.Split('#');
            string path = list[0];
            string filename = null;
            if (list.Count() > 1)
            {
                filename = list[1];
            }
            var dir = Server.MapPath("~/uploads/reestrhistory/" + path);
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
        public ActionResult Exluded(RST_Reestr model, IEnumerable<HttpPostedFileBase> files)
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
                model.StatusId = CodeConstManager.EXTEND_STATUS_REESTR_ID;

                 var history = new RST_ReestrHistory
                    {
                        ReestrId = model.Id,
                        StatusId = CodeConstManager.EXTEND_STATUS_REESTR_ID,
                        UserId = MyExtensions.GetCurrentUserId(),
                        RegDate = DateTime.Now
                    };
                new RstReestrRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                new RstReestrRepository().SaveHistory(history);

                var dirpath = Server.MapPath("~/uploads/reestrhistory/" + history.Id);
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

                return RedirectToAction("Index");
            }
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            FillViewBags(model);
            return View(model);

        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult ShowDetails(long id)
        {
            var repository = new RstReestrRepository();
            var model = repository.GetById(id);

            var dir = Server.MapPath("~/uploads/application/" + model.RST_Application.Id + "/");
            model.RST_Application.AttachFiles = new List<string>();
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');
                    string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    model.RST_Application.AttachFiles.Add(name);
                }
            }
            foreach (var rstReestrHistory in model.RST_ReestrHistory)
            {
                var dir1 = Server.MapPath("~/uploads/reestrhistory/" + rstReestrHistory.Id + "/");
                if (Directory.Exists(dir1))
                {
                    var files = Directory.GetFiles(dir1);
                    rstReestrHistory.AttachFiles = new List<string>();
                    foreach (var file in files)
                    {
                        var fullname = file.Split('\\');
                        string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                        rstReestrHistory.AttachFiles.Add(name);
                    }
                }
            }

            return View(model);
        }
    }
}
