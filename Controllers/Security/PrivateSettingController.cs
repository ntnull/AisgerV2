using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using Aisger.Models.Repository.Reestr;
using Aisger.Models.Repository.Subject;

namespace Aisger.Controllers.Security
{
    public class PrivateSettingController : ACommonController
    {
        public ActionResult Map()
        {
            var user = new SecUserRepository().GetById(MyExtensions.GetCurrentUserId().Value);
            return View(user);
        }
        public ActionResult ShowDialogMap(long id)
        {
            var user = new SecUserRepository().GetById(id);
            return View(user);
        }
        [HttpPost]
        public virtual ActionResult UpdateMark(long userId, string lat, string lng)
        {
            new SecUserRepository().UpdateMark(userId, lat, lng, MyExtensions.GetCurrentUserId());
            return Json(new { Success = true });
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var user = new SecUserRepository().GetById(MyExtensions.GetCurrentUserId().Value);
            if (user.IsGuest)
            {
                var model = GetGestInfo(user);
                FillGuestViewBag(model);
                return View("GuestView", model);
            }
            return View("EmplView", user);
        }

        public SEC_Guest GetGestInfo(SEC_User user)
        {
            var model = new GuestController().SecGuest(user);
            return model;
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult GuestEditView(int year, string regUrl = "")
        {
            var user = new SecUserRepository().GetById(MyExtensions.GetCurrentUserId().Value);
            var model = new GuestController().SecGuest(user);

            //----check all fields is filled
            ViewBag.IsFilled = true;
            ViewBag.Year = year;
            ViewBag.regUrl = regUrl;
            FillGuestViewBag(model);
            return View(model);
        }

        private void FillGuestViewBag(SEC_Guest model)
        {

            model.WastList = new AccountController().GetPlants(model.Wastes);
            model.KindList = new AccountController().GetKinds(model.Kinds);
            ViewData["TypeApplicationList"] = new SelectList(new DicTypeApplicationRepository().GetAll().Where(x => x.IsDeleted == false), "Id", "NameRu", model.TypeApplicationId);
            ViewData["OKEDList"] = new SelectList(new DicOkedRepository().GetAll(), "Id", "FullName", model.OkedId);
            var repository = new KatoRepository();
            var listanimal = repository.GetKatos(1, true);
            ViewData["OblastList"] = new SelectList(listanimal, "Id",
                                                 "NameRu", model.Oblast);

            ViewData["RegionList"] = new SelectList(repository.GetKatos(model.Oblast, true), "Id",
                                                   "NameRu", model.Region);

            ViewData["SubRegionList"] = new SelectList(repository.GetKatos(model.Region, false), "Id",
                                                   "NameRu", model.SubRegion);

            ViewData["VillageList"] = new SelectList(repository.GetKatos(model.SubRegion, false), "Id",
                                                   "NameRu", model.Village);

            var list = new List<FsCode>();
            list.Add(new FsCode() { Id = null, NameRu = "" });
            list.Add(new FsCode() { Id = 1, NameRu = "юр" });
            list.Add(new FsCode() { Id = 2, NameRu = "кв" });
            list.Add(new FsCode() { Id = 3, NameRu = "гу" });
            list.Add(new FsCode() { Id = 4, NameRu = "ип" });
            ViewData["FsCodeList"] = new SelectList(list, "Id", "NameRu", model.FSCode);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult EmplEditView()
        {
            var user = new SecUserRepository().GetById(MyExtensions.GetCurrentUserId().Value);
            return View(user);
        }

        [HttpPost]
        public ActionResult EmplEditView(SEC_User model)
        {
            ModelState.Remove("Pwd");
            ModelState.Remove("ConfirmPwd");
            ModelState.Remove("RolesId");
            ModelState.Remove("DIC_Organization.NameRu");
            ModelState.Remove("DIC_Organization.NameKz");
            ModelState.Remove("DIC_Organization.Address");
            ModelState.Remove("SEC_Roles.NameKz");
            ModelState.Remove("SEC_Roles.NameRu");
            if (ModelState.IsValid)
            {
                new SecUserRepository().UpdateEmployee(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult GuestEditView(SEC_Guest model,int year, string regUrl = "")
        {
            ModelState.Remove("Pwd");
            ModelState.Remove("ConfirmPwd");
            FillGuestViewBag(model);
            ViewBag.Year = year;
            ViewBag.regUrl = regUrl;

            if (model.Region == 0)
            {
                ModelState.AddModelError("JuridicalKato", ResourceSetting.RegionNotEmpty);
            }

            if (string.IsNullOrEmpty(model.Post))
            {
                ModelState.AddModelError("Post", ResourceSetting.NotEmpty);
            }

            if (ModelState.IsValid)
            {
                //----
                ViewBag.IsFilled = true;

                var repository = new SecUserRepository();
                if (model.Village == 0)
                {
                    model.Village = null;
                }
                if (model.SubRegion == 0)
                {
                    model.SubRegion = null;
                }

                repository.RegisteredUser(model, MyExtensions.GetCurrentUserId());
                //----copy user info to rst_reportreestr
                new RstReportRepository().UpdateReportReestr(model, year);
                return RedirectToAction("Create", "RegisterForm");
            }

            //----
            ViewBag.IsFilled = false;

            return View(model);
        }


    }
}
