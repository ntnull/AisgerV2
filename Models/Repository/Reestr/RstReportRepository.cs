using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Aisger.Helpers;
using Aisger.Models.Entity.Security;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using NPOI.SS.Formula.Functions;
using Aisger.Models.Entity.Reestr;
using Aisger.Models.Entity.Dictionary;

namespace Aisger.Models.Repository.Reestr
{
    public class RstReportRepository : AObjectRepository<RST_Report>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.RST_Report; }
        }

        public int GetNewYear()
        {
            var year = AppContext.RST_Report.Max(e => e.ReportYear);
            if (year == null)
            {
                return DateTime.Now.Year;
            }
            return year.Value + 1;
        }

        public List<UnMappedDictionary> GetYears()
        {
            var reportsYear = GetAll().OrderBy(e=>e.ReportYear).Select(e => e.ReportYear).Distinct();
//            var years = AppContext.RST_Application.Where(e => !reportsYear.Contains(e.ReportYear)).Select(e => e.ReportYear).Distinct();
            var list = new List<UnMappedDictionary>();
            foreach (var year in reportsYear)
            {
                if (year != null)
                {
                    list.Add(new UnMappedDictionary(Convert.ToInt64(year), year.ToString()));
                }
            }
            return list;
        }

		public List<RST_Report> GetRstReport()
		{
			var list = AppContext.Database.SqlQuery<RST_Report>("select * from \"RST_Report\" where \"IsDeleted\"=false  order by \"Id\" ").ToList();// Where(e => !e.IsDeleted).ToList();
			return list;
		}

        public List<DicOKEDView> GetOKED(string lang)
        {
            var list = AppContext.Database.SqlQuery<DicOKEDView>("select \"Id\", case '" + lang + "' when 'kz' then \"NameKz\" else \"NameRu\" end \"Name\" from \"DIC_OKED\" where \"refParent\" is null ").ToList();
            return list;
        }

        public List<RST_ReportCustom> GetRstReportCustom()
        {
            var list = AppContext.Database.SqlQuery<RST_ReportCustom>("select * from \"RST_Report\" where \"IsDeleted\"=false  order by \"Id\" ").ToList();// Where(e => !e.IsDeleted).ToList();
            return list;
        }

        public List<RST_ReportReestr> GetRstReportReestrsByUserId(long? userId)
        {
            return AppContext.RST_ReportReestr.Where(e => !e.IsDeleted && e.UserId == userId).ToList();
        }

        protected override void BeforeSave(RST_Report attachedEntity, RST_Report oldEntity)
        {
            if (!oldEntity.IsDeleted)
            {
//                UpdateProducts(attachedEntity, oldEntity);
            }
        }

        private void UpdateProducts(RST_Report attachedEntity, RST_Report oldEntity)
        {
            if (oldEntity.Id == 0)
            {
                var list = GetReestr(oldEntity.ReportYear);
                foreach (var temp in list)
                {
                    var entity = new RST_ReportReestr
                    {
                        IDK = temp.IDK,
                        BINIIN = temp.BINIIN,
                        Address = temp.Address,
                        Oblast = temp.Oblast,
                        OwnerName = temp.OwnerName,
                        ReestrId = temp.Id,
                        StatusId = temp.StatusId,
                        RST_Report = oldEntity,
                        UserId = temp.UserId,
                        ReasonId = temp.ReasonId,
                        Expectant = temp.Expectant,
                        
                    };
                    var history = new RST_ReestrReportHistory
                    {
                        StatusId = entity.StatusId,
                        Author = oldEntity.UserId,
                        UserId = entity.UserId,
                        Oblast = entity.Oblast,
                        ReportYear = oldEntity.ReportYear,
                        RegDate = DateTime.Now,
                        RST_ReportReestr = entity,
                        Note = "Перенесен на следующий год",
                        Expectant = entity.Expectant,
                        ReasonId = entity.ReasonId,
                    };
                    AppContext.RST_ReestrReportHistory.Add(history);
                    AppContext.RST_ReportReestr.Add(entity);
                }
                AppContext.SaveChanges();
            }
        }

        public IEnumerable<RST_TEMP_ReportReestr> GetNewReestr(int? year)
        {
            if (year == null)
            {
                return new List<RST_TEMP_ReportReestr>();
            }

            var newStatus = AppContext.RST_DIC_Status.FirstOrDefault(e => e.Id == CodeConstManager.NEW_STATUS_REESTR_ID);
            var oldtatus = AppContext.RST_DIC_Status.FirstOrDefault(e => e.Id == CodeConstManager.OLD_STATUS_REESTR_ID);
            year = year - 1;
            return from s in AppContext.RST_ReportReestr
                   join sa in AppContext.RST_Report on s.ReportId equals sa.Id
                   where !s.IsDeleted && !s.IsExcluded && sa.ReportYear == year
//                   where !s.IsDeleted && s.StatusId != 1 && sa.ReportYear == year
                   group s by
                   new
                   {
                       s.Id,
                       s.IDK,
                       s.BINIIN,
                       s.OwnerName,
                       s.Address,
                       sa.ReportYear,
                       OblastName = s.DIC_Kato.NameRu,
                       Oblast = s.DIC_Kato.Id,
                       s.UserId,
                       s.ReasonId,
                       s.Expectant
                   }
                       into grp
                       select new RST_TEMP_ReportReestr
                       {
                           Id = grp.Key.Id,
                           IDK = grp.Key.IDK,
                           BINIIN = grp.Key.BINIIN,
                           OwnerName = grp.Key.OwnerName,
                           Address = grp.Key.Address,
                           OblastName = grp.Key.OblastName,
                           Oblast = grp.Key.Oblast,
                           StatusId = 4,
                           ReasonId = grp.Key.ReasonId,
                           Expectant = grp.Key.Expectant,
                           StatusName = "Перенесен с прошлого года"
                       };

        }
        public IEnumerable<RST_TEMP_ReportReestr> GetReestr(int? year)
        {
            if (year == null)
            {
                return new List<RST_TEMP_ReportReestr>();
            }
            var newStatus = AppContext.RST_DIC_Status.FirstOrDefault(e => e.Id == CodeConstManager.NEW_STATUS_REESTR_ID);
            var oldtatus = AppContext.RST_DIC_Status.FirstOrDefault(e => e.Id == CodeConstManager.OLD_STATUS_REESTR_ID);

            return from s in AppContext.RST_Reestr
            join sa in AppContext.RST_Application on s.ApplicationId equals sa.Id
            where sa.ReportYear <= year && !sa.IsDeleted && !s.IsDeleted && s.StatusId==1
            group s by
            new
            {
               s.Id,
               s.IDK,
               s.BINIIN,
               s.OwnerName,
               s.Address,
               sa.ReportYear,
               OblastName =sa.DIC_Kato.NameRu, 
               Oblast = sa.DIC_Kato.Id
            }
            into grp
                       select new RST_TEMP_ReportReestr
                       {
                          Id=grp.Key.Id,
                          IDK = grp.Key.IDK,
                          BINIIN = grp.Key.BINIIN,
                          OwnerName = grp.Key.OwnerName,
                          Address = grp.Key.Address,
                          OblastName =  grp.Key.OblastName,
                          Oblast = grp.Key.Oblast,
                          StatusId = grp.Key.ReportYear < year ? oldtatus.Id : newStatus.Id,
                          StatusName = grp.Key.ReportYear < year ? oldtatus.NameRu : newStatus.NameRu
                       };

        }

        public RST_ReportReestr GetRecord(long id)
        {
            return AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == id);
        }

        public void UpdateReportReestr(RST_ReportReestr model, long? getCurrentUserId)
        {
            var entity = AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == model.Id);
            if (entity == null)
            {
                return;
            }
            entity.StatusId = model.StatusId;
            entity.ReasonId = model.ReasonId;
            entity.EditDate = model.EditDate;
            entity.Note = model.Note;
            entity.UserId = getCurrentUserId;
            AppContext.SaveChanges();
        }

        public IEnumerable<RST_ReportReestrFilter> GetRstReportReestrsByFilter(RST_ReportFilter filter)
        {
            string query = "select r.\"Id\",\"ReportId\",\"Address\",\"BINIIN\",\"OwnerName\",\"StatusId\",\"Oblast\",\"ReasonId\","+
            "s.\"NameRu\" as \"StatusName\","+ 
            "d.\"NameRu\" as \"ReasonName\", "+
            "k.\"NameRu\" as \"OblastName\" from public.\" \"  as r " +
            "left join public.\"RST_DIC_Status\" as s on s.\"Id\"=r.\"StatusId\" "+
            "left join public.\"RST_DIC_Reason\" as d on d.\"Id\"=r.\"ReasonId\" "+
            "left join public.\"DIC_Kato\" as k on k.\"Id\"=r.\"Oblast\" "+
                           " where ";
            if (!string.IsNullOrEmpty(filter.BINIIN))
            {
                query = query + " \"BINIIN\" like '%" + filter.BINIIN + "%' AND ";
            }
            if (!string.IsNullOrEmpty(filter.Adress))
            {
                query = query + " \"Address\" like '%" + filter.Adress + "%' AND ";
            }
            if (!string.IsNullOrEmpty(filter.SubjectName))
            {
                query = query + " \"OwnerName\" like '%" + filter.SubjectName + "%' AND ";
            }
      
            if (filter.Statuses != null && filter.Statuses.Count > 0)
            {
                query = query + " \"StatusId\" IN (" + string.Join(",",filter.Statuses) + ") AND ";
            }
            if (filter.Reasons != null && filter.Reasons.Count > 0)
            {
                query = query + " \"ReasonId\" IN (" + string.Join(",", filter.Reasons) + ") AND ";
            }
            if (filter.Oblasts != null && filter.Oblasts.Count > 0)
            {
                query = query + " \"Oblast\" IN (" + string.Join(",", filter.Oblasts) + ") AND ";
            }
            query = query + " \"ReportId\"="+filter.ReportId;
            var list = AppContext.Database.SqlQuery<RST_ReportReestrFilter>(query);
            return list.OrderBy(e => e.Id);
            /*   var list = AppContext.RST_ReportReestr.Where(e => e.ReportId==filter.ReportId);
            if (!string.IsNullOrEmpty(filter.BINIIN))
           
            return list.OrderBy(e=>e.Id);*/
        }

        public IEnumerable<RST_ReportReestrFilter> GetCommonReestrsByFilter(RST_ReportFilter filter)
        {
            var sendjoin = "left";
            var sendquery = "";
            if (filter.SendId != null && filter.SendId.Value > 0)
            {
                switch (filter.SendId.Value)
                {
                    case CodeConstManager.SUB_REASON_SEND_ID:
                        {
                            sendquery = " AND f.\"IsBack\"='true' ";
                            sendjoin = "inner";
                            break;
                        }
                    case CodeConstManager.SUB_REASON_NOTSEND_ID:
                        {
                            sendquery = " AND f.\"IsBack\"<>'true' ";
                            sendjoin = "inner";
                            break;
                        }
                }
            }
            var nameIndex = CultureHelper.GetDictionaryName("NameRu");
            string query =
                "select r.\"UserId\", r.\"Id\",r.\"IDK\",r.\"usrfscode\" ,\"Expectant\",f.\"SendDate\" as \"SendDate\", \"ReportId\",r.\"Address\",(case when r.\"BINIIN\" is not null then r.\"BINIIN\" else ow.\"BINIIN\" end) as \"BINIIN\", \"OwnerName\" , r.\"StatusId\",r.\"IsExcluded\",r.\"Oblast\",\"ReasonId\", " +
                "(case when  r.\"IsExcluded\"=TRUE then 'Исключен' else NULL end) as \"ExcludedName\", " +
                " s.\"" + nameIndex + "\"  as \"StatusName\", " +
                "d.\"" + nameIndex + "\" as \"ReasonName\",  " +
                "k.\"" + nameIndex + "\" as \"OblastName\", " +
                "d1.\"" + nameIndex + "\" as \"ExpectantName\",  " +
                "f.\"Id\" as \"FormId\", " +
                "sf.\"Code\" as \"FormStatusCode\", " +
                "(case when sf.\"" + nameIndex + "\" is null then '" + CultureHelper.GetDictionaryName(CodeConstManager.SUB_DIC_STATUS_NOTGIVED) + "' else sf.\"" + nameIndex + "\" end) as \"FormStatus\", " +
                "(case when f.\"IsBack\" is null then FALSE else f.\"IsBack\" end) as \"IsBack\", " +
                " us.\"Id\" as \"Editor\", (us.\"LastName\" ||' '||SUBSTRING(us.\"FirstName\", 1, 1)||'. '||(case when us.\"SecondName\" is null then '' else SUBSTRING(us.\"SecondName\", 1, 1)||'.' END)) as \"AuthorName\" " +
                "from public.\"RST_ReportReestr\"  as r  " +
                "inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\" " +
                "inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\" " +
                 sendjoin + " join public.\"SUB_Form\" as f on f.\"Id\"=r.\"FormId\" " +
                 "left join public.\"SEC_User\" as ow on ow.\"Id\"=f.\"UserId\" " +
                 sendjoin + " join public.\"SUB_DIC_Status\" as sf on sf.\"Id\"=f.\"StatusId\" " +
                 sendjoin + " join public.\"SEC_User\" as us on us.\"Id\"=f.\"Editor\" " +
                "left join public.\"RST_DIC_Status\" as s on s.\"Id\"=r.\"StatusId\"  " +
                "left join public.\"RST_DIC_Reason\" as d on d.\"Id\"=r.\"ReasonId\"  " +
                "left join public.\"RST_DIC_Reason\" as d1 on d1.\"Id\"=r.\"Expectant\"  " +
                "left join public.\"DIC_Kato\" as k on k.\"Id\"=r.\"Oblast\" " +
                "where ";

            #region for count query
            string query2 =
           "select count(r.\"Id\") from public.\"RST_ReportReestr\"  as r  " +
           "inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\" " +
           "inner join public.\"SEC_User\" as usr on r.\"UserId\"=usr.\"Id\" " +
            sendjoin + " join public.\"SUB_Form\" as f on f.\"Id\"=r.\"FormId\" " +
            "left join public.\"SEC_User\" as ow on ow.\"Id\"=f.\"UserId\" " +
            sendjoin + " join public.\"SUB_DIC_Status\" as sf on sf.\"Id\"=f.\"StatusId\" " +
            sendjoin + " join public.\"SEC_User\" as us on us.\"Id\"=f.\"Editor\" " +
           "left join public.\"RST_DIC_Status\" as s on s.\"Id\"=r.\"StatusId\"  " +
           "left join public.\"RST_DIC_Reason\" as d on d.\"Id\"=r.\"ReasonId\"  " +
           "left join public.\"RST_DIC_Reason\" as d1 on d1.\"Id\"=r.\"Expectant\"  " +
           "left join public.\"DIC_Kato\" as k on k.\"Id\"=r.\"Oblast\" " +
           "where r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"= " + filter.ReportYear + "  and usr.\"IsDeleted\"=FALSE ";
            #endregion

            string where = "";            
            if (!string.IsNullOrEmpty(filter.BINIIN))
            {
                where = where + " r.\"BINIIN\" like '%" + filter.BINIIN + "%' AND ";
            }
            if (!string.IsNullOrEmpty(filter.IDK))
            {
                where = where + " r.\"IDK\" like '%" + filter.IDK + "%' AND ";
            }
            if (!string.IsNullOrEmpty(filter.Adress))
            {
                where = where + " LOWER(r.\"Address\") like LOWER('%" + filter.Adress + "%') AND ";
            }
            if (!string.IsNullOrEmpty(filter.SubjectName))
            {

                where = where + " LOWER(\"OwnerName\") like LOWER('%" + filter.SubjectName + "%') AND ";
                //where = where + " LOWER(r.usrjuridicalname) like LOWER('%" + filter.SubjectName + "%') AND ";
            }

            if (filter.Statuses != null && filter.Statuses.Count > 0)
            {
                where = where + " r.\"StatusId\" IN (" + string.Join(",", filter.Statuses) + ") AND ";
            }
            if (filter.ExcludedId != null && filter.ExcludedId.Value > 0)
            {
                switch (filter.ExcludedId.Value)
                {
                    case CodeConstManager.RST_EXCLUDED_ID:
                        {
                            where = where + " r.\"IsExcluded\"=TRUE AND ";
                            break;
                        }
                    case CodeConstManager.RST_NOTEXCLUDED_ID:
                        {
                            where = where + " r.\"IsExcluded\"=FALSE AND ";
                            break;
                        }
                }
            }

            if (filter.Reasons != null && filter.Reasons.Count > 0)
            {
                where = where + " r.\"ReasonId\" IN (" + string.Join(",", filter.Reasons) + ") AND ";
            }
            if (filter.Oblasts != null && filter.Oblasts.Count > 0)
            {
                where = where + " r.\"Oblast\" IN (" + string.Join(",", filter.Oblasts) + ") AND ";
            }
            if (filter.Expacts != null && filter.Expacts.Count > 0)
            {
                where = where + " r.\"Expectant\" IN (" + string.Join(",", filter.Expacts) + ") AND ";
            }

            if (filter.FsCodes != null && filter.FsCodes.Count > 0)
            {
                string fsCode = string.Join(",", filter.FsCodes);
                if (fsCode.Contains("5"))
                {
                    filter.FsCodes = filter.FsCodes.Where(x => !x.Contains("5")).ToList();
                    string strIds = string.Join(",", filter.FsCodes);
                    if (strIds != "")
                    {
                        where = where + " ( (r.usrfscode IN (" + strIds + ")) or (r.usrfscode is null)) AND ";
                    }
                    else where = where + " r.usrfscode is null and ";                    
                }
                else
                {
                    where = where + " r.usrfscode IN (" + string.Join(",", filter.FsCodes) + ") AND ";
                }
            }

            if (filter.SubDicStatuses != null && filter.SubDicStatuses.Count > 0)
            {
                var listSubStatus = new List<string>();
                var isNull = false;
                foreach (var subDicStatuse in filter.SubDicStatuses)
                {
                    if (subDicStatuse == "0")
                    {
                        isNull = true;
                    }
                    else
                    {
                        listSubStatus.Add(subDicStatuse);
                    }

                }
                if (!isNull)
                {
                    where = where + " sf.\"Id\" IN (" + string.Join(",", listSubStatus) + ") AND ";
                }
                else
                {
                    var statquery = "";
                    if (listSubStatus.Count > 0)
                    {
                        statquery = " sf.\"Id\" IN (" + string.Join(",", listSubStatus) + ") OR ";
                    }

                    where = where + " (" + statquery + " sf.\"Id\" IS NULL ) AND ";
                }
            }
            where = where + " r.\"IsDeleted\"=FALSE AND rr.\"ReportYear\"= " + filter.ReportYear + "  and usr.\"IsDeleted\"=FALSE ";
            where = where + sendquery;
            string sortIndex;
            if (filter.SortId == CodeConstManager.SORT_INDEX_DATEEDIT)
            {
                sortIndex = "  order by r.\"EditDate\" DESC NULLS LAST,  r.\"Id\" DESC  NULLS LAST";
            }
            else
            {
                sortIndex = "  order by  f.\"SendDate\" DESC NULLS LAST, f.\"Id\" DESC NULLS LAST";
            }

            query = query+" "+where+ sortIndex;

            AppContext.Database.CommandTimeout = 180;
            //if (filter.Count == 0)
            //{
            //    filter.Count= AppContext.Database.SqlQuery<int>(query2).FirstOrDefault();
            //}
            #region pagination
            /*
            //----
            if (filter.PageNum == 1)
            {
                int _count= AppContext.Database.SqlQuery<int>("select count(gr.*) from (" + query + ") gr ").FirstOrDefault();
                filter.Count = _count;
                filter.PageCount = (_count % 2 == 0) ? _count / 50 : (_count / 50) + 1;
            }

            int offset = 0;
            if (filter.PageNum > 1)
                offset = (filter.PageNum - 1) * 50;

            query = "select gr.* from (" + query + ") gr limit 50 OFFSET " + offset;*/
            #endregion

            return AppContext.Database.SqlQuery<RST_ReportReestrFilter>(query);
        }

        public void UpdateExecuted(RST_ExecutedFilter filter)
        {
            var list =
                AppContext.RST_ReportReestr.Where(
                    e => e.Expectant != null && e.RST_Report.ReportYear == filter.ReportYear && e.StatusId!=2);
            foreach (var report in list)
            {
                var history = new RST_ReestrReportHistory
                {
//                    StatusId = 2,
                    Author = filter.UserId,
                    UserId = report.UserId,
                    Oblast = report.Oblast,
                    ReportYear = filter.ReportYear,
                    RegDate = DateTime.Now,
                    RST_ReportReestr = report,
                    Note = "Из кандидатов перенесен в исключенные",
                    Expectant = report.Expectant,
//                    ReasonId = report.Expectant
                };
                AppContext.RST_ReestrReportHistory.Add(history);

            }
            AppContext.SaveChanges();
            AppContext.Database.ExecuteSqlCommand("UPDATE public.\"RST_ReportReestr\" SET \"IsExcluded\"=TRUE,\"ReasonId\"=\"Expectant\" from public.\"RST_Report\" as rr WHERE public.\"RST_ReportReestr\".\"ReportId\"=rr.\"Id\" and rr.\"ReportYear\"=" + filter.ReportYear + " AND public.\"RST_ReportReestr\".\"Expectant\" is not null"); 
        }
        public void UpdateEvaders(RST_ReportFilter filter)
        {
            var list =
                AppContext.RST_ReportReestr.Where(
                    e => e.FormId == null && e.RST_Report.ReportYear == filter.ReportYear && !e.IsDeleted).ToList();
            foreach (var report in list)
            {
                var history = new RST_ReestrReportHistory
                {
                    StatusId = report.StatusId,
                    Author = filter.UserId,
                    Oblast = report.Oblast,
                    UserId = report.UserId,
                    ReportYear = report.RST_Report.ReportYear,
                    RegDate = DateTime.Now,
                    RST_ReportReestr = report,
                    Note = "Отмечены как уклонисты, не сдавшие отчет",
                    Expectant =  CodeConstManager.STATUS_EVADERS_ID,
                    ReasonId = CodeConstManager.STATUS_EVADERS_ID
                };
                report.Expectant = CodeConstManager.STATUS_EVADERS_ID;
                report.ReasonId = CodeConstManager.STATUS_EVADERS_ID;

                AppContext.RST_ReestrReportHistory.Add(history);

            }
            AppContext.SaveChanges();
//            AppContext.Database.ExecuteSqlCommand("UPDATE public.\"RST_ReportReestr\" SET \"StatusId\"=2,\"ReasonId\"=\"Expectant\" from public.\"RST_Report\" as rr WHERE public.\"RST_ReportReestr\".\"ReportId\"=rr.\"Id\" and rr.\"ReportYear\"=" + filter.ReportYear + " AND public.\"RST_ReportReestr\".\"Expectant\" is not null"); 
        }
        
        public IEnumerable<RST_ReportReestrFilter> GetExpactedCommonReestrsByFilter(RST_ExecutedFilter filter)
        {
            var nameIndex = CultureHelper.GetDictionaryName("NameRu");
            var nameGive = CultureHelper.GetDictionaryName("nameGive");
            string query =
                "select r.\"UserId\", r.\"Id\",\"IDK\",\"Expectant\",f.\"SendDate\" as \"SendDate\", \"ReportId\",\"Address\",\"BINIIN\",\"OwnerName\",r.\"StatusId\",r.\"IsExcluded\", r.\"Oblast\",\"ReasonId\", " +
                "(case when r.\"IsExcluded\"=TRUE then '" + CultureHelper.GetDictionaryName(CodeConstManager.RST_STATUS_EXCLUDED_NAME) + "' else  s.\"" + nameIndex + "\" end) as \"StatusName\", " +
                "d.\"" + nameIndex + "\" as \"ReasonName\",  " +
                "k.\"" + nameIndex + "\" as \"OblastName\", " +
                "d1.\"" + nameIndex + "\" as \"ExpectantName\",  " +
                "f.\"Id\" as \"FormId\", " +
                "(case when sf.\"" + nameIndex + "\" is null then '"+nameGive+"' else sf.\"" + nameIndex + "\" end) as \"FormStatus\" " +
                "from public.\"RST_ReportReestr\"  as r  " +
                "inner join public.\"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\" " +
                " left join public.\"SUB_Form\" as f on f.\"Id\"=r.\"FormId\" " +
                "left join public.\"SUB_DIC_Status\" as sf on sf.\"Id\"=f.\"StatusId\" " +
                "left join public.\"RST_DIC_Status\" as s on s.\"Id\"=r.\"StatusId\"  " +
                "left join public.\"RST_DIC_Reason\" as d on d.\"Id\"=r.\"ReasonId\"  " +
                "left join public.\"RST_DIC_Reason\" as d1 on d1.\"Id\"=r.\"Expectant\"  " +
                "left join public.\"DIC_Kato\" as k on k.\"Id\"=r.\"Oblast\" " +
                "where ";
            if (!string.IsNullOrEmpty(filter.BINIIN))
            {
                query = query + " \"BINIIN\" like '%" + filter.BINIIN + "%' AND ";
            }
            if (!string.IsNullOrEmpty(filter.IDK))
            {
                query = query + " \"IDK\" like '%" + filter.IDK + "%' AND ";
            }
            if (filter.Oblasts != null && filter.Oblasts.Count > 0)
            {
                query = query + " r.\"Oblast\" IN (" + string.Join(",", filter.Oblasts) + ") AND ";
            }
            if (!string.IsNullOrEmpty(filter.Adress))
            {
                query = query + " LOWER(\"Address\") like LOWER('%" + filter.Adress + "%') AND ";
            }
            if (!string.IsNullOrEmpty(filter.SubjectName))
            {
                query = query + " LOWER(\"OwnerName\") like LOWER('%" + filter.SubjectName + "%') AND ";
            }
            query = query + " rr.\"ReportYear\"= " + filter.ReportYear ;
            if (filter.Expacts != null && filter.Expacts.Count > 0)
            {
                query = query + "AND  r.\"Expectant\" IN (" + string.Join(",", filter.Expacts) + ") ";
            }

            if (filter.Statuses != null && filter.Statuses.Count > 0)
            {
                if (filter.Statuses.Count == 1)
                {
                    var code = filter.Statuses[0];
                    switch (code)
                    {
                        case "1":
                        {
                            query = query + " AND (r.\"Expectant\" is not null AND r.\"IsExcluded\"=FALSE) ";
                            break;
                        }
                        case "2":
                        {
                            query = query + " AND (r.\"IsExcluded\"=TRUE) ";
                            break;
                        }
                        case "3":
                        {
                            query = query + " AND (r.\"Expectant\" is not null or r.\"IsExcluded\"=TRUE) ";
                            break;
                        }

                    }
                   
                }
                else
                {
                    query = query + " AND (r.\"Expectant\" is not null or r.\"IsExcluded\"=TRUE) ";
                }
            }
            else
            {
                query = query + " AND (r.\"Expectant\" is not null or r.\"IsExcluded\"=TRUE) ";
            }
            string sortIndex;
            if (filter.SortId == CodeConstManager.SORT_INDEX_DATEEDIT)
            {
                sortIndex = " order by r.\"EditDate\" DESC NULLS LAST,  r.\"Id\" DESC  NULLS LAST";
            }
            else
            {
                sortIndex = " order by  f.\"SendDate\" DESC NULLS LAST, f.\"Id\" DESC NULLS LAST";
            }

//             query = query + " order by r.\"EditDate\" DESC, r.\"Id\" DESC";
            query = query + sortIndex;
            return AppContext.Database.SqlQuery<RST_ReportReestrFilter>(query);
            /*   var list = AppContext.RST_ReportReestr.Where(e => e.ReportId==filter.ReportId);
            if (!string.IsNullOrEmpty(filter.BINIIN))
           
            return list.OrderBy(e=>e.Id);*/
        }

        public void UpdateExpact(long recordId, long? fieldValue, long? userId)
        {
            var report = AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == recordId);
            if (report == null)
            {
                return;
            }
            report.Expectant = fieldValue;
            var history = new RST_ReestrReportHistory
            {
                StatusId = report.StatusId,
                Author = userId,
                UserId = report.UserId,
                Oblast = report.Oblast,
                ReportYear = report.RST_Report.ReportYear,
                RegDate = DateTime.Now,
                RST_ReportReestr = report,
                ReasonId = report.ReasonId,
                Note = "Указан кандидатом на исключение",
                Expectant = fieldValue
            };
            AppContext.RST_ReestrReportHistory.Add(history);
            AppContext.SaveChanges();
        }

        public void UpdateReason(long recordId, long? fieldValue, long? userId)
        {
            var report = AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == recordId);
            if (report == null)
            {
                return;
            }
            report.ReasonId = fieldValue;
            var history = new RST_ReestrReportHistory
            {
                StatusId = report.StatusId,
                Author = userId,
                UserId = report.UserId,
                Oblast = report.Oblast,
                ReportYear = report.RST_Report.ReportYear,
                RegDate = DateTime.Now,
                RST_ReportReestr = report,
                ReasonId = report.ReasonId,
                Note = "",
                Expectant = report.Expectant
            };
            AppContext.RST_ReestrReportHistory.Add(history);
            AppContext.SaveChanges();
        }

        public void UpdateFsCode(long recordId, int? fieldValue, long? userId)
        {
            var report = AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == recordId);
            if (report == null)
            {
                return;
            }
            report.usrfscode = fieldValue;
            var history = new RST_ReestrReportHistory
            {
                StatusId = report.StatusId,
                Author = userId,
                UserId = report.UserId,
                Oblast = report.Oblast,
                ReportYear = report.RST_Report.ReportYear,
                RegDate = DateTime.Now,
                RST_ReportReestr = report,
                ReasonId = report.ReasonId,               
                Note = "",
                Expectant = report.Expectant
            };
            AppContext.RST_ReestrReportHistory.Add(history);
            AppContext.SaveChanges();
        }              

        public void UpdateState(long recordId, long? fieldValue, long? userId)
        {
            var report = AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == recordId);
            if (report == null)
            {
                return;
            }

            report.authorid = MyExtensions.GetCurrentUserId();
            report.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (fieldValue == CodeConstManager.RST_STATUS_EXCLUDED_ID)
            {
          
                report.IsExcluded = true;
            }
            else
            {
                report.StatusId = fieldValue;
            }

            var history = new RST_ReestrReportHistory
            {
                StatusId = fieldValue,
                Author = userId,
                UserId = report.UserId,
                Oblast = report.Oblast,
                ReportYear = report.RST_Report.ReportYear,
                RegDate = DateTime.Now,
                RST_ReportReestr = report,
                Note = "Изменен статус",
                ReasonId = report.ReasonId,
                Expectant = report.Expectant              
            };
            AppContext.RST_ReestrReportHistory.Add(history);
            AppContext.SaveChanges();
        }

        public RST_Executed GetExecutedByReportYear(int? year)
        {
            if (year == null)
            {
                return null;
            }
            return AppContext.RST_Executed.FirstOrDefault(e => e.ReportYear == year && !e.IsDeleted);
        }

        public bool UpdateOrderInfo(int reportYear, long userId, string fieldName, string fieldValue)
        {
            var newOrder = false;
            var titleEvent = "Приказ на исключение";
            var order = AppContext.RST_Executed.FirstOrDefault(e => e.ReportYear == reportYear && !e.IsDeleted) ??
                        new RST_Executed {CreateDate = DateTime.Now, ReportYear = reportYear, UserId = userId};
            switch (fieldName)
            {
                case "NumberOrder":
                {
                    order.NumberOrder = fieldValue; 
                    break;
                }
                case "DateOrderStr":
                {
                    order.DateOrderStr = fieldValue;
                    if (!string.IsNullOrEmpty(fieldValue) && order.DateOrderStr == null)
                    {
                        return false;
                    }
                    break;
                }
                case "Note":
                {
                    order.Note = fieldValue;
                    break;
                }
            }
            if (order.Id == 0)
            {
                newOrder = true;
                AppContext.RST_Executed.Add(order);
            }
            AppContext.SaveChanges();
             var user = new AccountRepository().GetUserById(userId);
            if (newOrder)
            {
                RegJurnalManager.Instance.AddObject(titleEvent, order.Id, typeof(RST_Executed).FullName, userId,
                    user.Login);
            }
            else
            {
                RegJurnalManager.Instance.EditObject(titleEvent, order.Id, typeof(RST_Executed).FullName, userId,
                   user.Login);
            }

            return true;
        }

        public List<RST_ReestrReportHistory> GetReestrReportByUserId(long id)
        {
            return AppContext.RST_ReestrReportHistory.Where(e => e.UserId == id).ToList();

        }

		public void RegistredYear(RST_Report model, long? getCurrentUserId)
		{
			var lastYear = AppContext.RST_Report.Where(e => !e.IsDeleted).Max(e => e.ReportYear);
			SaveOrUpdate(model, getCurrentUserId);
			var where =
				"INSERT INTO   public.\"RST_ReportReestr\" (\"ReportId\", \"ReestrId\",\"Address\",\"BINIIN\",\"OwnerName\", "
				+ " \"StatusId\",\"EditDate\",\"UserId\",\"IsDeleted\",\"Oblast\",\"IDK\",\"Note\",\"Editor\",\"IsExcluded\",usrfirstname,usrlastname,usrsecondname,usrjuridicalname "
				+ ",usrpost,usrmobile,usrworkphone,usrinternalphone,usraddress,usriscvazy,usrresponcefio,usrresponcepost,usroblast,usrregion,usrsubregion,usrvillage,usrtypeapplicationid,usrokedid,usrfscode,usridk )"
				+ " (select " + model.Id + ", r.\"ReestrId\",r.\"Address\",r.\"BINIIN\",r.\"OwnerName\", "
				+ " 4,r.\"EditDate\",r.\"UserId\",r.\"IsDeleted\",r.\"Oblast\",r.\"IDK\",r.\"Note\",r.\"Editor\", 'false', r.usrfirstname,r.usrlastname,r.usrsecondname,r.usrjuridicalname "
				+ " , r.usrpost,r.usrmobile,r.usrworkphone,r.usrinternalphone,r.usraddress,r.usriscvazy,r.usrresponcefio,r.usrresponcepost,r.usroblast,r.usrregion,r.usrsubregion,r.usrvillage,r.usrtypeapplicationid,r.usrokedid,r.usrfscode,r.usridk "
				+ " from public.\"RST_ReportReestr\" as r inner join public.\"RST_Report\" as rr on rr.\"Id\"=r.\"ReportId\" where rr.\"ReportYear\"=" + lastYear + " and r.\"StatusId\"<>2 and   r.\"IsExcluded\"=FALSE )";

			//----old query
			//var where =
			//	"INSERT INTO   public.\"RST_ReportReestr\" (\"ReportId\", \"ReestrId\",\"Address\",\"BINIIN\",\"OwnerName\", " +
			//	" \"StatusId\",\"EditDate\",\"UserId\",\"IsDeleted\",\"Oblast\",\"IDK\",\"Note\",\"Editor\",\"IsExcluded\")" +
			//	" (select " + model.Id + ", r.\"ReestrId\",r.\"Address\",r.\"BINIIN\",r.\"OwnerName\", " +
			//	" 4,r.\"EditDate\",r.\"UserId\",r.\"IsDeleted\",r.\"Oblast\",r.\"IDK\",r.\"Note\",r.\"Editor\", 'false' from public.\"RST_ReportReestr\" as r inner join public.\"RST_Report\" as rr on rr.\"Id\"=r.\"ReportId\" where rr.\"ReportYear\"=" + lastYear + " and r.\"StatusId\"<>2 and   r.\"IsExcluded\"=FALSE )"; 
			
			AppContext.Database.ExecuteSqlCommand(where);

			var history =
				"INSERT INTO public.\"RST_ReestrReportHistory\" (\"RegDate\",\"UserId\",\"Note\",\"StatusId\",\"ReestrId\",\"ReasonId\",\"ReportYear\",\"Oblast\",\"Expectant\",\"Author\")  ( " +
				"select '" + DateTime.Now.ToShortDateString() + "',  r.\"UserId\", 'Перенесен на следующий год',r.\"StatusId\", r.\"Id\", r.\"ReasonId\", rr.\"ReportYear\", r.\"Oblast\", r.\"Expectant\"," + getCurrentUserId + " from public.\"RST_ReportReestr\" as r inner join public.\"RST_Report\" as rr on rr.\"Id\"=r.\"ReportId\" where rr.\"ReportYear\"=" + model.ReportYear + ")  ";
			AppContext.Database.ExecuteSqlCommand(history);
		}

        public bool CheckHaveInReestr(long id, int year)
        {
            return AppContext.RST_ReportReestr.Any(e => e.UserId == id && e.RST_Report.ReportYear == year && !e.IsDeleted);
        }

        public void DeleteReestr(long id, long? getCurrentUserId)
        {
            var reest = AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == id);
            if (reest == null)
            {
                return;
            }
            var user = new AccountRepository().GetUserById(getCurrentUserId);
            string userName = null;
            if (user != null)
            {
                userName = user.Login;
            }
            reest.IsDeleted = true;
            AppContext.SaveChanges();
            RegJurnalManager.Instance.DelObject("Реестр", id, typeof(RST_ReportReestr).FullName, getCurrentUserId, userName);
        }

		public RST_ReportReestr GetRecordByUserIdVSReportYear(long userid, int? reportyear)
		{
			var item = AppContext.Database.SqlQuery<RST_ReportReestr>("select * from \"RST_ReportReestr\" t , \"RST_Report\" t1 where  t.\"ReportId\"=t1.\"Id\"  and t.\"UserId\"=" + userid + " and t1.\"ReportYear\"=" + reportyear).FirstOrDefault();
			return item;
		}

		public void UpdateReportReestrByUserIdVSReportYear(RST_ReportReestr row)
		{
			string err = "";
			try
			{
				var entity = AppContext.RST_ReportReestr.Find(row.Id);//FirstOrDefault(e => e.Id == row.Id);
				if (entity == null)
				{
					return;
				}

				entity.usrlastname = row.usrlastname;
				entity.usrsecondname = row.usrsecondname;
				entity.usrfirstname = row.usrfirstname;
				entity.usrjuridicalname = row.usrjuridicalname;
				entity.usrpost = row.usrpost;
				entity.usrmobile = row.usrmobile;
				entity.usrworkphone = row.usrworkphone;
				entity.usrinternalphone = row.usrinternalphone;
				entity.usraddress = row.usraddress;

				entity.usriscvazy = row.usriscvazy;
				entity.usrresponcefio = row.usrresponcefio;
				entity.usrresponcepost = row.usrresponcepost;
				entity.usroblast = row.usroblast;
				
				if (row.usrregion != 0)
					entity.usrregion = row.usrregion;

				entity.usrsubregion = row.usrsubregion;
				entity.usrvillage = row.usrvillage;
				entity.usrtypeapplicationid = row.usrtypeapplicationid;
				entity.usrokedid = row.usrokedid;
				entity.usridk = row.usridk;
                entity.usremail = row.usremail;

                var _dicTypeApplication = AppContext.DIC_TypeApplication.FirstOrDefault(x => x.Id == row.usrtypeapplicationid);
                if (_dicTypeApplication.Code.Equals("юр"))
                    entity.usrfscode = 1;
                else if (_dicTypeApplication.Code.Equals("кв"))
                    entity.usrfscode = 2;
                else if (_dicTypeApplication.Code.Equals("гу"))
                    entity.usrfscode = 3;
                else entity.usrfscode = null;


                if (row.usrfscode != null)
                {
                    entity.usrfscode = row.usrfscode;
                 
                    if (entity.usrfscode == 2)
                        entity.usriscvazy = true;
                }

                //----update sec_user
                var secUser = AppContext.SEC_User.FirstOrDefault(x => x.Id == entity.UserId);
                secUser.ConfirmPwd = secUser.Pwd;
                secUser.LastName = row.usrlastname;
                secUser.SecondName = row.usrsecondname;
                secUser.FirstName = row.usrfirstname;
                secUser.JuridicalName = row.usrjuridicalname;
                secUser.Post = row.usrpost;
                secUser.Mobile = row.usrmobile;
                secUser.WorkPhone = row.usrworkphone;
                secUser.InternalPhone = row.usrinternalphone;
                secUser.Address = row.usraddress;                             
                secUser.ResponceFIO = row.usrresponcefio;
                secUser.ResponcePost = row.usrresponcepost;
                secUser.Oblast = row.usroblast;
                secUser.Region = row.usrregion;
                secUser.SubRegion = row.usrsubregion;
                secUser.Village = row.usrvillage;

                if (entity.usrfscode == 2)
                    secUser.IsCvazy = true;

                if (entity.usrtypeapplicationid != null)
                    secUser.TypeApplicationId = entity.usrtypeapplicationid.Value;

                secUser.OkedId = row.usrokedid;
                secUser.IDK = row.usridk;
                secUser.Email = row.usremail;
                secUser.FSCode = entity.usrfscode;

                int result = AppContext.SaveChanges();
			}
			catch (Exception ex) {
				err = ex.Message;
			}

		}

		public void UpdateReportReestr(SEC_Guest model,int year)
		{
            string rst_query = " SELECT rst.* from \"RST_ReportReestr\" as rst  "
                              + " inner join \"RST_Report\" as r on rst.\"ReportId\" = r.\"Id\" "
                              + " where \"BINIIN\" = '" + model.BINIIN + "' and r.\"ReportYear\" =" + year + " ";

			var row = AppContext.Database.SqlQuery<RST_ReportReestr>(rst_query).FirstOrDefault();
			var rst_reestr = AppContext.Database.SqlQuery<RST_Reestr>("select * from \"RST_Reestr\"  where \"BINIIN\"='" + model.BINIIN + "' ").FirstOrDefault();
			if (row != null)
			{
				row.usrlastname = model.LastName;
				row.usrsecondname = model.SecondName;
				row.usrfirstname = model.FirstName;
				row.usrjuridicalname = model.JuridicalName;
				row.usrpost = model.Post;
				row.usrmobile = model.Mobile;
				row.usrworkphone = model.WorkPhone;
				row.usrinternalphone = model.InternalPhone;
				row.usraddress = model.Address;

				row.usriscvazy = model.IsCvazy;
				row.usrresponcefio = model.ResponceFIO;
				row.usrresponcepost = model.ResponcePost;
				row.usroblast = model.Oblast;
				row.usrregion = model.Region;
				row.usrsubregion = model.SubRegion;
				row.usrvillage = model.Village;
				row.usrtypeapplicationid = model.TypeApplicationId;
				row.usrokedid = model.OkedId;
				row.usridk = model.IDK;
                row.usremail = model.Email;

                var _dicTypeApplication = AppContext.DIC_TypeApplication.FirstOrDefault(x => x.Id == model.TypeApplicationId);
                if (_dicTypeApplication.Code.Equals("юр"))
                    row.usrfscode = 1;
                else if (_dicTypeApplication.Code.Equals("кв"))
                    row.usrfscode = 2;
                else if (_dicTypeApplication.Code.Equals("гу"))
                    row.usrfscode = 3;
                else row.usrfscode = null;

                if (model.IsCvazy == true)
                    row.usrfscode = 2;
         
				if (rst_reestr != null)
					row.ReestrId = rst_reestr.Id;

				var attachedEntity = AppContext.Set<RST_ReportReestr>().Find(row.Id);
				AppContext.Entry(attachedEntity).CurrentValues.SetValues(row);
				AppContext.Commit(true);
			}
		}

		public string UpdateRstReportIsActive(long id)
		{
			string errorMessage = "";
			try
			{
				AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  isactive=false ");
				AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  isactive=true  WHERE \"Id\"=" + id);

			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
			}
			return errorMessage;
		}

        public string UpdateRstReportIsEditSubjectReport(long id,bool isedit)
        {
            string errorMessage = "";
            try
            {
                if (isedit)
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsEditSubjectReport\"=true  WHERE \"Id\"=" + id);
                }
                else
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsEditSubjectReport\"=false  WHERE \"Id\"=" + id);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }

        public string UpdateRstReportIsCreateSubjectReport(long id, bool iscreate)
        {
            string errorMessage = "";
            try
            {
                if (iscreate)
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsCreateSubjectReport\"=true  WHERE \"Id\"=" + id);
                }
                else
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsCreateSubjectReport\"=false  WHERE \"Id\"=" + id);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }

        public string UpdateRstReportIsEditSubjectReportByManager(long id, bool isedit)
        {
            string errorMessage = "";
            try
            {
                if (isedit)
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsEditSubjectReportByManager\"=true  WHERE \"Id\"=" + id);
                }
                else
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsEditSubjectReportByManager\"=false  WHERE \"Id\"=" + id);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }

        public string UpdateRstReportIsStatisticMainPageByManager(long id, bool isedit)
        {
            string errorMessage = "";
            try
            {
                if (isedit)
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsStatisticMainPage\"=true  WHERE \"Id\"=" + id);
                }
                else
                {
                    AppContext.Database.ExecuteSqlCommand("UPDATE \"RST_Report\"  SET  \"IsStatisticMainPage\"=false  WHERE \"Id\"=" + id);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }
        //----addtional
        public string GetRstReportReestrPageCount(ref int allCount, int year, int calcDeep, string oblast_ids)
		{
			string ErrorMessage = "";
			try
			{
				//----query
				string query = GetQuery(year,calcDeep);

				if (!oblast_ids.Equals("-1"))
				{
					query += " and rr.\"Oblast\" in (" + replaceToComma(oblast_ids) + ")";
				}

				//----order by
				query += " order by rr.\"Id\"  ";

				var rows = AppContext.Database.SqlQuery<RST_ReportReestr>(query).ToList();
				if (rows != null)
					allCount = rows.Count;

			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
			return ErrorMessage;
		}

		public string GetRstReportReestr(ref List<RST_ReportReestrClass> ListItems, int PageNum, int year, int calcDeep, string oblast_ids, string name_idk_bin)
		{
			string ErrorMessage = "";
			try
			{
				string query = GetQuery(year,calcDeep);

				if (!oblast_ids.Equals("-1"))
				{
					query += " and rr.\"Oblast\" in (" + replaceToComma(oblast_ids) + ")";
				}

				//----окэд идк наименование
				if (!string.IsNullOrWhiteSpace(name_idk_bin))
					query += " and ( upper(rr.\"IDK\") like '%" + name_idk_bin.ToUpper() + "%'  or upper(rr.\"OwnerName\") like '%" + name_idk_bin.ToUpper() + "%' or upper(rr.\"BINIIN\") like '%" + name_idk_bin.ToUpper() + "%' ) ";
				
				//----order by
				query += " order by rr.\"Id\"  ";

				int offset = 0;
				if (PageNum > 1)
				{
					offset = (PageNum - 1) * 50;
				}

				if (PageNum != 0)
					query = "select f.* from  (" + query + ") f LIMIT 50 OFFSET " + offset;

				var rows = AppContext.Database.SqlQuery<RST_ReportReestrClass>(query).ToList();
				if (rows != null)
					ListItems = rows;
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public string GetQuery(int year, int calcDeep)
		{
			string lang = CultureHelper.GetCurrentCulture();

			#region query
			string query = " select rr.*  ,   case '" + lang + "' when 'kz' then k.\"NameKz\" else k.\"NameRu\" end as \"OblastName\"  "
								+ " from \"RST_ReportReestr\" rr        "
								+ " inner join \"DIC_Kato\" k on k.\"Id\"=rr.\"Oblast\"  "
								+ " INNER join (        "
								+ "   select t.*      "
								+ " from  \"RST_ReportReestr\" t , \"RST_Report\" t5, \"SUB_Form\" f        "
								+ "   where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=" + year + "     "
								+ " and f.\"Id\"=t.\"FormId\" and f.\"IsPlan\" is not true        "
								+ " ) as t00 on t00.\"Id\"=rr.\"Id\"        ";

			for (int i = 1; i <calcDeep; i++)
			{
				query += " INNER join (        "
				+ "   select t.*      "
				+ "   from  \"RST_ReportReestr\" t , \"RST_Report\" t5, \"SUB_Form\" f      "
				+ "   where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=" + (year - i) + "     "
				+ " and f.\"Id\"=t.\"FormId\" and f.\"IsPlan\" is not true        "
				+ " ) as t0" + i + " on t0" + i + ".\"UserId\"=rr.\"UserId\"        ";

			}

			query += " where 1=1 ";
			/*
				+" INNER join (        "
				+ "   select t.*      "
				+ " from  \"RST_ReportReestr\" t , \"RST_Report\" t5, \"SUB_Form\" f        "
				+ "   where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=2016     "
				+ " and f.\"Id\"=t.\"FormId\" and f.\"IsPlan\" is not true        "
				+ " ) as t16 on t16.\"Id\"=rr.\"Id\"        "
				+ " INNER join (        "
				+ "   select t.*      "
				+ "   from  \"RST_ReportReestr\" t , \"RST_Report\" t5, \"SUB_Form\" f      "
				+ "   where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=2015     "
				+ " and f.\"Id\"=t.\"FormId\" and f.\"IsPlan\" is not true        "
				+ " ) as t15 on t15.\"UserId\"=rr.\"UserId\"        "
				+ " INNER join (        "
				+ "   select t.*      "
				+ "   from  \"RST_ReportReestr\" t , \"RST_Report\" t5, \"SUB_Form\" f      "
				+ "   where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=2014     "
				+ " and f.\"Id\"=t.\"FormId\" and f.\"IsPlan\" is not true        "
				+ " ) as t14 on t14.\"UserId\"=rr.\"UserId\"        "
				+ " INNER join (        "
				+ "   select t.*      "
				+ "   from  \"RST_ReportReestr\" t , \"RST_Report\" t5, \"SUB_Form\" f      "
				+ "   where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=2013     "
				+ " and f.\"Id\"=t.\"FormId\" and f.\"IsPlan\" is not true        "
				+ " ) as t13 on t13.\"UserId\"=rr.\"UserId\"        "
				+ " INNER join (        "
				+ "   select t.*      "
				+ "   from  \"RST_ReportReestr\" t , \"RST_Report\" t5, \"SUB_Form\" f      "
				+ "   where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=2012     "
				+ " and f.\"Id\"=t.\"FormId\" and f.\"IsPlan\" is not true        "
				+ " ) as t12 on t12.\"UserId\"=rr.\"UserId\"        ";*/
			#endregion

			return query;
		}

		//---- helper function
		public string replaceToComma(string val)
		{

			while (val.IndexOf("*") != -1)
			{
				val = val.Replace("*", ",");
			}
			return val;
		}


        //----upload pdf
        public List<Pdf_Region> GetOblastList(int year)
        {
            string sql = " select * from f_get_pdf_by_obl(" + year + ") ";
            var rows = AppContext.Database.SqlQuery<Pdf_Region>(sql).ToList();
            return rows;
        }

        public List<Pdf_Subject> GetReportListByOblId(int year, long oblastId)
        {
            string sql = "select * from f_get_pdf_subj_obl("+year+"," + oblastId + ");";
            var rows = AppContext.Database.SqlQuery<Pdf_Subject>(sql).ToList();
            return rows;
        }

        public void WriteToPdfResult(long rst_id,string fileName,string errorMessage="")
        {
            //string sql = (string.IsNullOrEmpty(errorMessage)) ? "select * from f_get_pdf_save_result(" + rst_id + ", '" + fileName + "',null);"
            //                                        : "select * from f_get_pdf_save_result(419404,null,'" + errorMessage + "');";
            
            string sql = string.Empty;
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                sql = "update \"RST_ReportReestr\" set pdf_stat=true, pdf_gen_date=now(), pdf_filename='" + fileName + "', pdf_gen_err=null where \"Id\"=" + rst_id + ";";
            }
            else
            {
                sql = "update \"RST_ReportReestr\" set pdf_stat=false, pdf_gen_date=now(), pdf_filename=null, pdf_gen_err='" + errorMessage + "' where \"Id\"=" + rst_id + ";";
            }

            var result = AppContext.Database.ExecuteSqlCommand(sql);            
        }

        public void ClearPdfFiles(int year, long oblastId)
        {
            string sql = " update \"RST_ReportReestr\" set pdf_filename=NULL, pdf_gen_date=NULL,  pdf_gen_err = NULL, pdf_stat = NULL  "
                    + "  where \"Id\" in (select r.\"Id\" from \"RST_ReportReestr\" as r  "
                    + " inner join \"RST_Report\" as rr on r.\"ReportId\"=rr.\"Id\" "
                    + " where rr.\"ReportYear\"=" + year + " and r.\"IsDeleted\"=false and r.\"Oblast\"=" + oblastId + "); ";

            sql += " update upload_pdf_state set startdate=null , enddate=null,finished=false where oblast_id=" + oblastId + ";";
            var result = AppContext.Database.ExecuteSqlCommand(sql);
        }

        public List<Pdf_State> GetUploadPdfStateList()
        {
            string sql = "select * from upload_pdf_state ";
            var rows = AppContext.Database.SqlQuery<Pdf_State>(sql).ToList();
            return rows;
        }

        public void WriteUploadPdfState(long oblastId,string type)
        {
            var list = GetUploadPdfStateList();
            var item = list.Find(x => x.oblast_id == oblastId);
            string sql = string.Empty;
            if (item == null)
            {
                sql = " insert into upload_pdf_state(oblast_id,startdate,enddate,finished) values(" + oblastId + ",now(),null,false);";
            }
            else
            {
                sql = (type.Equals("start")) ? " update upload_pdf_state set startdate=now(), enddate=null, finished=false where oblast_id=" + oblastId
                    : " update upload_pdf_state set enddate=now(), finished=true where oblast_id=" + oblastId;
            }

            var result = AppContext.Database.ExecuteSqlCommand(sql);
        }

        public bool IsFinished(long oblastId)
        {
            bool result = true;
            string sql = "select * from upload_pdf_state where oblast_id=" + oblastId;
            var row = AppContext.Database.SqlQuery<Pdf_State>(sql).FirstOrDefault();
            if (row != null)
            {
                if (row.startdate != null && row.enddate == null)
                    result = false;
            }

            return result;
        }
    }
}