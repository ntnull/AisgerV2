using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Action;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Models.Repository.Subject;
using Aisger.Utils;

namespace Aisger.Controllers
{

    public abstract class ACommonController : Controller
    {
        // GET: ACommon
		
        protected ACommonController()
        {
            ViewBag.RejectCount = new SubFormRepository().GetCountReject(MyExtensions.GetCurrentUserId());
            ViewBag.InboxCount = new SubFormRepository().GetCountInbox();
            ViewBag.AppActionCount = new SubActionPlanRepository().GetCountInbox();
            var oblasts = new List<string>();
            foreach (var code in CodeConstManager.OBLAST_CODES)
            {
                if (MyExtensions.CheckRight(code))
                {
                    oblasts.Add(code);
                }
            }
            if (oblasts.Count > 0)
            {
                ViewBag.InboxCount =
                    new SubFormRepository().GetCountInboxByOblast(oblasts);
            }
            else
            {
                ViewBag.InboxCount =
                  new SubFormRepository().GetCountInboxByCurrentEmployee(MyExtensions.GetCurrentUserId());
            }

      /*      ViewBag.AllCount = new RstReestrRepository().GetCountRegsiter();
            ViewBag.InboxCount = new RstReestrRepository().GetCountReject(MyExtensions.GetCurrentUserId());
            ViewBag.TaskCount = new SdReestrRepository().GetTaskCount(MyExtensions.GetCurrentUserId());*/

        }
      
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
       
        protected static string GetContentType(string fileName)
        {
            var strcontentType = "application/octetstream";
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var ext = extension.ToLower();
                Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
                if (registryKey != null && registryKey.GetValue("Content Type") != null)
                    strcontentType = registryKey.GetValue("Content Type").ToString();
            }
            return strcontentType;
        }
       
    }
}