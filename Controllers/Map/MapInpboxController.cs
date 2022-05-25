using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Map;
using Aisger.Utils;
using Aisger.Models.Repository.Security;

namespace Aisger.Controllers.Map
{
    public class MapInpboxController : AMapController
    {
        //
        // GET: /MapApp/
        [GerNavigateLogger]
        public ActionResult Index()
        {
           var collection = new List<MAP_Application>();
           var list = new MapApplicationRepository().GetCollectionList().Where(e => e.Editor == MyExtensions.GetCurrentUserId());
           foreach (var mapApplication in list)
            {
                if (mapApplication.SEC_User1 != null)
                {
                    var userKind =
                        mapApplication.SEC_User1.SEC_UserKind.FirstOrDefault(
                            e => e.KindId == CodeConstManager.KIND_USER_MAP);
                    if (userKind != null && userKind.IsBlocked != null)
                    {
                        mapApplication.IsBlocked = userKind.IsBlocked.Value;
                    }
                }

                collection.Add(mapApplication);
            }
            return View(list);
        }

    }
}
