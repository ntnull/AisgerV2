using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aisger.Models.Constants;
using log4net.Util;
using NPOI.POIFS.Storage;
using NPOI.SS.Formula.Functions;

namespace Aisger.Models.Repository.EnergyAudit
{
    public class CollectorRepository : AObjectRepository<COLLECTOR_Cmdevice>, IDisposable
    {
        public IEnumerable<COLLECTOR_Cmdevice> GetCmdevicesList()
        {
            return GetQuery(e => !e.IsDeleted).ToList();
        }

        public COLLECTOR_Cmdevice GetCmdevice(string cmdCode)
        {
            return GetQuery(e => !e.IsDeleted && e.Code == cmdCode).FirstOrDefault();
        }
        
        public void Dispose()
        {
            
        }

        public List<VIEW_COLL_IndicatorValuesPerDay> GetDataPerDay(string cmdCode, DateTime startPeriod, DateTime endPeriod)
        {
            var device = AppContext.COLLECTOR_Cmdevice.FirstOrDefault(cmd => cmd.Code == cmdCode);
            List<VIEW_COLL_IndicatorValuesPerDay> indicatorValues = null;
            if (device != null)
            {
                long id = device.Id;
                indicatorValues = AppContext.VIEW_COLL_IndicatorValuesPerDay
                    .Where(iv => iv.refCmdevice == id && iv.ReportDate >= startPeriod && iv.ReportDate < endPeriod)
                    .ToList();
            }

            return indicatorValues;
        }

        public List<COLLECTOR_IndicatorValues> GetDataForLastPeriod(string cmdCode, int period)
        {
            var device = AppContext.COLLECTOR_Cmdevice.FirstOrDefault(cmd => cmd.Code == cmdCode);
            List<COLLECTOR_IndicatorValues> indicatorValues = null;
            if (device != null)
            {
                long id = device.Id;
                var datetimeLast = AppContext.COLLECTOR_IndicatorValues
                    .Where(iv => iv.refCmdevice == id).Max(iv => iv.DatetimeStamp);
                if (datetimeLast != null && datetimeLast != DateTime.MinValue)
                {
                    datetimeLast = datetimeLast.AddMinutes((-1) * period);
                    indicatorValues = AppContext.COLLECTOR_IndicatorValues
                        .Where(iv => iv.refCmdevice == id && iv.DatetimeStamp >= datetimeLast)
                        .OrderByDescending(iv => iv.DatetimeStamp)
                        .ToList();
                }
            }
            return indicatorValues;
        }

        public List<SelectListItem> GetIndicatorTypeList()
        {
            var typeList = AppContext.COLLECTOR_DIC_CmdeviceTypes.Select(ct => new SelectListItem()
            {
                Value = ct.Id.ToString(),
                Text = ct.NameRu
            }).ToList();

            return typeList;
        }
    }
}