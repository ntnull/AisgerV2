using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Report
{
    public class ReportAnalyseRepository
    {
        Common common = new Common();
        //----
        public Dictionary<string, object> report2(string years, int oblastId = -1, string lang = "ru")
        {
            #region query
            string query = " select "
                         + " tr.\"Id\" as resource_id , "
                         + " case '" + lang + "'"
                         + " when 'ru' then tr.\"NameRu\" "
                         + " when 'kz' then tr.\"NameKz\" "
                         + " else tr.\"NameRu\" end as resource_name, "
                         + " r.\"ReportYear\" as report_year, "
                         + " sum( "
                         + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) "
                         + " ) as consumption "
                         + " from "
                         + " \"RST_ReportReestr\" rr inner join "
                         + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                         + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                         + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                         + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                         + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                         + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                         + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                         + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                         + " where "
                         + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false "
                         + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                         + " and kato.\"IsDeleted\" = false  and rr.\"IsExcluded\" = false "
                         + " and "
                         + " tr.\"Id\" in ( "
                         + " select itr.\"Id\" "
                         + " from  "
                         + " \"SEC_User\" iu inner join "
                         + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                         + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                         + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                         + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                         + " and if.\"ReportYear\" in (" + years + ")"
                         + " and ((" + oblastId + " = -1) or (iu.\"Oblast\" = " + oblastId + ")) "
                         + " group by itr.\"Id\" "
                         + " order by sum( "
                         + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) "
                         + " ) desc limit 7 ) "
                         + " and r.\"ReportYear\" in (" + years + ") "
                         + " and ((" + oblastId + " = -1) or (u.\"Oblast\" = " + oblastId + ")) "
                         + " group by   tr.\"Id\", r.\"ReportYear\" "
                         + " union all "
                         + " select "
                         + " -1 as resource_id, "
                         + " case '" + lang + "'"
                         + " when 'ru' then 'Прочие' "
                         + " when 'kz' then 'Басқалары' "
                         + " else 'Прочие' end as resource_name, "
                         + " r.\"ReportYear\" as report_year, "
                         + " sum( "
                         + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)  "
                         + " ) as consumption "
                         + " from "
                         + " \"RST_ReportReestr\" rr inner join  "
                         + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                         + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                         + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                         + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                         + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                         + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                         + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                         + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                         + " where  "
                         + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                         + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false  "
                         + " and kato.\"IsDeleted\" = false and rr.\"IsExcluded\" = false "
                         + " and "
                         + " tr.\"Id\" not in ( "
                         + " select itr.\"Id\" "
                         + " from  "
                         + " \"SEC_User\" iu inner join "
                         + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                         + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                         + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                         + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                         + " and if.\"ReportYear\" in (" + years + ")"
                         + " and ((" + oblastId + " = -1) or (iu.\"Oblast\" = " + oblastId + ")) "
                         + " group by itr.\"Id\" "
                         + " order by sum( "
                         + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1)  "
                         + " ) desc limit 7 ) "
                         + " and r.\"ReportYear\" in (" + years + ") "
                         + " and ((" + oblastId + " = -1) or (u.\"Oblast\" = " + oblastId + ")) "
                         + " group by  r.\"ReportYear\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //----
        public Dictionary<string, object> reportTop10ByRes(decimal resource_id, int year, int oblastId)
        {
            #region
            string query = "select "
                            + "r.\"ReportYear\" as report_year, "
                            + " u.\"Id\" as subject_id, "
                            + " u.\"JuridicalName\" as subject_name, "
                            + " sum( "
                            + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1) "
                            + " else  "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                            + " ) as consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and r.\"ReportYear\" = " + year
                            + " and tr.\"Id\"=" + resource_id
                            + " and ((" + oblastId + " = -1)or(kato.\"Id\" = " + oblastId + ")) "
                            + " group by  r.\"ReportYear\",  u.\"Id\" "
                            + " order by consumption desc limit 10";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //----
        public Dictionary<string, object> reportResByRegion(decimal resource_id, int year, int oblastId, string lang = "ru")
        {
            #region query
            string query = "select  kato.\"Id\", "
                            + " case '" + lang + "'"
                            + " when 'ru' then kato.\"NameRu\" "
                            + " when 'kz' then kato.\"NameKz\" "
                            + " else kato.\"NameRu\" end as oblast_name, "
                            + " sum( "
                            + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                            + " else  "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                            + " ) as consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and r.\"ReportYear\" = " + year
                            + " and tr.\"Id\" = " + resource_id
                            + " and ((" + oblastId + " = -1)or(kato.\"Id\" = " + oblastId + ")) "
                            + " group by kato.\"Id\" "
                            + " order by consumption desc ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        #region report 2
        //---
        public Dictionary<string, object> reportConsByRes7ForPie(int oblast_id, int year, string lang)
        {
            #region query
            string query = " select "
                            + " tr.\"Id\" as resource_id, "
                            + " case '" + lang + "' "
                            + " when 'ru' then tr.\"NameRu\" "
                            + " when 'kz' then tr.\"NameKz\" "
                            + " else tr.\"NameRu\" end as resource_name, "
                            + " r.\"ReportYear\" as report_year, "
                            + " sum( "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)  "
                            + " ) as consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and "
                            + " tr.\"Id\" in ( "
                            + " select itr.\"Id\" "
                            + " from  "
                            + " \"SEC_User\" iu inner join "
                            + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                            + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                            + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                            + " and if.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + "= -1) or (iu.\"Oblast\" =" + oblast_id + ")) "
                            + " group by itr.\"Id\" "
                            + " order by sum( "
                            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1)  "
                            + " ) desc limit 6 "
                            + " ) "
                            + " and r.\"ReportYear\" =" + year
                            + " and ((" + oblast_id + " = -1) or (u.\"Oblast\" = " + oblast_id + ")) "
                            + " group by  "
                            + " tr.\"Id\", "
                            + " r.\"ReportYear\" "
                            + " union all "
                            + " select "
                            + " -1 as resource_id, "
                            + " case '" + lang + "' "
                            + " when 'ru' then 'Прочие' "
                            + " when 'kz' then 'Басқалары' "
                            + " else 'Прочие' end as resource_name, "
                            + " r.\"ReportYear\" as report_year, "
                            + " sum( "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)  "
                            + " ) as consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and "
                            + " tr.\"Id\" not in ( "
                            + " select itr.\"Id\" "
                            + " from  "
                            + " \"SEC_User\" iu inner join "
                            + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                            + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                            + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                            + " and if.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + " = -1) or (iu.\"Oblast\" =" + oblast_id + ")) "
                            + " group by itr.\"Id\" "
                            + " order by sum( "
                            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1)  "
                            + " ) desc limit 6 "
                            + " ) "
                            + " and r.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + " = -1) or (u.\"Oblast\" = " + oblast_id + ")) "
                            + " group by  "
                            + " r.\"ReportYear\"   ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //---
        public Dictionary<string, object> reportConsByRes7(int oblast_id, int year, string lang)
        {
            #region query
            string query = " select "
                            + " tr.\"Id\" as resource_id, "
                            + " case '" + lang + "' "
                            + " when 'ru' then tr.\"NameRu\" "
                            + " when 'kz' then tr.\"NameKz\" "
                            + " else tr.\"NameRu\" end as resource_name, "
                            + " r.\"ReportYear\" as report_year, "
                            + " sum( "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)  "
                            + " )/1000 as consumption, "
                            + " 1 as not_others "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and "
                            + " tr.\"Id\" not in ( "
                            + " select itr.\"Id\" "
                            + " from  "
                            + " \"SEC_User\" iu inner join "
                            + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                            + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                            + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                            + " and if.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + " = -1) or (iu.\"Oblast\" =" + oblast_id + ")) "
                            + " group by itr.\"Id\" "
                            + " order by sum( "
                            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1)  "
                            + " ) desc limit 1 "
                            + " ) "
                            + " and "
                            + " tr.\"Id\"  in ( "
                            + " select itr.\"Id\" "
                            + " from  "
                            + " \"SEC_User\" iu inner join "
                            + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                            + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                            + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                            + " and if.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + "= -1) or (iu.\"Oblast\" = " + oblast_id + ")) "
                            + " group by itr.\"Id\" "
                            + " order by sum( "
                            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1)  "
                            + " ) desc limit 7 "
                            + " ) "
                            + " and ((" + oblast_id + " = -1) or (u.\"Oblast\" = " + oblast_id + ")) "
                            + " group by  "
                            + " tr.\"Id\", "
                            + " r.\"ReportYear\" "
                            + " union all "
                            + " select "
                            + " -1 as resource_id, "
                            + " case '" + lang + "' "
                            + " when 'ru' then 'Прочие' "
                            + " when 'kz' then 'Басқалары' "
                            + " else 'Прочие' end as resource_name, "
                            + " r.\"ReportYear\" as report_year, "
                            + " sum( "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)  "
                            + " )/1000 as consumption, "
                            + " 0 as not_others "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and "
                            + " tr.\"Id\" not in ( "
                            + " select itr.\"Id\" "
                            + " from  "
                            + " \"SEC_User\" iu inner join "
                            + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                            + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                            + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                            + " and if.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + " = -1) or (iu.\"Oblast\" =" + oblast_id + ")) "
                            + " group by itr.\"Id\" "
                            + " order by sum( "
                            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1)  "
                            + " ) desc limit 7 "
                            + " ) "
                            + " and ((" + oblast_id + " = -1) or (u.\"Oblast\" = " + oblast_id + ")) "
                            + " group by  "
                            + " r.\"ReportYear\" "
                            + " order by 5 desc, 4 desc, 3 ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //---
        public Dictionary<string, object> reportConsByRes1(int oblast_id, int year, string lang)
        {
            #region query
            string query = " select "
                            + " tr.\"Id\" as resource_id, "
                            + " case '" + lang + "' "
                            + " when 'ru' then tr.\"NameRu\" "
                            + " when 'kz' then tr.\"NameKz\" "
                            + " else tr.\"NameRu\" end as resource_name, "
                            + " r.\"ReportYear\" as report_year, "
                            + " sum( "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)  "
                            + " )/1000 as consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and "
                            + " tr.\"Id\" in ( "
                            + " select itr.\"Id\" "
                            + " from  "
                            + " \"SEC_User\" iu inner join "
                            + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                            + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                            + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                            + " and if.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + " = -1) or (iu.\"Oblast\" =" + oblast_id + ")) "
                            + " group by itr.\"Id\" "
                            + " order by sum( "
                            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1)  "
                            + " ) desc limit 1 "
                            + " ) "
                            + " and ((" + oblast_id + " = -1) or (u.\"Oblast\" = " + oblast_id + ")) "
                            + " group by  "
                            + " tr.\"Id\", "
                            + " r.\"ReportYear\" ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        #endregion

        #region subject form
        //---- subject form
        public Dictionary<string, object> subjectForm(decimal subject_id, string lang,int year)
        {
            #region query
            string query = " select "
                            + " rr.\"OwnerName\" as \"JuridicalName\", "
                            + " rr.\"IDK\", "
                            + " case '" + lang + "'  "
                            + " when 'ru' then kato.\"NameRu\" "
                            + " when 'kz' then kato.\"NameKz\" "
                            + " else kato.\"NameRu\" end as oblast_name, "
                            + " case '" + lang + "'"
                            + " when 'ru' then jur_type.name_full "
                            + " when 'kz' then jur_type.name_full_kz "
                            + " else jur_type.name_full end as jur_name, "
                            + " case '" + lang + "'"
                            + " when 'ru' then oked.\"NameRu\" "
                            + " when 'kz' then oked.\"NameKz\" "
                            + " else oked.\"NameRu\" end as oked_name "
                            + " from  \"SEC_User\" u "
                            + "  inner join \"RST_ReportReestr\" as rr on rr.\"UserId\"=u.\"Id\" "
                            + "  inner join \"RST_Report\" as rs on rr.\"ReportId\"=rs.\"Id\" "
                              + " inner join \"DIC_Kato\" kato on u.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" left join "
                            + " v_fs_code jur_type on rr.usrfscode = jur_type.id left join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" "
                            + " where u.\"Id\"=" + subject_id + " and rs.\"ReportYear\"=" + year;
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //----
        public Dictionary<string, object> subjectFormPieChart(decimal subject_id, int year, string lang = "ru")
        {
            #region query
            string query = " select "
                    + " u.\"JuridicalName\", "
                    + " sum( "
                    + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1) "
                    + " else  "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                    + " ) as consumption "
                    + " from "
                    + " \"RST_ReportReestr\" rr inner join  "
                    + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                    + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                    + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                    + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                    + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                    + " where "
                    + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                    + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                    + " and rr.\"IsExcluded\" = false "
                    + " and r.\"ReportYear\" =" + year
                    + " and u.\"Id\" =" + subject_id
                    + " group by u.\"Id\" "
                    + " union all "
                    + " select "
                    + " case '" + lang + "'"
                    + " when 'ru' then 'Другие' "
                    + " when 'kz' then 'Басқалары' "
                    + " else 'Другие' end, "
                    + " sum( "
                    + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                    + " else "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                    + " ) as consumption "
                    + " from "
                    + " \"RST_ReportReestr\" rr inner join  "
                    + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                    + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                    + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                    + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                    + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                    + " where "
                    + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false "
                    + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                    + " and rr.\"IsExcluded\" = false  and r.\"ReportYear\" =" + year + "  and u.\"Id\"!= " + subject_id
                    + " group by u.\"Id\" ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //----Мероприятия
        public Dictionary<string, object> subForm4Record(decimal subject_id, int year)
        {
            #region query
            string query = "select  f4.* "
                           + " from "
                           + " \"RST_ReportReestr\" rr inner join  "
                           + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                           + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                           + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                           + " \"SUB_Form4Record\" f4 on f4.\"FormId\" = f.\"Id\" "
                           + " where  "
                           + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                           + " and f.\"IsDeleted\" = false "
                           + " and rr.\"IsExcluded\" = false "
                           + " and r.\"ReportYear\"=" + year
                           + " and u.\"Id\"=" + subject_id;
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //----Объем потребления энергоресурсов
        public Dictionary<string, object> subFormResValumeByYears(decimal subject_id)
        {
            #region query
            string query = " select "
                        + " r.\"ReportYear\" as report_year, "
                        + " sum( "
                        + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                        + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                        + " else  "
                        + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                        + " ) as consumption "
                        + " from "
                        + " \"RST_ReportReestr\" rr inner join  "
                        + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                        + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                        + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                        + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                        + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                        + " where  "
                        + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                        + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                        + " and rr.\"IsExcluded\" = false and u.\"Id\"=" + subject_id
                        + " group by r.\"ReportYear\" "
                        + " order by report_year, consumption ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //---- Структура потребления энергоресурсов
        public Dictionary<string, object> subFormResConsumption(decimal subject_id, int year, string lang)
        {
            #region
            string query = " select "
                         + " case '" + lang + "' "
                         + " when 'ru' then tr.\"NameRu\" "
                         + " when 'kz' then tr.\"NameKz\" "
                         + " else tr.\"NameRu\" end as resource_name, "
                         + " ut.\"NameRu\" as unit_meas, "
                         + " sum(coalesce(f2.\"OwnSource\", 0) ) as consumption_own, "
                         + " sum(coalesce(f2.\"NotOwnSource\", 0)) as consumption_notown, "
                         + " sum(coalesce(f2.\"TransferOtherLegal\", 0)) as transfer, "
                         + " sum( "
                         + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) "
                         + " ) as consumption, "
                         + " sum(coalesce(f2.\"ExpenceEnergy\", 0)) as expense "
                         + " from "
                         + " \"RST_ReportReestr\" rr inner join  "
                         + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                         + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                         + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                         + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                         + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" inner join "
                         + " \"DIC_Unit\" ut on tr.\"UnitId\" = ut.\"Id\" "
                         + " where  "
                         + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                         + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                         + " and rr.\"IsExcluded\" = false "
                         + " and r.\"ReportYear\" = " + year
                         + " and u.\"Id\" = " + subject_id
                         + " group by  tr.\"Id\", ut.\"Id\" "
                         + " order by consumption desc ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //----Динамика потребления энергоресурсов 1
        public Dictionary<string, object> subFormDynamicsResByYears1(decimal subject_id, int year)
        {
            #region
            string query = "select "
                           + " tr.\"Id\" as resource_id, tr.\"NameRu\"  as resource_name, "
                           + " r.\"ReportYear\" as report_year, "
                           + " sum( "
                           + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)  "
                           + " )/1000 as consumption "
                           + " from "
                           + " \"RST_ReportReestr\" rr inner join  "
                           + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                           + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                           + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                           + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                           + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                           + " where "
                           + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                           + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                           + " and rr.\"IsExcluded\" = false "
                           + " and "
                           + " tr.\"Id\" in (  "
                           + " select itr.\"Id\" "
                           + " from  "
                           + " \"SEC_User\" iu inner join "
                           + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                           + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                           + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                           + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                           + " and if.\"ReportYear\" = " + year + " and iu.\"Id\" =" + subject_id
                           + " group by itr.\"Id\" "
                           + " order by sum( "
                           + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) "
                           + " ) desc limit 1 ) "
                           + " and u.\"Id\"=" + subject_id
                           + " group by tr.\"Id\", r.\"ReportYear\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //----Динамика потребления энергоресурсов 7
        public Dictionary<string, object> subFormDynamicsResByYears7(decimal subject_id, int year, string lang)
        {
            #region
            string query = " select "
                    + " tr.\"Id\" as resource_id, "
                    + " case '" + lang + "'"
                    + " when 'ru' then tr.\"NameRu\" "
                    + " when 'kz' then tr.\"NameKz\" "
                    + " else tr.\"NameRu\" end as resource_name, "
                    + " r.\"ReportYear\" as report_year, "
                    + " sum( "
                    + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                    + " else "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                    + " )/1000 as consumption, "
                    + " 1 as not_others "
                    + " from "
                    + " \"RST_ReportReestr\" rr inner join  "
                    + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                    + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                    + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                    + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                    + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                    + " where  "
                    + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                    + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                    + " and rr.\"IsExcluded\" = false "
                    + " and  "
                    + " tr.\"Id\" not in ( "
                    + " select itr.\"Id\"  "
                    + " from  "
                    + " \"SEC_User\" iu inner join "
                    + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                    + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                    + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                    + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                    + " and if.\"ReportYear\" =" + year
                    + " and iu.\"Id\" =" + subject_id
                    + " group by itr.\"Id\" "
                    + " order by sum( "
                    + " case when coalesce(if2.\"ExtractVolume\", 0) > 0 then "
                    + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0) - coalesce(if2.\"TransferOtherLegal\", 0)) * coalesce(itr.\"Keof\", 1) "
                    + " else  "
                    + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) end "
                    + " ) desc limit 1 ) and "
                    + " tr.\"Id\"  in ( "
                    + " select itr.\"Id\"  from "
                    + " \"SEC_User\" iu inner join "
                    + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                    + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                    + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                    + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                    + " and if.\"ReportYear\"=" + year + "  and iu.\"Id\" = " + subject_id
                    + " group by itr.\"Id\" "
                    + " order by sum( "
                    + " case when coalesce(if2.\"ExtractVolume\", 0) > 0 then  "
                    + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0) - coalesce(if2.\"TransferOtherLegal\", 0)) * coalesce(itr.\"Keof\", 1)  "
                    + " else  "
                    + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) end "
                    + " ) desc limit 7) "
                    + " and u.\"Id\" =" + subject_id
                    + " group by tr.\"Id\",  r.\"ReportYear\" "
                    + " union all "
                    + " select "
                    + " -1 as resource_id, "
                    + " case '" + lang + "' "
                    + " when 'ru' then 'Прочие' "
                    + " when 'kz' then 'Басқалары' "
                    + " else 'Прочие' end as resource_name, "
                    + " r.\"ReportYear\" as report_year, "
                    + " sum( "
                    + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1) "
                    + " else  "
                    + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                    + " )/1000 as consumption, "
                    + " 0 as not_others "
                    + " from "
                    + " \"RST_ReportReestr\" rr inner join  "
                    + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                    + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                    + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                    + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                    + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                    + " where "
                    + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                    + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                    + " and rr.\"IsExcluded\" = false "
                    + " and "
                    + " tr.\"Id\" not in ( "
                    + " select itr.\"Id\" "
                    + " from  "
                    + " \"SEC_User\" iu inner join "
                    + " \"SUB_Form\" if on if.\"UserId\" = iu.\"Id\" inner join "
                    + " \"SUB_Form2Record\" if2 on if.\"Id\" = if2.\"FormId\" inner join "
                    + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                    + " where if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                    + " and if.\"ReportYear\" = " + year + "  and iu.\"Id\" = " + subject_id
                    + " group by itr.\"Id\" "
                    + " order by sum( "
                    + " case when coalesce(if2.\"ExtractVolume\", 0) > 0 then  "
                    + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0) - coalesce(if2.\"TransferOtherLegal\", 0)) * coalesce(itr.\"Keof\", 1)  "
                    + " else  "
                    + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) end "
                    + " ) desc limit 7 ) "
                    + " and u.\"Id\" = " + subject_id
                    + " group by  r.\"ReportYear\"  order by 5 desc, 4 desc, 3 ";

            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //----Показатели энергоэффективности
        public Dictionary<string, object> subFormEnergyIndicators(decimal subject_id, int year)
        {
            #region query
            string query = " select  f5.* , case f5.energyindicator_id when null then f5.\"IndicatorName\" else  sd.nameru  end as \"IndicatorName1\" "
                       + " from "
                       + " \"RST_ReportReestr\" rr inner join  "
                       + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                       + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                       + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" "
                       +" inner join  \"SUB_Form5Record\" f5 on f5.\"FormId\" = f.\"Id\"  "
                       + " left join sub_dic_energyindicator sd on f5.energyindicator_id=sd.id"
                       + " where  "
                       + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                       + " and f.\"IsDeleted\" = false and rr.\"IsExcluded\" = false "
                       + " and r.\"ReportYear\" = " + year + "   and u.\"Id\" = " + subject_id
                       + " and f5.\"EnergyValue\" > 0 ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        //----Сведения об энергоаудите 
        public Dictionary<string, object> subFormEAudit(decimal subject_id)
        {
            #region
            string query = " select "
                    + " ea.\"AuditorName\" as auditor_name, "
                    + " ea.\"ReportYear\" as report_year "
                    + " from \"EAUDIT_Preamble\" ea "
                    + " where "
                    + " ea.\"refEauditObject\"=" + subject_id
                    + "  order by ea.\"ReportYear\" ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        #endregion

        public Dictionary<string, object> reportTop10ByCons(int oblast_id, int year)
        {
            string query = "  select "
                            + " r.\"ReportYear\" as report_year, "
                            + " u.\"Id\" as subject_id, "
                            + " u.\"JuridicalName\" as subject_name, "
                            + " sum( "
                            + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                            + " else  "
                            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                            + " )/1000 as consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and kato.\"IsDeleted\" = false "
                            + " and rr.\"IsExcluded\" = false "
                            + " and r.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + "= -1)or(kato.\"Id\" = " + oblast_id + ")) "
                            + " group by r.\"ReportYear\",  u.\"Id\" "
                            + " order by consumption desc  limit 10";

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> reportTop10AndOthersByCons(int oblast_id, int year, string lang)
        {
            #region query
            string query = " select ( select  sum( "
            + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
            + " else  "
            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
            + " ) as consumption "
            + " from "
            + " \"RST_ReportReestr\" rr inner join  "
            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
            + " where  "
            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false  "
            + " and kato.\"IsDeleted\" = false "
            + " and rr.\"IsExcluded\" = false "
            + " and r.\"ReportYear\" = " + year
            + " and ((" + oblast_id + " = -1)or(kato.\"Id\" = " + oblast_id + ")) "
            + " and "
            + " u.\"Id\" in ( "
            + " select iu.\"Id\" "
            + " from \"RST_ReportReestr\" irr inner join  "
            + " \"RST_Report\" ir on irr.\"ReportId\" = ir.\"Id\" inner join "
            + " \"DIC_Kato\" ikato on irr.\"Oblast\" = ikato.\"Id\" inner join "
            + " \"SEC_User\" iu on irr.\"UserId\" = iu.\"Id\" inner join "
            + " \"SUB_Form\" if on iu.\"Id\" = if.\"UserId\" and ir.\"ReportYear\" = if.\"ReportYear\" inner join "
            + " \"SUB_Form2Record\" if2 on if2.\"FormId\" = if.\"Id\" inner join "
            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
            + " where "
            + " ir.\"ReportYear\" =" + year
            + " and irr.\"IsExcluded\" = false "
            + " and ((" + oblast_id + " = -1)or(ikato.\"Id\" = " + oblast_id + ")) "
            + " group by iu.\"Id\" "
            + " order by sum( "
            + " case when coalesce(if2.\"ExtractVolume\", 0) > 0 then  "
            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0) - coalesce(if2.\"TransferOtherLegal\", 0)) * coalesce(itr.\"Keof\", 1)  "
            + " else  "
            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) end "
            + " ) desc limit 10 "
            + " )) as top10_value, "
            + " 'Топ 10' as top10_name  "
            + " union all "
            + " select ( "
            + " select sum( "
            + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
            + " else  "
            + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
            + " ) as consumption "
            + " from "
            + " \"RST_ReportReestr\" rr inner join  "
            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
            + " where  "
            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
            + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
            + " and kato.\"IsDeleted\" = false "
            + " and rr.\"IsExcluded\" = false "
            + " and r.\"ReportYear\" = " + year
            + " and ((" + oblast_id + " = -1)or(kato.\"Id\" = " + oblast_id + ")) "
            + " and "
            + " u.\"Id\" not in ( select iu.\"Id\" "
            + " from \"RST_ReportReestr\" irr inner join  "
            + " \"RST_Report\" ir on irr.\"ReportId\" = ir.\"Id\" inner join "
            + " \"DIC_Kato\" ikato on irr.\"Oblast\" = ikato.\"Id\" inner join "
            + " \"SEC_User\" iu on irr.\"UserId\" = iu.\"Id\" inner join "
            + " \"SUB_Form\" if on iu.\"Id\" = if.\"UserId\" and ir.\"ReportYear\" = if.\"ReportYear\" inner join "
            + " \"SUB_Form2Record\" if2 on if2.\"FormId\" = if.\"Id\" inner join "
            + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
            + " where "
            + " ir.\"ReportYear\" =" + year + "  and irr.\"IsExcluded\" = false "
            + " and ((" + oblast_id + "= -1)or(ikato.\"Id\" = " + oblast_id + ")) "
            + " group by iu.\"Id\" "
            + " order by sum( "
            + " case when coalesce(if2.\"ExtractVolume\", 0) > 0 then  "
            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0) - coalesce(if2.\"TransferOtherLegal\", 0)) * coalesce(itr.\"Keof\", 1)  "
            + " else  "
            + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) end "
            + " ) desc limit 10 )), "
            + " case '" + lang + "' when 'ru' then 'Остальные'  "
            + " when 'kz' then 'Басқалары'  "
            + " else 'Остальные' end ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        #region report 3
        public Dictionary<string, object> reportByJurType(int oblast_id, string lang)
        {
            #region query
            string query = " select "
                           + " jur_type_id, "
                           + " case '" + lang + "'  "
                           + " when 'ru' then jur_type_name_full "
                           + " when 'kz' then jur_type_name_full_kz "
                           + " else jur_type_name_full end as jur_type_name_full, "
                           + " report_year, "
                           + " sum(consumption) as consumption, "
                           + " count(distinct subject_id) as subject_count "
                           + " from "
                           + " v_consumption_star "
                           + " where  "
                           + " ((" + oblast_id + " = -1) or (oblast_id = " + oblast_id + ")) "
                           + " group by  "
                           + " jur_type_id, "
                           + " report_year, "
                           + " jur_type_name_full, "
                           + " jur_type_name_full_kz ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> reportTop10ByJurType(int oblast_id, int year, int jur_type_id)
        {
            #region query
            string query = "  select "
                         + " report_year, "
                         + " subject_id, "
                         + " subject_name, "
                         + " sum(consumption) as consumption "
                         + " from "
                         + " v_consumption_star "
                         + " where  "
                         + " report_year=" + year
                         + " and jur_type_id=" + jur_type_id
                         + " and ((" + oblast_id + " = -1)or (oblast_id = " + oblast_id + ")) "
                         + " group by 	report_year, subject_id, subject_name "
                         + " order by sum(consumption) desc "
                         + " limit 10 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }
        #endregion

        #region report 4
        public Dictionary<string, object> reportConsByOked(int year, string lang)
        {
            #region query
            string query = "  select "
                + " r.\"ReportYear\" as report_year, "
                + " oked.\"RootId\" as oked_root_id, "
                + " (select case '" + lang + "' "
                + " when 'ru' then \"NameRu\" "
                + " when 'kz' then \"NameKz\" "
                + " else \"NameRu\" end from \"DIC_OKED\" where \"Id\" = oked.\"RootId\") as oked_name, "
                + " sum( "
                + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                + " else  "
                + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                + " ) as consumption, "
                + " count(distinct u.\"Id\") as subject_count, "
                + " ( "
                + " select count(distinct iu.\"Id\") "
                + " from "
                + " \"RST_ReportReestr\" irr inner join  "
                + " \"RST_Report\" ir on irr.\"ReportId\" = ir.\"Id\" inner join "
                + " \"DIC_Kato\" ikato on irr.\"Oblast\" = ikato.\"Id\" inner join "
                + " \"SEC_User\" iu on irr.\"UserId\" = iu.\"Id\" inner join "
                + " \"DIC_TypeApplication\" ia on iu.\"TypeApplicationId\" = ia.\"Id\" inner join "
                + " \"DIC_OKED\" ioked on iu.\"OkedId\" = ioked.\"Id\" inner join "
                + " \"SUB_Form\" if on iu.\"Id\" = if.\"UserId\" and ir.\"ReportYear\" = if.\"ReportYear\" inner join "
                + " \"SUB_Form2Record\" if2 on if2.\"FormId\" = if.\"Id\" inner join "
                + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                + " where  "
                + " irr.\"IsDeleted\" = false and ir.\"IsDeleted\" = false and iu.\"IsDeleted\" = false  "
                + " and ia.\"IsDeleted\" = false and if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                + " and ikato.\"IsDeleted\" = false "
                + " and ir.\"ReportYear\" = r.\"ReportYear\" "
                + " and irr.\"IsExcluded\" = false "
                + " ) as count_all, "
                + " ( "
                + " select sum( "
                + " case when coalesce(if2.\"ExtractVolume\", 0) > 0 then  "
                + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0) - coalesce(if2.\"TransferOtherLegal\", 0)) * coalesce(itr.\"Keof\", 1)  "
                + " else  "
                + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) end "
                + " ) "
                + " from "
                + " \"RST_ReportReestr\" irr inner join  "
                + " \"RST_Report\" ir on irr.\"ReportId\" = ir.\"Id\" inner join "
                + " \"DIC_Kato\" ikato on irr.\"Oblast\" = ikato.\"Id\" inner join "
                + " \"SEC_User\" iu on irr.\"UserId\" = iu.\"Id\" inner join "
                + " \"DIC_TypeApplication\" ia on iu.\"TypeApplicationId\" = ia.\"Id\" inner join "
                + " \"DIC_OKED\" ioked on iu.\"OkedId\" = ioked.\"Id\" inner join "
                + " \"SUB_Form\" if on iu.\"Id\" = if.\"UserId\" and ir.\"ReportYear\" = if.\"ReportYear\" inner join "
                + " \"SUB_Form2Record\" if2 on if2.\"FormId\" = if.\"Id\" inner join "
                + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                + " where  "
                + " irr.\"IsDeleted\" = false and ir.\"IsDeleted\" = false and iu.\"IsDeleted\" = false  "
                + " and ia.\"IsDeleted\" = false and if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                + " and ikato.\"IsDeleted\" = false "
                + " and ir.\"ReportYear\" = r.\"ReportYear\" "
                + " and irr.\"IsExcluded\" = false "
                + " ) as consumption_all "
                + " from "
                + " \"RST_ReportReestr\" rr inner join  "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                + "  "
                + " where  "
                + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                + " and kato.\"IsDeleted\" = false "
                + " and r.\"ReportYear\" =" + year
                + " and rr.\"IsExcluded\" = false "
                + " group by  "
                + " r.\"ReportYear\", "
                + " oked.\"RootId\" "
                + " order by 4 desc  ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        #endregion

        #region report 6
        public Dictionary<string, object> reportEffecSecial(int oblast_id, int year, string lang)
        {
            #region query
            string query = " select  (select case '" + lang + "' "
                            + " when 'ru' then o2.\"NameRu\" "
                            + " when 'kz' then o2.\"NameKz\" "
                            + " else o2.\"NameRu\" end from \"DIC_OKED\" o2 where o2.\"Id\" = oked.\"RootId\") as oked_name, "
                            + " min(f5.\"EnergyValue\") as min_value, "
                            + " max(f5.\"EnergyValue\") as max_value, "
                            + " avg(f5.\"EnergyValue\") as avg_value "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                            + " \"SUB_Form5Record\" f5 on f5.\"FormId\" = f.\"Id\" "
                            + " where  "
                            + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and kato.\"IsDeleted\" = false  "
                            + " and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and oked.\"IsDeleted\" = false  "
                            + " and f.\"IsDeleted\" = false "
                            + " and oked.\"RootId\" in (1281, 1261, 1305) "
                            + " and trim(f5.\"UnitMeasure\") in ('гкал/м2','г кал/м2','Гкал/м2','Гкал/м²') "
                            + " and f5.\"EnergyValue\" != 0 "
                            + " and rr.\"IsExcluded\" = false "
                            + " and r.\"ReportYear\" = " + year
                            + " and ((" + oblast_id + " = -1) or (kato.\"Id\" = " + oblast_id + ")) "
                            + " group by  "
                            + " r.\"ReportYear\", "
                            + " oked.\"RootId\", "
                            + " kato.\"Id\" ";

            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> reportFlatEffecSecial(int oblast_id,int year,string lang)
        {
            #region query
            string query = " select "
                + " u.\"JuridicalName\" as subject_name, "
                + " u.\"IDK\" as idk, "
                + " (select  "
                + " case '" + lang + "' "
                + " when 'ru' then o2.\"NameRu\" "
                + " when 'kz' then o2.\"NameKz\" "
                + " else o2.\"NameRu\" end from \"DIC_OKED\" o2 where o2.\"Id\" = oked.\"RootId\") as oked_name, "
                + " f5.\"EnergyValue\" as energy_value "
                + " from "
                + " \"RST_ReportReestr\" rr inner join  "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                + " \"SUB_Form5Record\" f5 on f5.\"FormId\" = f.\"Id\" "
                + " where  "
                + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and kato.\"IsDeleted\" = false  "
                + " and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and oked.\"IsDeleted\" = false  "
                + " and f.\"IsDeleted\" = false "
                + " and oked.\"RootId\" in (1281, 1261, 1305) "
                + " and trim(f5.\"UnitMeasure\") in ('гкал/м2','г кал/м2','Гкал/м2','Гкал/м²') "
                + " and f5.\"EnergyValue\" != 0 "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" = " + year
                + " and ((" + oblast_id + " = -1) or (kato.\"Id\" = " + oblast_id + ")) "
                + " order by 4 desc ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }
        #endregion

        #region report 7
        public Dictionary<string, object> reportMoreThan100(int oblast_id, int year)
        {
            #region query
            string query = " select  "
                + " t1.report_year, "
                + " t1.subject_id, "
                + " t1.subject_name, "
                + " t1.consumption, "
                + " t1.cons_prev, "
                + " case when t1.cons_prev = null then null "
                + " when t1.cons_prev = 0 then null "
                + " else (t1.consumption-t1.cons_prev)*100/t1.cons_prev "
                + " end as dynamic "
                + "  "
                + " from ( "
                + " select "
                + " r.\"ReportYear\" as report_year, "
                + " u.\"Id\" as subject_id, "
                + " u.\"JuridicalName\" as subject_name, "
                + " sum( "
                + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                + " else  "
                + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                + " ) as consumption, "
                + " ( "
                + " select "
                + " sum( "
                + " case when coalesce(if2.\"ExtractVolume\", 0) > 0 then  "
                + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0) - coalesce(if2.\"TransferOtherLegal\", 0)) * coalesce(itr.\"Keof\", 1)  "
                + " else  "
                + " (coalesce(if2.\"OwnSource\", 0) + coalesce(if2.\"NotOwnSource\", 0)) * coalesce(itr.\"Keof\", 1) end "
                + " ) as cons_prev "
                + " from "
                + " \"RST_ReportReestr\" irr inner join  "
                + " \"RST_Report\" ir on irr.\"ReportId\" = ir.\"Id\" inner join "
                + " \"DIC_Kato\" ikato on irr.\"Oblast\" = ikato.\"Id\" inner join "
                + " \"SEC_User\" iu on irr.\"UserId\" = iu.\"Id\" inner join "
                + " \"SUB_Form\" if on iu.\"Id\" = if.\"UserId\" and ir.\"ReportYear\" = if.\"ReportYear\" inner join "
                + " \"SUB_Form2Record\" if2 on if2.\"FormId\" = if.\"Id\" inner join "
                + " \"SUB_DIC_TypeResource\" itr on if2.\"TypeResourceId\" = itr.\"Id\" "
                + " where "
                + " irr.\"IsDeleted\" = false and ir.\"IsDeleted\" = false and iu.\"IsDeleted\" = false  "
                + " and if.\"IsDeleted\" = false and itr.\"IsDeleted\" = false "
                + " and ikato.\"IsDeleted\" = false "
                + " and irr.\"IsExcluded\" = false "
                + " and ir.\"ReportYear\" = r.\"ReportYear\"-1 "
                + " and iu.\"Id\" = u.\"Id\" "
                + " ) as cons_prev "
                + " from "
                + " \"RST_ReportReestr\" rr inner join  "
                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                + " \"DIC_Kato\" kato on rr.\"Oblast\" = kato.\"Id\" inner join "
                + " \"SEC_User\" u on rr.\"UserId\" = u.\"Id\" inner join "
                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" inner join "
                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" and r.\"ReportYear\" = f.\"ReportYear\" inner join "
                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                + " where  "
                + " rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                + " and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                + " and kato.\"IsDeleted\" = false "
                + " and rr.\"IsExcluded\" = false "
                + " and r.\"ReportYear\" =" + year
                + " and ((" + oblast_id + " = -1)or(kato.\"Id\" = " + oblast_id + ")) "
                + " group by  "
                + " r.\"ReportYear\", "
                + " u.\"Id\" "
                + " having sum( "
                + " case when coalesce(f2.\"ExtractVolume\", 0) > 0 then  "
                + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0) - coalesce(f2.\"TransferOtherLegal\", 0)) * coalesce(tr.\"Keof\", 1)  "
                + " else  "
                + " (coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1) end "
                + " ) > 100000 "
                + " ) t1 "
                + " order by \"consumption\" desc NULLS LAST ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }
        #endregion

        #region map
        public Dictionary<string, object> reportSubjectsOnMap(int year)
        {
            #region
            string query = " select u.\"Id\", u.\"JuridicalName\", u.\"Lat\", u.\"Lng\" "
                    + " from \"RST_ReportReestr\" rr inner join  "
                    + "   \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                    + "   \"SEC_User\" u on rr.\"UserId\" = u.\"Id\"  "
                    + " where "
                    + "   rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false "
                    + "   and u.\"IsDeleted\" = false "
                    + "   and rr.\"IsExcluded\" = false "
                    + "   and u.\"Lat\" is not null and u.\"Lng\" is not null "
                    + "   and r.\"ReportYear\" = " + year;
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> reportOblConsOnMap(int year, string lang)
        {
            #region query
            string query = " select 	t.\"Id\", case '" + lang + "'  "
                        + " when 'ru' then t.\"NameRu\" "
                        + " when 'kz' then t.\"NameKz\" "
                        + " else t.\"NameRu\" end as name, "
                        + " t.\"Lat\", t.\"Lng\", "
                        + " ( select  "
                        + " coalesce(round(sum(coalesce(consumption, 0)) / 1000), 0) "
                        + " from v_consumption_star  "
                        + " where  	report_year=" + year + " 	and "
                        + " oblast_id = t.\"Id\" ) as val "
                        + " from \"DIC_Kato\" t   "
                        + " where  t.\"refParent\" = 1 order by t.\"NameRu\" ";

            #endregion

            var item = common.getTableList(query);
            return item;
        }
        #endregion

		#region views
		public Dictionary<string, object> viewByJurType(string lang)
		{
			#region query
			string query = " select "
			+ " jur_type_id, "
			+ " case '" + lang + "' "
			+ " when 'ru' then jur_type_name_short "
			+ " when 'kz' then jur_type_name_short_kz "
			+ " else jur_type_name_short end as jur_type_name_short, "
			+ " report_year, "
			+ " sum(consumption) as consumption, "
			+ " count(distinct subject_id) as subject_count "
			+ " from "
			+ " v_consumption_star "
			+ " group by  "
			+ " jur_type_id, "
			+ " jur_type_name_short, "
			+ " jur_type_name_short_kz, "
			+ " report_year ";
			#endregion

			var item = common.getTableList(query);
			return item;
		}

		public Dictionary<string, object> viewConsByRes8(int year,string lang)
		{
			#region query
			string query = " select "
							+ " resource_id, "
							+ " ( select "
							+ " case '"+lang+"' "
							+ " when 'ru' then \"NameRu\" when 'kz' then \"NameKz\" "
							+ " else \"NameRu\" end from \"SUB_DIC_TypeResource\" where \"Id\" = resource_id "
							+ " ) resource_name, "
							+ " report_year, "
							+ " sum((consumption_own+consumption_not_own)*koef) as consumption "
							+ " from v_consumption_star "
							+ " where report_year = "+year
							+ " group by  resource_id, resource_name, report_year ";
			#endregion

			var item = common.getTableList(query);
			return item;
		}

		public Dictionary<string, object> viewByJurTop10(int year)
		{

			#region query
			string query = "  select subject_id, subject_name, "
						+ " report_year,  sum(consumption) as consumption "
						+ " from  v_consumption_star "
						+ " where report_year ="+year
						+ " group by  subject_id, subject_name, report_year "
						+ " order by sum(consumption) desc limit 10 ";
			#endregion

			var item = common.getTableList(query);
			return item;
		}

		public Dictionary<string, object> viewAskuerDay(int CmdeviceId)
		{
            #region query
            /*string query = "   select "
                           + " m.month_name||'-'||cast(extract(day from iv.\"DatetimeStamp\")as varchar) as day, "
                           + " avg(iv.\"Value\") as val  "
                           + " from "
                           + " \"COLLECTOR_Cmdevice\" d inner join "
                           + " \"COLLECTOR_DIC_CmdeviceTypes\" dt on d.\"refIndicatorType\" = dt.\"Id\" inner join "
                           + " \"COLLECTOR_IndicatorValues\" iv on iv.\"refCmdevice\" = d.\"Id\" inner join "
                           + " v_months m on extract(month from iv.\"DatetimeStamp\")=m.month_id  "
                           + " where  d.\"Id\"="+CmdeviceId
                           + " group by  "
                           + " m.month_name||'-'||cast(extract(day from iv.\"DatetimeStamp\")as varchar) ";

       string query = " select to_char(iv.\"DatetimeStamp\", 'YYYY-MM-DD') as day,  avg(iv.\"Value\") as val    "
        + " from  \"COLLECTOR_Cmdevice\" d  "
        + " inner join  \"COLLECTOR_DIC_CmdeviceTypes\" dt on d.\"refIndicatorType\" = dt.\"Id\"  "
        + " inner join  \"COLLECTOR_IndicatorValues\" iv on iv.\"refCmdevice\" = d.\"Id\"  "
        + " inner join  v_months m on extract(month from iv.\"DatetimeStamp\")=m.month_id    "
        + " where  d.\"Id\"=" + CmdeviceId
        + " group by   to_char(iv.\"DatetimeStamp\", 'YYYY-MM-DD')   "
        + " order by " + CmdeviceId;*/

            string query = "  SELECT *  FROM (  "
                        + "  select to_char(iv.\"DatetimeStamp\", 'DD-MM-YYYY') as day,  avg(iv.\"Value\") as val  ,to_char(iv.\"DatetimeStamp\", 'YYYY-MM-DD') as dayorder "
                        + "  from  \"COLLECTOR_Cmdevice\" d  "
                        + "  inner join  \"COLLECTOR_DIC_CmdeviceTypes\" dt on d.\"refIndicatorType\" = dt.\"Id\"  "
                        + "  inner join  \"COLLECTOR_IndicatorValues\" iv on iv.\"refCmdevice\" = d.\"Id\"  "
                        + "  where  d.\"Id\"=" + CmdeviceId
                        + "  group by   to_char(iv.\"DatetimeStamp\", 'DD-MM-YYYY') ,to_char(iv.\"DatetimeStamp\", 'YYYY-MM-DD')  "
                        + "  order by 3 desc LIMIT 20  "
                        + "  ) as t "
                        + " ORDER BY dayorder ASC ";

            #endregion
            var item = common.getTableList(query);
			return item;
		}

		public Dictionary<string, object> viewAskuerMonth(int CmdeviceId)
		{
			#region
			string query = " select  m.month_id, m.month_name, avg(iv.\"Value\") as val "
				+ " from "
				+ " \"COLLECTOR_Cmdevice\" d inner join "
				+ " \"COLLECTOR_DIC_CmdeviceTypes\" dt on d.\"refIndicatorType\" = dt.\"Id\" inner join "
				+ " \"COLLECTOR_IndicatorValues\" iv on iv.\"refCmdevice\" = d.\"Id\" inner join "
				+ " v_months m on extract(month from iv.\"DatetimeStamp\") = m.month_id "
				+ " where  d.\"Id\" = "+CmdeviceId
				+ " group by  m.month_id, m.month_name ";
			#endregion

			var item = common.getTableList(query);
			return item;
		}
		#endregion
	}
}