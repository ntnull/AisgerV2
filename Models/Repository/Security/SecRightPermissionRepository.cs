using System.Collections.Generic;
using System.Linq;
using Aisger.Models.Entity;
using Aisger.Models.Entity.Security;

namespace Aisger.Models.Repository.Security
{
    public class SecRightPermissionRepository : AObjectRepository<SEC_RightPermission>
    {
        public List<SEC_RightPermission> GetChild(long? parentId)
        {
            return GetQuery(o => o.ParentId.Equals(parentId), true, e => ((IObject) e).Id).ToList();
        }

        public override List<SEC_RightPermission> GetAll()
        {
            return AppContext.SEC_RightPermission.ToList();
        }

        public List<SEC_RightPermissionCustom> GetAllCustom()
        {
            return AppContext.Database.SqlQuery<SEC_RightPermissionCustom>("select *  from \"SEC_RightPermission\"  order by \"Id\" ").ToList();
        }
    }
}