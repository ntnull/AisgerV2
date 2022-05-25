using System.Linq;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using FlowDoc.Models.Repository.Dictionary;
using Aisger.Helpers;
using System.Collections.Generic;
using Aisger.Models.Repository;
using System;

namespace Aisger.Controllers.Security
{
    public class UsersController : ACommonController
    {
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var repository = new AccountRepository();
			var model = repository.GetAll().Where(e => !e.IsGuest);
            return View(model);
        }

        private void FillBagView(SEC_User user)
        {
            ViewBag.OrganizationId = new SelectList(new DicOrganisationRepository().GetAll(), "Id", "NameRu",user.OrganizationId);
            ViewBag.RolesId = new SelectList(new SecRolesRepository().GetAll(), "Id", "NameRu", user.RolesId);
            ViewBag.DeparmentId = new SelectList(new DicDepartmentRepository().GetAll(), "Id", "NameRu", user.DeparmentId);
        }

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult Create()
        {
            var environmental = new SEC_User();
            environmental.TypeApplicationId = CodeConstManager.APP_TYPE_PHYS_PERSON;
            FillBagView(environmental);
            return View(environmental);
        }

        [HttpPost]
        public ActionResult Create(SEC_User model)
        {
            FillBagView(model);
            var user = new SecUserRepository().GetAll().SingleOrDefault(e => e.Login == model.Login && e.Id != model.Id);
            if (user != null && model.Id != user.Id)
            {
                model.IsError = true;
                model.ErrorMessage = "С данным Логином пользователь уже зарегистрирован, обратитесь к администратору";
                return View(model);
            }

            if (model.Id==0 && model.Pwd != model.ConfirmPwd)
            {
                model.IsConfirm = true;
                return View(model);
            }
             var repository = new SecUserRepository();
            if (model.Id > 0)
            {
                ModelState.Remove("Pwd");
                ModelState.Remove("ConfirmPwd");
                model.Pwd = repository.GetById(model.Id).Pwd;
                model.ConfirmPwd = model.Pwd;
            }
            else
            {
                model.Pwd = RegJurnalManager.Instance.Encrypt(model.Pwd);
            }
            if (ModelState.IsValid)
            {
               
                model.IsGuest = false;
                model.TypeApplicationId = CodeConstManager.APP_TYPE_PHYS_PERSON;
                repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult Delete(long id)
        {
            new SecUserRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }
        
		[HttpGet]
        [GerNavigateLogger]
        public ActionResult Edit(long id)
        {
            SEC_User environmental = new SecUserRepository().GetById(id);
            FillBagView(environmental);
            return View("Create", environmental);
        }

		[HttpGet]
        [GerNavigateLogger]
        public ActionResult EditByAdmin(long id) {
			SEC_User model = new SecUserRepository().GetById(id);
			FillBagView(model);

			var repository = new KatoRepository();

		    List<DicObjectClass> oblast=new List<DicObjectClass>();
			var lang = CultureHelper.GetCurrentCulture();
			string ErrorMessage = repository.GetKatoByCuture(ref oblast,1,lang);
			if (model.is_obl_control==true && model.rst_obl_control != null && model.rst_obl_control.Count != 0)
			{
				foreach (var item in oblast)
				{
					var isexist = model.rst_obl_control.Where(x => x.dic_kato_id == item.Id).FirstOrDefault();
					if (isexist != null)
						item.IsChecked = true;
				}
			}

			ViewBag.OblastList = oblast;

			return View(model);
		}

		[HttpPost]
		public ActionResult EditByAdmin(SEC_User model, FormCollection form)
		{
			FillBagView(model);
			var user = new SecUserRepository().GetAll().SingleOrDefault(e => e.Login == model.Login && e.Id != model.Id);
			if (user != null && model.Id != user.Id)
			{
				model.IsError = true;
				model.ErrorMessage = "С данным Логином пользователь уже зарегистрирован, обратитесь к администратору";
				return View(model);
			}

			if (model.Id == 0 && model.Pwd != model.ConfirmPwd)
			{
				model.IsConfirm = true;
				return View(model);
			}
			var repository = new SecUserRepository();
			if (model.Id > 0)
			{
				ModelState.Remove("Pwd");
				ModelState.Remove("ConfirmPwd");
				model.Pwd = repository.GetById(model.Id).Pwd;
				model.ConfirmPwd = model.Pwd;
			}
			else
			{
				model.Pwd = RegJurnalManager.Instance.Encrypt(model.Pwd);
			}

			if (ModelState.IsValid)
			{

				model.IsGuest = false;
				model.TypeApplicationId = CodeConstManager.APP_TYPE_PHYS_PERSON;
				repository.SaveOrUpdate(model, MyExtensions.GetCurrentUserId());

				//----control oblast
				try
				{
					var buffer = form["check_control_oblast"];
					if (!string.IsNullOrWhiteSpace(buffer))
					{
						var arr = buffer.Split(',');
						repository.SaveOrUpdate(arr, model.Id);
					}
				}
				catch (Exception ex) { 
				
				}

				return RedirectToAction("Index");
			}
			return View(model);
		}

        [HttpGet]
        [GerNavigateLogger]
        public ActionResult ChangePwd(long id)
        {
            SEC_User model = new SecUserRepository().GetById(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult ChangePwd(SEC_User model)
        {
            if (model.Id == 0 && model.Pwd != model.ConfirmPwd)
            {
                model.IsConfirm = true;
                return View(model);
            }
           
            if (ModelState.IsValid)
            {
                var pwd = RegJurnalManager.Instance.Encrypt(model.Pwd);
                new SecUserRepository().ChangePwd(model.Id, pwd);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //---- Далее присвоить новый пароль и добавить роль ЭЭ2.0.
        [HttpPost]
        public ActionResult ChangePwdByEE2(long userId, string password)
        {
            SEC_User model = new SEC_User();
            model.Id = userId;
            model.Pwd = password;

            var pwd = RegJurnalManager.Instance.Encrypt(model.Pwd);
            new SecUserRepository().ChangePwd(model.Id, pwd);

            new SecUserRepository().AddNewUserKind(model.Id, 5);          
            return Json(model);
        }
      
        [HttpPost]
        public ActionResult Edit(SEC_User environmental)
        {
            FillBagView(environmental);
            if (ModelState.IsValid)
            {
                var repository = new SecUserRepository();
                repository.SaveOrUpdate(environmental, MyExtensions.GetCurrentUserId());
                return RedirectToAction("Index");
            }
            return View("Create", environmental);
        }

    }
}