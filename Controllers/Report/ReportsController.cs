using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Telerik.Reporting.Cache.Interfaces;
using Telerik.Reporting.Services.Engine;
using Telerik.Reporting.Services.WebApi;
using CacheFactory = Telerik.Reporting.Services.WebApi.CacheFactory;

namespace Aisger.Controllers.Report
{
    public class ReportsController : ReportsControllerBase
    {
        protected override IReportResolver CreateReportResolver()
        {
            var reportsPath = HttpContext.Current.Server.MapPath("~/Reports");

            return new ReportFileResolver(reportsPath)
              .AddFallbackResolver(new ReportTypeResolver());
        }

        protected override ICache CreateCache()
        {
            //return CacheFactory.CreateFileCache();
            return Telerik.Reporting.Services.Engine.CacheFactory.CreateFileCache();
        }
    }
}