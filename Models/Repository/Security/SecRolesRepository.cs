using System.Collections.Generic;
using System.Linq;

namespace Aisger.Models.Repository.Security
{
    public class SecRolesRepository : AObjectRepository<SEC_Roles>
    {
        public SEC_Roles GetByCode(string code)
        {
            return AppContext.SEC_Roles.FirstOrDefault(e => e.Code == code);
        }

     

        protected override void BeforeSave(SEC_Roles newObj, SEC_Roles obj)
        {
           
            if (obj.Id == 0) 
            {
                obj.SEC_RolePermission = new List<SEC_RolePermission>();
                foreach (var saveRigthtId in obj.SaveRigthtIds)
                {
                    obj.SEC_RolePermission.Add(new SEC_RolePermission
                    {
                        RightPermissionId = saveRigthtId.Id,
                        IS_EDIT = saveRigthtId.Edit,
                       SEC_Roles = obj
                    });
                }
                return;
            }
            if (obj.SaveRigthtIds == null)
            {
                return;
            }
            var list = AppContext.SEC_RolePermission.Where(e => e.RolesId == obj.Id).ToList();
            IList<SEC_RolePermission> deleteList = new List<SEC_RolePermission>();
           
            foreach (var rolePermission in list)
            {
                var saveRigth =  obj.SaveRigthtIds.FirstOrDefault(a => a.Id == rolePermission.RightPermissionId);
                if (saveRigth == null)
                {
                    deleteList.Add(rolePermission);
                }
                else
                {
                    if (!rolePermission.IS_EDIT.Equals(saveRigth.Edit))
                    {
                        rolePermission.IS_EDIT = saveRigth.Edit;
//                        AppContext.SEC_ROLESPERMISSIONS.Attach(rolePermission);//todo!! check
                        var attachedEntity = AppContext.Set<SEC_RolePermission>().Find(rolePermission.Id);
                        AppContext.Entry(attachedEntity).CurrentValues.SetValues(rolePermission);
                    }
                }
            }

            var listOld = list.Select(a => a.RightPermissionId).ToList();
            var newRigths = obj.SaveRigthtIds.Where(a => !listOld.Contains(a.Id)).ToList();

            foreach (var rigthSave in newRigths)
            {
                AppContext.SEC_RolePermission.Add(new SEC_RolePermission
                {
                    RightPermissionId = rigthSave.Id,
                    IS_EDIT = rigthSave.Edit,
                    RolesId = obj.Id
                });
            }


            foreach (var secRolePermission in deleteList)
            {
                AppContext.Set<SEC_RolePermission>().Remove(secRolePermission);
            }


        }
    }
}