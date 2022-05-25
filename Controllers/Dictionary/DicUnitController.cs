using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models;
using Aisger.Models.Repository.Dictionary;

namespace Aisger.Controllers.Dictionary
{

    public class DicUnitController : ABaseDicController<DIC_Unit, DicUnitRepository>
    {
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(SUB_DIC_KindResource model)
        {

            return GetPostHolder(model);
        }

    }
}
