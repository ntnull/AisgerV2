using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Aisger.Helpers;
using Aisger.Validation;
using Telerik.Reporting.Services.WebApi;

namespace Aisger
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
//            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ValidInteger), typeof(ValidIntegerValidator));
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(ValidDecimal), typeof(ValidDecimalValidator));
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Request.Cookies[CultureHelper.CookiesField] != null && !string.IsNullOrEmpty(Request.Cookies[CultureHelper.CookiesField].Value))
            {
                string culture = Request.Cookies[CultureHelper.CookiesField].Value;
                CultureInfo ci = new CultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = ci;
            }
            else
            {
                HttpCookie cookie = Request.Cookies[CultureHelper.CookiesField];
                if (cookie != null)
                    cookie.Value = CultureHelper.Ru;   // если куки уже установлено, то обновляем значение
                else
                {
                    cookie = new HttpCookie(CultureHelper.CookiesField);
                    cookie.HttpOnly = false;
                    cookie.Value = CultureHelper.Ru;
                    cookie.Expires = DateTime.Now.AddYears(1);
                    cookie.Shareable = true;
                    Response.Cookies.Add(cookie);
                    Thread.CurrentThread.CurrentCulture =
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(CultureHelper.Ru);
                }
            }
        }
    }
}