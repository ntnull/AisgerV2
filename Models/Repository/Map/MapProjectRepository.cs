using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger.Utils;
using NPOI.SS.Formula.Functions;

namespace Aisger.Models.Repository.Map
{
    public class MapProjectRepository : AObjectRepository<MAP_Project>
    {
       
        public virtual List<SEC_User> GetEscoList()
        {
            var list= from s in AppContext.SEC_User
            join sa in AppContext.SEC_UserKind on s.Id equals sa.UserId
            where sa.KindId == 3 && !s.IsDeleted
            select s;
            return list.ToList();
        }

        public List<MAP_Application> GetAppList(MAP_Project model)
        {
            IEnumerable<long?> ids;
            if (model.Id == 0)
            {
                ids = GetCollectionList().Select(e => e.ApplicationId);
                return AppContext.MAP_Application.Where(e => e.MAP_DIC_Status.Code==CodeConstManager.MAP_STATUS_FINISHED && !ids.Contains(e.Id)).ToList();
            }
            ids = GetCollectionList().Where(e=>e.Id!=model.Id).Select(e => e.ApplicationId);
            return AppContext.MAP_Application.Where(e => e.MAP_DIC_Status.Code == CodeConstManager.MAP_STATUS_FINISHED && !ids.Contains(e.Id)).ToList();
        }
    }
}