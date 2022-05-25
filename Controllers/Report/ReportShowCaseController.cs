using Aisger.Models.Repository.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Controllers.Report
{
    public class ReportShowCaseController : ACommonController
    {
        //
        // GET: /ReportShowCase/
        [GerNavigateLogger]
        public ActionResult Index()
        {
            return View();
        }

    }
}
