#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Aisger.Models;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Security;
using Aisger.Helpers;

#endregion

namespace Aisger.Controllers.Security
{
    public class SecRulesController : ACommonController
    {
        //
        // GET: /SecRules/
        private static string lang = CultureHelper.GetCurrentCulture();
        [GerNavigateLogger]
        public ActionResult Index()
        {
            var repository = new SecRolesRepository();
            return View(repository.GetAll().OrderBy(e=>e.Id));
        }

        [HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult Create()
        {
            var employee = new SEC_Roles();
            return View("Edit", employee);
        }


        [HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult Edit(long id)
        {
            SEC_Roles employee = new SecRolesRepository().GetById(id);
            return View("Edit", "_Layout", employee);
        }

		[HttpGet]
        [GerNavigateLogger]
        public virtual ActionResult CopyRole(long id)
		{
			SEC_Roles employee = new SecRolesRepository().GetById(id);
			employee.Code = null;
			employee.NameKz = null;
			employee.NameRu = null;
			return View("CopyRole", "_Layout", employee);
		}

		[HttpPost]
		public ActionResult Edit(SEC_Roles roles, FormCollection form)
		{
			roles.SaveRigthtIds = new List<RigthSave>();
			foreach (string allKey in form.AllKeys)
			{

				if (allKey.Contains("check_"))
				{
					string keysStr =Convert.ToString(form[allKey]);
					var arr = keysStr.Split('*');
					for (int i = 0; i < arr.Length; i++)
					{
						long idkey;

						if (long.TryParse(arr[i], out idkey))
						{
							roles.SaveRigthtIds.Add(new RigthSave { Id = idkey, Edit = false });
						}
					}
				}
			}
			var repository = new SecRolesRepository();
			roles.EditDate = DateTime.Now;
			roles.NameKz = roles.NameRu;
			repository.SaveOrUpdate(roles, MyExtensions.GetCurrentUserId());
			return RedirectToAction("Index");
		}

        [HttpPost]
        public ActionResult EditOld(SEC_Roles roles, FormCollection form)
        {
            roles.SaveRigthtIds = new List<RigthSave>();
            foreach (string allKey in form.AllKeys)
             {
                
                if (allKey.Contains("check_"))
                {
                    bool value = Boolean.Parse(form[allKey]);
                    long idkey;
                    string idString = allKey.Replace("check_", "");
                    if (long.TryParse(idString, out idkey))
                    {
                        roles.SaveRigthtIds.Add(new RigthSave {Id = idkey, Edit = value});
                    }
                }
            }
            var repository = new SecRolesRepository();
            roles.EditDate = DateTime.Now;
            roles.NameKz = roles.NameRu;
            repository.SaveOrUpdate(roles, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }

        public string GetTreeData(long id)
        {
            List<SEC_RightPermission> flatObjects = new SecRightPermissionRepository().GetAll().OrderBy(e=>e.Id).ToList();
            SEC_Roles employee = new SecRolesRepository().GetById(id);
			List<RecursiveObject> recursiveObjects = FillRecursive(flatObjects, null, employee != null ? employee.SEC_RolePermission : null);
            return new JavaScriptSerializer().Serialize(recursiveObjects);
        }
		
        private static List<RecursiveObject> FillRecursive(IEnumerable<SEC_RightPermission> list, long? parentId,
            ICollection<SEC_RolePermission> listPermissions)
        {
            var recursiveObjects = new List<RecursiveObject>();

            foreach (SEC_RightPermission item in list.Where(x => x.ParentId.Equals(parentId)).OrderBy(e=>e.Id))
            {
                bool isChecked;
                var secRolespermissions = listPermissions != null ? listPermissions.FirstOrDefault(a => a.RightPermissionId == item.Id) : null;
                if (listPermissions != null &&
                    secRolespermissions != null)
                {
                    isChecked = true;
                }
                else
                {
                    isChecked = false;
                }
                var recursiveObject = new RecursiveObject
                {
                    data = item.NameRu,
                    id = item.Id,
                    attr =
                        new FlatTreeAttribute
                        {
                            id = item.Id.ToString(CultureInfo.InvariantCulture),
                            selected = isChecked
                        },
                    children = FillRecursive(list.OrderBy(e => e.Id), item.Id, listPermissions)
                };
                if (recursiveObject.children == null || recursiveObject.children.Count == 0)
                {
                    recursiveObject.attr.editable = secRolespermissions != null && secRolespermissions.IS_EDIT;
                }
                recursiveObjects.Add(recursiveObject);
            }
            return recursiveObjects;
        }

		//----
		/*public string GetTreeDatasOld(long id)
		{
			List<SEC_RightPermission> flatObjects = new SecRightPermissionRepository().GetAll().OrderBy(e => e.Id).ToList();
			SEC_Roles employee = new SecRolesRepository().GetById(id);
			List<RecursiveObjectTree> recursiveObjects = FillRecursiveTree(flatObjects, null, employee != null ? employee.SEC_RolePermission : null);
			return new JavaScriptSerializer().Serialize(recursiveObjects);
		}*/

        public string GetTreeDatas(long id)
        {
            List<SEC_RightPermissionCustom> flatObjects = new SecRightPermissionRepository().GetAllCustom();
            SEC_Roles employee = new SecRolesRepository().GetById(id);
            List<RecursiveObjectTree> recursiveObjects = FillRecursiveTree(flatObjects, null, employee != null ? employee.SEC_RolePermission : null);
            return new JavaScriptSerializer().Serialize(recursiveObjects);
        }

        private static List<RecursiveObjectTree> FillRecursiveTree(IEnumerable<SEC_RightPermissionCustom> list, long? parentId,
		 ICollection<SEC_RolePermission> listPermissions)
		{
			var recursiveObjects = new List<RecursiveObjectTree>();

			foreach (SEC_RightPermissionCustom item in list.Where(x => x.ParentId.Equals(parentId)).OrderBy(e => e.Id))
			{
				bool isChecked;
				var secRolespermissions = listPermissions != null ? listPermissions.FirstOrDefault(a => a.RightPermissionId == item.Id) : null;
				if (listPermissions != null &&
					secRolespermissions != null)
				{
					isChecked = true;
				}
				else
				{
					isChecked = false;
				}
				var recursiveObject = new RecursiveObjectTree
				{
					name = (lang!="ru")?item.NameKz:item.NameRu,
					id = item.Id,
					isselected = isChecked,
					items = FillRecursiveTree(list.OrderBy(e => e.Id), item.Id, listPermissions)
				};
				//if (recursiveObject.items == null || recursiveObject.items.Count == 0)
				//{
				//	recursiveObject.attr.editable = secRolespermissions != null && secRolespermissions.IS_EDIT;
				//}
				recursiveObjects.Add(recursiveObject);
			}
			return recursiveObjects;
		}

        public virtual ActionResult Delete(long id)
        {
            new SecRolesRepository().Delete(id, MyExtensions.GetCurrentUserId());
            return RedirectToAction("Index");
        }
    }
}