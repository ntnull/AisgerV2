using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Aisger.Utils;

namespace Aisger.Models.Repository.Reestr
{
    public class RstReestrRepository : AObjectRepository<RST_Reestr>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.RST_Reestr; }
        }

        public StatusReestr GetStatusByBin(string bin)
        {
            if (string.IsNullOrEmpty(bin))
            {
                return StatusReestr.EMPTY_REESTR;
            }
            var reestr = AppContext.RST_ReportReestr.FirstOrDefault(e => !e.IsDeleted && e.BINIIN == bin);
            if (reestr == null)
            {
               return StatusReestr.NEW_REESTR;
            }
            if (reestr.StatusId != CodeConstManager.EXTEND_STATUS_REESTR_ID)
            {
                return StatusReestr.INCLULDE_REESTR;
            }
            return StatusReestr.EXCLUDE_REESTR;
        }

        public CheckReestr GetReestrByBin(string bin, int year)
        {
            var entity = new CheckReestr();
            if (string.IsNullOrEmpty(bin))
            {
                entity.StatusReestr = StatusReestr.EMPTY_REESTR;
                return entity;
            }

            var reestr = AppContext.RST_ReportReestr.OrderBy(e => e.RST_Report.ReportYear).FirstOrDefault(e => !e.IsDeleted && e.BINIIN == bin && e.RST_Report.ReportYear== year);
            if (reestr == null)
            {
                entity.StatusReestr = StatusReestr.NEW_REESTR;
                return entity;
            }

            if (!reestr.IsExcluded)
            {
                entity.StatusReestr = StatusReestr.INCLULDE_REESTR;
                entity.OwnerName = reestr.OwnerName;
                entity.Address = reestr.Address;
                return entity;
            }

            entity.StatusReestr = StatusReestr.EXCLUDE_REESTR;
            entity.OwnerName = reestr.OwnerName;
            entity.Address = reestr.Address;
            return entity;
        }

        public RST_ReestrHistory SaveHistory(RST_ReestrHistory history)
        {
            AppContext.RST_ReestrHistory.Add(history);
            AppContext.SaveChanges();
            return history;
        }

        public List<RST_ReestrReportHistory> GetReestrReportHistoryByUserId(long? userId)
        {
            return AppContext.RST_ReestrReportHistory.Where(e => e.UserId == userId).ToList();
        }

		public string SaveRstReestr(RST_Reestr model,long? currUserId)
		{
			string errorMessage = "";
			try
			{
				var rst_application = AppContext.Database.SqlQuery<RST_Application>("select t.* from \"RST_Application\" t where t.\"UserId\"="+currUserId).FirstOrDefault();
				
				model.CreateDate = DateTime.Now;
				model.StatusId = 4;

				if (rst_application != null)
					model.ApplicationId = rst_application.Id;

				AppContext.RST_Reestr.Add(model);
				AppContext.SaveChanges();
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}

			return errorMessage;
		}



    }
}