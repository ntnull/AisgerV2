using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Utils;
using NPOI.SS.Formula.Functions;

namespace Aisger.Controllers.Esco
{
    public class EscoDicProductKindController : ACommonController
    {
        //
        // GET: /EscoDicProductKind/

        public ActionResult Index()
        {
            var list = new EscoDicProductKindRepository().GetList(MyExtensions.GetCurrentUserId());
            return View(list);
        }
        public ActionResult Delete(long id)
        {
            new EscoDicProductKindRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Create()
        {
            var model = new ESCO_DIC_ProductKind { UserId = MyExtensions.GetCurrentUserId()};
//            model.ESCO_DIC_Product.Add(new ESCO_DIC_Product());
            model.Wastes =new List<string>();
            FillViewBag(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var repository = new EscoDicProductKindRepository();
            var model = repository.GetById(id);
            model.Wastes = model.ESCO_DIC_ProductKindOked.Select(aquticOblast => aquticOblast.OkedId.ToString()).ToList();
            FillViewBag(model);
            foreach (var product in model.ESCO_DIC_Product)
            {
                var path = Server.MapPath("~/uploads/products/" + product.Id);
                product.AttachFiles = new List<string>();
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);
                    foreach (string file in files)
                    {
                        var fi = new FileInfo(file);
                        var filename = "../../uploads/products/" + product.Id + "/" + fi.Name;
                        product.CurrentUrlImage = filename;
                        product.AttachFiles.Add(filename);
                    }
                }
            }
            return View("Create", model);
        }

        [HttpGet]
        public ActionResult EditProduct(long modelid, long kindId)
        {
            var repository = new EscoDicProductKindRepository();
            var model = repository.GetProductById(modelid);
            if (model == null)
            {
                model = new ESCO_DIC_Product();
                model.KindId = model.KindId;
                model.ESCO_DIC_ProductKind = repository.GetById(kindId);
                model.AttachFiles = new List<string>();
            }
            else
            {
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
            }
            return View(model);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditProduct(ESCO_DIC_Product model, HttpPostedFileBase image)
        {
            var repository = new EscoDicProductKindRepository();
            if (ModelState.IsValid)
            {
                repository.UpdatePostProduct(model);
                if (image != null)
                {

                    if (model.KindId != null) model.ESCO_DIC_ProductKind = repository.GetById(model.KindId.Value);
                    var dirpath = Server.MapPath("~/uploads/products/" + model.Id);
                    if (!Directory.Exists(dirpath))
                    {
                        Directory.CreateDirectory(dirpath);
                    }

                    model.AttachFiles = new List<string>();
                    var files = Directory.GetFiles(dirpath);
                    var _ext = Path.GetExtension(image.FileName);
                    var _imgname = files.Count() + _ext;
                    var uploadFilePathAndName = Path.Combine(dirpath, _imgname);
                    ImageUtility.WriteFileFromStream(image.InputStream, uploadFilePathAndName);

                    foreach (string file in files)
                    {
                        var fi = new FileInfo(file);
                        var filename = "../../uploads/products/" + model.Id + "/" + fi.Name;
                        
                        model.AttachFiles.Add(filename);
                    }
                    model.CurrentUrlImage = "../../uploads/products/" + model.Id + "/" + _imgname;
                    model.AttachFiles.Add(model.CurrentUrlImage);
                    return RedirectToAction("EditProduct", new { modelid = model.Id, kindId = model.KindId });
                    return View(model);
                }
                return RedirectToAction("Edit", new { id = model.KindId });
            }
            return RedirectToAction("EditProduct", new { modelid = model.Id, kindId = model.KindId });

        }

       
        [HttpPost]
        public virtual ActionResult DeleteProduct(long id)
        {
            new EscoDicProductKindRepository().DeleteProduct(id);
            return Json(new { Success = true});
        }
        [HttpPost]
        public virtual ActionResult UpdateProductKind(long modelId, long userd, string fieldName, string fieldValue)
        {
            var filter = new EscoDicProductKindRepository().UpdateProductKind(modelId, userd, fieldName, fieldValue);
            return Json(new { Success = true, formId = filter.ModelId, fromRecordId = filter.RecordId, unique = filter.Unique });
        }
        [HttpPost]
        public virtual ActionResult UpdateRecord(long modelId, long recordId, string fieldName, string fieldValue)
        {
            var filter = new EscoDicProductKindRepository().UpdateRecord(modelId, recordId, fieldName, fieldValue);
            return Json(new { Success = true, formId = filter.ModelId, fromRecordId = filter.RecordId, unique = filter.Unique });
        }
        private void FillViewBag(ESCO_DIC_ProductKind model)
        {
            model.WastList = GetPlants(model.Wastes);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile()
        {
            string _imgname = string.Empty;
            if (System.Web.HttpContext.Current.Request.Files.AllKeys.Any())
            {
                var pic = System.Web.HttpContext.Current.Request.Files["MyImages"];
                var rowindex = System.Web.HttpContext.Current.Request.Params["recordId"];
                if (pic.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(pic.FileName);
                    var _ext = Path.GetExtension(pic.FileName);

                    _imgname = Guid.NewGuid().ToString() + _ext; 
                    var dirpath = Server.MapPath("~/uploads/products/" + rowindex);
                    if (!Directory.Exists(dirpath))
                    {
                        Directory.CreateDirectory(dirpath);
                    }
                    var _comPath = dirpath +"/"+ _imgname;
                    _imgname = rowindex + "/" + _imgname;
//                    _imgname = "MVC_" + _imgname + _ext;

                    ViewBag.Msg = _comPath;
                    var path = _comPath;

                    // Saving Image in Original Mode
                    pic.SaveAs(path);

                    // resizing image
                    MemoryStream ms = new MemoryStream();
                    WebImage img = new WebImage(_comPath);

                    if (img.Width > 200)
                        img.Resize(200, 200);
                    img.Save(_comPath);
                    // end resize
                }
            }
            return Json(Convert.ToString(_imgname), JsonRequestBehavior.AllowGet);
        }

        public MultiSelectList GetPlants(IList<string> selectedValues)
        {
            var plants = new DicOkedRepository().GetList().Where(e=>e.refParent==null);
            return new MultiSelectList(plants, "Id", "NameRu", selectedValues);
        }

    }
}
