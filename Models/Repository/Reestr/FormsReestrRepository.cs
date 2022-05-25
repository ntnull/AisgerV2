using Aisger.Models.Repository.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Reestr
{
    public class FormsReestrRepository
    {
        Common common = new Common();

        //----
        public Dictionary<string, object> getFormsReestr(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                + " case '" + lang + "' "
                + " when 'ru' then status.\"NameRu\" "
                + " when 'kz' then status.\"NameKz\" "
                + " else status.\"NameRu\" "
                + " end "
                 + " as status_name,"
                + " ("
                + " select count(distinct u.\"Id\") "
                + " from \"RST_ReportReestr\" rr inner join "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                + " and u.\"IsDeleted\" = false "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" = '" + year + "'  and u.\"FSCode\" = 3 and rr.\"StatusId\" = status.\"Id\" "
                + " ) as state_inst_qty, "
                + " ( "
                + " select count(distinct u.\"Id\") "
                + " from \"RST_ReportReestr\" rr inner join "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                + " and u.\"IsDeleted\" = false "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" = '" + year + "'  and u.\"FSCode\" = 2 and rr.\"StatusId\" = status.\"Id\" "
                + " ) as quasi_state_inst_qty, "
                + " ( "
                + " select count(distinct u.\"Id\") "
                + " from \"RST_ReportReestr\" rr inner join "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                + " and u.\"IsDeleted\" = false "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" = '" + year + "'  and u.\"FSCode\" = 1 and rr.\"StatusId\" = status.\"Id\" "
                + " ) as jur_qty "
                + " from "
                + " \"RST_DIC_Status\" status "
                + " where "
                + " status.\"IsDeleted\" = false "
                + " order by status.\"Code\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getGerSubjectsReestrByObl(int year, int? oblastId, string lang = "ru")
        {
            #region query
            string obl = oblastId.ToString();
            if (oblastId == 0)
                obl = "null";
            string query = " select "
                + " case '" + lang + "' "
                + " when 'ru' then kato.\"NameRu\" "
                + " when 'kz' then kato.\"NameKz\" "
                + " else kato.\"NameRu\" "
                + " end as oblast_name, "
                + " case '" + lang + "' "
                + " when 'ru' then status.\"NameRu\" "
                + " when 'kz' then status.\"NameKz\" "
                + " else status.\"NameRu\" "
                + " end as status_name, "
                + " ( "
                + " select count(distinct u.\"Id\") "
                + " from \"RST_ReportReestr\" rr inner join "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and kato.\"IsDeleted\" = false "
                + " and u.\"IsDeleted\" = false "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 3 and rr.\"StatusId\" = status.\"Id\" and rr.\"Oblast\" = kato.\"Id\" "
                + " ) as state_inst_qty, "
                + " ( "
                + " select count(distinct u.\"Id\") "
                + " from \"RST_ReportReestr\" rr inner join "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and kato.\"IsDeleted\" = false "
                + " and u.\"IsDeleted\" = false "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 2 and rr.\"StatusId\" = status.\"Id\" and rr.\"Oblast\" = kato.\"Id\" "
                + " ) as quasi_state_inst_qty, "
                + " ( "
                + " select count(distinct u.\"Id\") "
                + " from \"RST_ReportReestr\" rr inner join "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and kato.\"IsDeleted\" = false "
                + " and u.\"IsDeleted\" = false "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 1 and rr.\"StatusId\" = status.\"Id\" and rr.\"Oblast\" = kato.\"Id\" "
                + " ) as jur_qty, "
                + " (select count(distinct u.\"Id\")  from \"RST_ReportReestr\" rr inner join  \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" "
                + " inner join  \"SEC_User\" u on rr.\"UserId\" = u.\"Id\"  where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                + " and kato.\"IsDeleted\" = false  and u.\"IsDeleted\" = false  and rr.\"IsExcluded\" = false  and r.\"ReportYear\"="+year
                + " and (u.\"FSCode\" is null or u.\"FSCode\">3)  and rr.\"StatusId\" = status.\"Id\" and rr.\"Oblast\" = kato.\"Id\" ) as other_qty "
                + "  "
                + " from "
                + " \"DIC_Kato\" kato, "
                + " \"RST_DIC_Status\" status "
                + " where "
                + " kato.\"refParent\"=1 "
                + " and "
                + " (kato.\"Id\" = " + obl + " or " + obl + " is null) "
                + " and kato.\"IsDeleted\" = false and status.\"IsDeleted\" = false "
                + " order by "
                + " kato.\"Code\", status.\"Code\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getSubjectsReestrOffReasonByRepublic(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                              + " case '" + lang + "' "
                              + " when 'ru' then rs.\"NameRu\" "
                              + " when 'kz' then rs.\"NameKz\" "
                              + " else rs.\"NameRu\" "
                              + " end reason_name, "
                              + " ( "
                              + " select count(distinct u.\"Id\") "
                              + " from \"RST_ReportReestr\" rr inner join "
                              + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                              + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                              + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                              + " and u.\"IsDeleted\" = false "
                              + " and rr.\"IsExcluded\" = true "
                              + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 3 and rr.\"Expectant\" = rs.\"Id\" "
                              + " ) as state_inst_qty, "
                              + " ( "
                              + " select count(distinct u.\"Id\") "
                              + " from \"RST_ReportReestr\" rr inner join "
                              + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                              + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                              + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                              + " and u.\"IsDeleted\" = false "
                              + " and rr.\"IsExcluded\" = true "
                              + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 2 and rr.\"Expectant\" = rs.\"Id\" "
                              + " ) as quasi_state_inst_qty, "
                              + " ( "
                              + " select count(distinct u.\"Id\") "
                              + " from \"RST_ReportReestr\" rr inner join "
                              + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                              + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                              + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                              + " and u.\"IsDeleted\" = false "
                              + " and rr.\"IsExcluded\" = true "
                              + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 1  and rr.\"Expectant\" = rs.\"Id\" "
                              + " ) as jur_qty "
                              + " from "
                              + " \"RST_DIC_Reason\" rs "
                              + " where "
                              + " rs.\"IsExcluded\" = true "
                              + " and "
                              + " rs.\"IsDeleted\" = false "
                              + " order by rs.\"Code\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getGerSubjectsReestrOffReasonByObl(int year, int? oblastId, string lang = "ru")
        {
            #region query
            string obl = oblastId.ToString();
            if (oblastId == 0)
                obl = "null";
            string query = " select "
                            + " case '" + lang + "' "
                            + " when 'ru' then kato.\"NameRu\" "
                            + " when 'kz' then kato.\"NameKz\" "
                            + " else kato.\"NameRu\" "
                            + " end as oblast_name, "
                            + " case '" + lang + "' "
                            + " when 'ru' then rs.\"NameRu\" "
                            + "  when 'kz' then rs.\"NameKz\" "
                            + " else rs.\"NameRu\" "
                            + " end as reason_name, "
                            + " ( "
                            + " select count(distinct u.\"Id\") "
                            + " from \"RST_ReportReestr\" rr inner join "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                            + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                            + " and u.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = true "
                            + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 3 and rr.\"Expectant\" = rs.\"Id\" and rr.\"Oblast\" = kato.\"Id\" "
                            + " ) as state_inst_qty, "
                            + " ( "
                            + " select count(distinct u.\"Id\") "
                            + " from \"RST_ReportReestr\" rr inner join "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                            + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                            + " and u.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = true "
                            + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 2 and rr.\"Expectant\" = rs.\"Id\" and rr.\"Oblast\" = kato.\"Id\" "
                            + " ) as quasi_state_inst_qty, "
                            + " ( "
                            + " select count(distinct u.\"Id\") "
                            + " from \"RST_ReportReestr\" rr inner join "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" "
                            + " where rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                            + " and u.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = true "
                            + " and r.\"ReportYear\" = " + year + "  and u.\"FSCode\" = 1 and rr.\"Expectant\" = rs.\"Id\"  and rr.\"Oblast\" = kato.\"Id\" "
                            + " ) as jur_qty "
                            + " from "
                            + " \"DIC_Kato\" kato, "
                            + " \"RST_DIC_Reason\" rs "
                            + " where "
                            + " rs.\"IsExcluded\" = true "
                            + " and "
                            + " kato.\"refParent\"=1 "
                            + " and "
                            + " (kato.\"Id\" =" + obl + " or " + obl + " is null) "
                            + " and "
                            + " kato.\"IsDeleted\" = false "
                            + " and "
                            + " rs.\"IsDeleted\" = false "
                            + " order by kato.\"Code\", rs.\"Code\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getEnergyAudit(string beginDate, string endDate)
        {

            string query = " select "
                            + " u.\"JuridicalName\" as auditor_name, "
                            + " u.\"BINIIN\" as bin, "
                            + " u.\"Address\" as address, "
                            + " 0 as qty "
                            + " from "
                            + " \"SEC_User\" u "
                            + " where "
                            + " u.\"Id\" in ( "
                            + " select uk.\"UserId\" from \"SEC_UserKind\" uk where uk.\"KindId\"=2 "
                            + " ) "
                            + " and "
                            + " u.\"CreateDate\" between  TO_DATE('" + beginDate + "', 'DD.MM.YYYY')" + " and TO_DATE('" + endDate + "', 'DD.MM.YYYY') ";
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getEnergyService(string beginDate, string endDate)
        {

            string query = " select "
                            + " u.\"JuridicalName\" as company_name, "
                            + " u.\"BINIIN\" as bin, "
                            + " u.\"Address\" as address "
                            + " from "
                            + " \"SEC_User\" u "
                            + " where "
                            + " u.\"Id\" in ( "
                            + " select uk.\"UserId\" from \"SEC_UserKind\" uk where uk.\"KindId\"=3 "
                            + " ) "
                            + " and "
                            + " u.\"CreateDate\" between  TO_DATE('" + beginDate + "', 'DD.MM.YYYY')" + " and TO_DATE('" + endDate + "', 'DD.MM.YYYY') ";
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getGerEsko(string beginDate, string endDate)
        {

            string query = " select "
                            + " 'Компания 2' as esko_name, "
                            + " 'Продукт 1' as product_name, "
                            + " 'Группа 1' as group_name, "
                            + " 'Аудитория 1' as target_group ";
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getAuditCount(int year)
        {

            string query = " select "
                            + " u.\"JuridicalName\" auditor_name, "
                            + " count(distinct p.\"Id\") as audit_count "
                            + " from "
                            + " \"EAUDIT_Preamble\" p inner join "
                            + " \"SEC_User\" u on p.\"refAuditor\" = u.\"Id\" "
                            + " where "
                            + " p.\"ReportYear\" = " + year + " "
                            + " group by "
                            + " u.\"Id\" ";
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getEnergyMapRequests(string beginDate, string endDate, string lang = "ru")
        {

            string query = " select "
                            + " case '" + lang + "' "
                            + " when 'ru' then m.month_name "
                            + " when 'kz' then m.month_name_kz "
                            + " else m.month_name end ||' '|| cast(extract (year from a.\"SendDate\") as varchar) as month_name, "
                            + " sum(case when a.\"StatusId\" != 1 then 1 else 0 end) as all_count, "
                            + " sum(case when a.\"StatusId\" in (4,6) then 1 else 0 end) as reject_count, "
                            + " sum(case when a.\"StatusId\" in (2,3,5,7) then 1 else 0 end) as wait_count, "
                            + " sum(case when a.\"StatusId\" in (8) then 1 else 0 end) as accept_count "
                            + " from "
                            + " \"MAP_Application\" a inner join "
                            + " v_months m on extract (month from a.\"SendDate\") = m.month_id "
                            + " where "
                            + " a.\"IsDeleted\" = false "
                            + " and "
                            + " a.\"SendDate\" between TO_DATE('" + beginDate + "', 'DD.MM.YYYY')" + " and TO_DATE('" + endDate + "', 'DD.MM.YYYY') "
                            + " group by "
                            + " m.month_id, "
                            + " m.month_name, "
                            + " m.month_name_kz, "
                            + " extract (year from a.\"SendDate\") "
                            + " order by m.month_id ";
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> getEnergyMapProjects(string beginDate, string endDate, string lang = "ru")
        {

            string query = " select "
                            + " case '" + lang + "' "
                            + " when 'ru' then m.month_name "
                            + " when 'kz' then m.month_name_kz "
                            + " else m.month_name end ||' '|| cast(extract (year from p.\"CreateDate\") as varchar) as month_name, "
                            + " count(distinct p.\"Id\") as project_count, "
                            + " sum(a.\"TotalCost\") total_cost, "
                            + " (select count(*) from \"MAP_Project\" where \"IsDeleted\" = false) as all_project_count, "
                            + " (select sum(a2.\"TotalCost\") from \"MAP_Project\" p2 inner join "
                            + " \"MAP_Application\" a2 on p2.\"ApplicationId\" = a2.\"Id\" where p2.\"IsDeleted\" = false and a2.\"IsDeleted\" = false ) as all_project_sum "
                            + " from "
                            + " \"MAP_Project\" p inner join "
                            + " \"MAP_Application\" a on p.\"ApplicationId\" = a.\"Id\" inner join "
                            + " v_months m on extract (month from p.\"CreateDate\") = m.month_id "
                            + " where "
                            + " p.\"IsDeleted\" = false "
                            + " and "
                            + " a.\"IsDeleted\" = false "
                            + " and "
                            + " p.\"CreateDate\" between  TO_DATE('" + beginDate + "', 'DD.MM.YYYY')" + " and  TO_DATE('" + endDate + "', 'DD.MM.YYYY') "
                            + " group by "
                            + " m.month_id, "
                            + " m.month_name, "
                            + " m.month_name_kz, "
                            + " extract (year from p.\"CreateDate\") "
                            + " order by "
                            + " month_id ";
            var item = common.getTableList(query);
            return item;
        }    
   
    }
}