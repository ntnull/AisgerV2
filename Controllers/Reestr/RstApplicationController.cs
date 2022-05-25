using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using Excel;

namespace Aisger.Controllers.Reestr
{
    public class RstApplicationController : ACommonController
    {
        [GerNavigateLogger]
        public ActionResult Index(int? id)
        {
            return View(new RstApplicationRepository().GetListReestr(id));
        }

        public ActionResult Delete(long id)
        {
            new RstApplicationRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        [GerNavigateLogger]
        public ActionResult Create(int year)
        {
            var model = new RST_Application
            {
                ReportYear = year,
                UserId = MyExtensions.GetCurrentUserId(),
               AttachFiles = new List<string>(),
                RstReestrs = new List<RST_Reestr>()
            };
            FillViewBag(model);
            return View(model);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            var repository = new RstApplicationRepository();
            var model = repository.GetById(id);
            var dir = Server.MapPath("~/uploads/application/" + id + "/");
            model.AttachFiles = new List<string>();
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    var fullname = file.Split('\\');
                    string name = fullname.Length > 0 ? fullname[fullname.Length - 1] : file;

                    model.AttachFiles.Add(name);
                }
            }
            if (model.AttachFiles == null)
            {
                model.AttachFiles = new List<string>();
            }
            model.RstReestrs = new List<RST_Reestr>();
            foreach (var activity in model.RST_Reestr)
            {
                model.RstReestrs.Add(activity);
            }

            if (model.RstReestrs.Count == 0)
            {
                model.RstReestrs.Add(new RST_Reestr());
            }
            FillViewBag(model);
            return View("Create", model);
        }
        
		[HttpPost]
		public ActionResult Create(RST_Application model, IEnumerable<HttpPostedFileBase> files)
		{
			var postedFileBases = files as HttpPostedFileBase[] ?? files.ToArray();
			if (postedFileBases.Length == 1 && postedFileBases[0] != null && postedFileBases[0].FileName.Contains(".xls"))
			{
				var fileImport = postedFileBases[0];
				if (fileImport != null && fileImport.ContentLength > 0)
				{
					Stream stream = fileImport.InputStream;
					IExcelDataReader reader = null;

					if (fileImport.FileName.EndsWith(".xls"))
					{
						reader = ExcelReaderFactory.CreateBinaryReader(stream);
					}
					else if (fileImport.FileName.EndsWith(".xlsx"))
					{
						reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
					}
					else
					{
						ModelState.AddModelError("File", "Не корректный формат");
						return View();
					}
					ModelState.Remove("RstReestrs[0].IDK");
					ModelState.Remove("RstReestrs[0].BINIIN");
					ModelState.Remove("RstReestrs[0].OwnerName");
					ModelState.Remove("RstReestrs[0].Address");

					reader.IsFirstRowAsColumnNames = true;
					var manager = new ExcelFileManager();
					model.RstReestrs = manager.ReadExcel(reader.AsDataSet(), model.ReportYear);
					reader.Close();
					FillViewBag(model);
					return View(model);
				}
			}
			if (ModelState.IsValid)
			{
				
				if (files == null)
				{
					files = new List<HttpPostedFileBase>();
				}
				var httpPostedFileBases = files as HttpPostedFileBase[] ?? postedFileBases.ToArray();
				var name =
					(from file in httpPostedFileBases
					 where file != null && file.ContentLength > 0
					 select file.FileName).FirstOrDefault();
				if (name != null)
				{
					var clientfile = name.Split('\\');
					name = clientfile.Length > 0 ? clientfile[clientfile.Length - 1] : name;
				}
				new RstApplicationRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
				string errorMessage = new RstReestrRepository().SaveRstReestr(model.RstReestrs[0], MyExtensions.GetCurrentUserId());
				//---- add rst_reestr 


				var dirpath = Server.MapPath("~/uploads/application/" + model.Id);
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

				return RedirectToAction("CommonView", "RstReport");
			}
			if (model.AttachFiles == null)
			{
				model.AttachFiles = new List<string>();
			}

			FillViewBag(model);
			return View("Create", model);
		}

        [HttpGet]
        public ActionResult DownLoadTempl()
        {
            var dir = Server.MapPath("~/Template/reestrTempl.xls");
            var fi = new FileInfo(dir);
            return File(fi.FullName, GetContentType(fi.Name), fi.Name);

        }
        [HttpGet]
        public ActionResult ShowFile(string id, string filename)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            string namefile = null;

            var dir = Server.MapPath("~/uploads/application/" + id);
            if (!Directory.Exists(dir))
            {
                return RedirectToAction("Index");
            }

            var files = Directory.GetFiles(dir);
            if (files.Length == 0)
            {
                return RedirectToAction("Index");
            }
          /*  foreach (var file in files)
            {
                split = file.Split('\\') let name = split.Length > 0 ? split[split.Length - 1]
                if (file == filename)
                {
                    
                }
            }*/
            string first = null;
            foreach (string file in files)
            {
                string[] split = file.Split('\\');
                string name = split.Length > 0 ? split[split.Length - 1] : file;
                if (filename == name)
                {
                    first = file;
                    break;
                }
            }
            var fullname = first ??
                              files[0];
            var fi = new FileInfo(fullname);
            return File(fi.FullName, GetContentType(fi.Name), fi.Name);

        }
        private void FillViewBag(RST_Application model)
        {
            if (model.RstReestrs == null || model.RstReestrs.Count == 0)
            {
                model.RstReestrs = new List<RST_Reestr>();
                var waste = new RST_Reestr();
                model.RstReestrs.Add(waste);
            }
            else
            {
				var check = new RstReestrRepository().GetReestrByBin(model.RstReestrs[0].BINIIN, model.ReportYear);
				model.RstReestrs[0].StatusReestr = check.StatusReestr; //new RstReestrRepository().GetStatusByBin(model.RstReestrs[0].BINIIN);
            }
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            ViewData["OblastList"] = new SelectList(listanimal, "Id",
                                                 "NameRu", model.Oblast);
        }
        public ActionResult ShowInfoView(string oldname, string newname)
        {
            var model = new BaseDictionary();
            model.NameRu = oldname;
            model.NameKz = newname;
            return View(model);
        }
		[HttpPost]
		public virtual ActionResult GetInfoSubject(string bin, int? year)
		{
			if (year == null)
			{
				return null;
			}

			var check = new RstReestrRepository().GetReestrByBin(bin, year.Value);
			int checkUser = new SecUserRepository().CheckUserByBiniin(bin);
			return Json(new { ownerName = check.OwnerName, adress = check.Address, code = check.StatusReestr, checkUser = checkUser });
		}
    }
}