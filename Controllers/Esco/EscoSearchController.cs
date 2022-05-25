using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Entity.Esco;
using Aisger.Models.Repository.Dictionary;

namespace Aisger.Controllers.Esco
{
    public class EscoSearchController : ACommonController
    {
        //
        // GET: /EscoSearch/

        public ActionResult Index(string term)
        {

            var model = new EscoEntityDocument { Biniin = term };
            var repository = new EscoDicProductKindRepository();
            model.CurrentPage = 0;
            model.PageSize = 20;
            GetListformService(model, 0, 20);
            
            model.AllCount = repository.GetAllCont(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(EscoEntityDocument model)
        {
            model.CurrentPage = 0;
            model.PageSize = 20;
            GetListformService(model, 0, 20);
            model.AllCount = new EscoDicProductKindRepository().GetAllCont(model);
            return View(model);
        }
        [ChildActionOnly]
        public ActionResult ResultListView(EscoEntityDocument model)
        {
            model.FilesId = "";
            foreach (var escoDicProduct in model.EscoDicProducts)
            {
                escoDicProduct.AttachFiles = new List<string>();
                var path = Server.MapPath("~/uploads/products/" + escoDicProduct.Id);
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        var fi = new FileInfo(file);
                        var filename = "../../uploads/products/" + escoDicProduct.Id + "/" + fi.Name;
                        escoDicProduct.CurrentUrlImage = filename;
                        escoDicProduct.AttachFiles.Add(filename);
                    }
                    if (!string.IsNullOrEmpty(model.FilesId))
                    {
                        model.FilesId = model.FilesId + "&";
                    }
                    model.FilesId = model.FilesId + escoDicProduct.Id;
                }
            }
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult InfinateScroll(EscoEntityDocument model)
        {

            GetListformService(model, model.CurrentPage, 20);
            JsonModel jsonModel = new JsonModel();

            jsonModel.NoMoreData = model.EscoDicProducts.Count() < 20;
            jsonModel.HTMLString = RenderPartialViewToString("ResultListView", model);

            return Json(jsonModel);
        }
        private  void GetListformService(EscoEntityDocument model, int start, int size)
        {
            var repository = new EscoDicProductKindRepository();
            var list = repository.GetReuslt(model, start, size);
            model.FilesId = "";
            var escoDicProducts = list as EscoDicProduct[] ?? list.ToArray();
            foreach (var escoDicProduct in escoDicProducts)
            {
                escoDicProduct.AttachFiles = new List<string>();
                var path = Server.MapPath("~/uploads/products/" + escoDicProduct.Id);
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        var fi = new FileInfo(file);
                        var filename = "../../uploads/products/" + escoDicProduct.Id + "/" + fi.Name;
                        escoDicProduct.CurrentUrlImage = filename;
                        escoDicProduct.AttachFiles.Add(filename);
                    }
                    if (!string.IsNullOrEmpty(model.FilesId))
                    {
                        model.FilesId = model.FilesId + "#";
                    }
                    model.FilesId = model.FilesId + escoDicProduct.Id;
                }
            }
            ViewBag.FilesId = model.FilesId;
            model.EscoDicProducts = escoDicProducts;

        }
        [HttpGet]
        public ActionResult ShowDetails(long id)
        {
            var repository = new EscoDicProductKindRepository();
            var model = repository.GetProductById(id);
          
                var path = Server.MapPath("~/uploads/products/" + model.Id);
                model.AttachFiles = new List<string>();
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        var fi = new FileInfo(file);
                        var filename = "../../uploads/products/" + model.Id + "/" + fi.Name;
                        model.CurrentUrlImage = filename;
                        model.AttachFiles.Add(filename);
                    }
                }
            return View(model);
        }
       
    }
}
