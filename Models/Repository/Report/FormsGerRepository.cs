using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Report
{
    public class FormsGerRepository
    {
        Common common = new Common();

        //8
        public Dictionary<string, object> gerPropertyConsumption(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                         + "   COALESCE( "
                         + "     case '" + lang + "'  when 'ru' then types.name_full   "
                         + "       when 'kz' then types.name_full_kz  else types.name_full   "
                         + "     end  "
                         + "     ,  "
                         + "     case '"+lang+"'  when 'ru' then 'Не установленные'   "
                         + "       when 'kz' then 'Белгiленбеген'  else 'is null'  "
                         + "     end  "
                         + "   ) as property_type "
                         + "   ,  count(table_data.\"Id\") as qty_subject "
                          + "   , sum(table_data.consumption)  as qty_consumption  "
                          + " from "
                          + " ( select  rr.usrfscode  "
                          + "       , rr.\"Id\"  "
                           + "       ,  sum(coalesce( "
                           + "       (  select  sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1))   "
                            + "            from  \"SUB_Form\" f,   "
                            + "            \"SUB_Form2Record\" f2,  \"SUB_DIC_TypeResource\" tr   "
                            + "            where  f.\"Id\" = rr.\"FormId\" "
                            + "            and f2.\"FormId\" = f.\"Id\"  "
                            + "            and f2.\"TypeResourceId\" = tr.\"Id\"  "
                            + "            and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false   "
                            + "       ), 0)) as consumption   "
                            + "       from  \"RST_ReportReestr\" rr, \"RST_Report\" r  , \"SEC_User\" u "
                            + "       where rr.\"ReportId\" = r.\"Id\" "
                            + "       and r.\"ReportYear\" = "+year
                            + "       and rr.\"IsExcluded\"=false "
                            + "       and rr.\"UserId\"=u.\"Id\"  "
                            + "       and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and  u.\"IsDeleted\"=false "
                            + "       group by  rr.usrfscode "
                            + "       ,rr.\"Id\"  "
                            + "       ) table_data "
                            + "  left join v_fs_code as types on table_data.usrfscode=types.id "
                            + "  group by table_data.usrfscode, types.name_full, types.name_full_kz  ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //9
        public Dictionary<string, object> gerOblastConsumption(int year, string lang = "ru")
        {

            string query = " select   case '"+lang+"'  when 'ru' then kato.\"NameRu\"  when 'kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end as oblast_name,   "
                              + "  (select count(distinct rr.\"Id\")    "
                              + "    from \"RST_ReportReestr\" rr, \"RST_Report\" r      "
                              + "      where rr.\"ReportId\" = r.\"Id\"  "
                              + "         "
                              + "      and r.\"ReportYear\" ="+year
                              + "      and rr.\"Oblast\" = kato.\"Id\"    "
                              + "      and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false  "
                              + "      and rr.\"IsExcluded\"=false  "
                              + "  ) as qty_subject,  "
                              + "  (select  sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1))   "
                              + "    from  \"RST_ReportReestr\" rr, \"RST_Report\" r      "
                              + "    ,  \"SUB_Form\" f,  \"SUB_Form2Record\" f2,  \"SUB_DIC_TypeResource\" tr "
                              + "    where rr.\"ReportId\" = r.\"Id\"  "
                              + "    and rr.\"FormId\"= f.\"Id\"     "
                              + "    and rr.\"IsExcluded\"=false "
                              + "    and f2.\"FormId\" = f.\"Id\" and f2.\"TypeResourceId\"=tr.\"Id\"       "
                              + "    and r.\"ReportYear\" ="+year
                              + "    and rr.\"Oblast\" = kato.\"Id\"     "
                              + "    and rr.\"IsDeleted\" = false  and r.\"IsDeleted\" = false      "
                              + "    and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false   "
                              + "  ) as qty_consumption   "
                              + "  from       "
                              + "  (select kto.\"Id\", kto.\"Code\", kto.\"NameRu\" ,kto.\"NameKz\",  fsall.\"Oblast\"  "
                              + "      from \"DIC_Kato\" as kto,  "
                              + "     (select rr.\"Oblast\"  "
                              + "          from   \"RST_ReportReestr\" rr, \"RST_Report\" r  "
                              + "          where  "
                              + "          r.\"ReportYear\"="+year
                              + "          and r.\"Id\"=rr.\"ReportId\"  "
                              + "          and rr.\"IsDeleted\" = false "
                              + "          and rr.\"IsExcluded\"=false             "
                              + "          group by rr.\"Oblast\" ) fsall  "
                              + "  where kto.\"Id\" =fsall.\"Oblast\") kato  "
                              + "  order by kato.\"Code\"  ";

            var item=common.getTableList(query);
            return item;
            /*List<string> keysToInclude = new List<string> { "oblast_name", "qty_subject", "qty_consumption" };
            var hlist = new List<Dictionary<string, object>>();
            var list = new List<Dictionary<string, object>>();
            string errorMessage = common.getTableWithList(ref list, query);
            foreach (var l in list)
            {
                if (year>2017 && l["Code"].ToString() == "5100000000")
                {
                    l["qty_consumption"] = 0;
                    l["qty_subject"] = 0;
                }

                var dict = l as Dictionary<string,object>;
                var keysToRemove = dict.Keys.Except(keysToInclude).ToList();

                foreach (var key in keysToRemove)
                    dict.Remove(key);         
            }
        
            var rItem = new Dictionary<string, object>();
            rItem["ListItems"] = list;
            rItem["ErrorMessage"] = errorMessage;
            return rItem;*/

        }

        //10
        public Dictionary<string, object> gerEconActivityConsumption(int year, string lang = "ru")
        {
            #region query
            string query =" select t.root_name, t.qty_subject,t.qty_consumption "
                        + " from   "
                        + " (select   "
                        + "   oked.\"Id\" as oked_id, "
                        + "     case '"+lang+"'  when 'ru' then oked.\"NameRu\"  when 'kz' then oked.\"NameKz\" else oked.\"NameRu\"  end  as root_name            "
                        + "     ,COALESCE (subject.cnt,0) as qty_subject "
                        + "     ,COALESCE (consumption.summary,0) as qty_consumption "
                        + "  from  \"DIC_OKED\" as oked "
                        + "   left join (select okd.\"RootId\", count(rr.\"Id\") cnt "
                        + "      from \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u, \"DIC_OKED\" okd "
                        + "      where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\"   "
                        + "      and r.\"ReportYear\" ="+year
                        + "      and rr.usrokedid = okd.\"Id\"   "
                        + "      and rr.\"IsExcluded\"=false  "
                        + "      and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                        + "      group by 1  "
                        + "    ) as subject on subject.\"RootId\" = oked.\"Id\" "
                        + "        "
                        + "   left join (select  okd.\"RootId\",  sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) summary "
                        + "      from  \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u "
                        + "      ,  \"SUB_Form\" f,  \"SUB_Form2Record\" f2 ,  \"SUB_DIC_TypeResource\" tr , \"DIC_OKED\" okd  "
                        + "      where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\"  "
                        + "      and rr.\"FormId\" = f.\"Id\"  "
                        + "      and rr.\"IsExcluded\"=false  "
                        + "      and f2.\"FormId\" = f.\"Id\"  "
                        + "      and f2.\"TypeResourceId\" = tr.\"Id\"  "
                        + "      and r.\"ReportYear\" ="+year
                        + "      and rr.usrokedid = okd.\"Id\"      "
                        + "      and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                        + "      and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false        "
                        + "      group by 1      "
                        + "    ) as consumption on consumption.\"RootId\" = oked.\"Id\" "
                        + "    where oked.\"refParent\" is null "
                        + "  UNION "
                        + "  select 1000000 as oked_id  "
                        + "    ,case '"+lang+"'  when 'ru' then 'Не установленные' when 'kz' then 'Белгiленбеген'  else 'is null' end  as root_name "
                        + "    ,COALESCE (subject.cnt,0) as qty_subject , COALESCE (consumption.summary,0) as qty_consumption "
                        + "  from "
                        + "    (select  count(rr.\"Id\") cnt "
                        + "      from \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u "
                        + "      where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\"   "
                        + "      and r.\"ReportYear\" ="+year
                        + "      and rr.usrokedid is null   "
                        + "      and rr.\"IsExcluded\"=false  "
                        + "      and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false       "
                        + "    ) as subject "
                        + "    ,    "
                        + "    (select  sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) summary "
                        + "      from  \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u "
                        + "      ,  \"SUB_Form\" f,  \"SUB_Form2Record\" f2 ,  \"SUB_DIC_TypeResource\" tr  "
                        + "      where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\"  "
                        + "      and rr.\"FormId\" = f.\"Id\"  "
                        + "      and rr.\"IsExcluded\"=false  "
                        + "      and f2.\"FormId\" = f.\"Id\"  "
                        + "      and f2.\"TypeResourceId\" = tr.\"Id\"  "
                        + "      and r.\"ReportYear\" ="+year
                        + "      and rr.usrokedid is NULL      "
                        + "      and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                        + "      and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false   "
                        + "    ) as consumption "
                        + " ) t    "
                        + " order by t.oked_id ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //11
        public Dictionary<string, object> gerConsumptionByConsumptionGroup(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " case '" + lang + "' "
                            + " when 'ru' then types.name_full "
                            + " when 'kz' then types.name_full_kz "
                            + " else types.name_full "
                            + " end  as property_type, "
                            + " sum(case when consumption < 100 then 1 else 0 end) as v_100, "
                            + " sum(case when consumption >= 100 and consumption < 1500 then 1 else 0 end) as v_100_1500, "
                            + " sum(case when consumption >= 1500 and consumption < 10000 then 1 else 0 end) as v_1500_10000, "
                            + " sum(case when consumption >= 10000 and consumption < 25000 then 1 else 0 end) as v_10000_25000, "
                            + " sum(case when consumption >= 25000 and consumption < 50000 then 1 else 0 end) as v_25000_50000, "
                            + " sum(case when consumption >= 50000 and consumption < 75000 then 1 else 0 end) as v_50000_75000, "
                            + " sum(case when consumption >= 75000 and consumption < 100000 then 1 else 0 end) as v_75000_100000, "
                            + " sum(case when consumption >= 100000 then 1 else 0 end) as v_100000 "
                            + "  "
                            + " from "
                            + " v_fs_code as types inner join "
                            + " ( "
                            + " select "
                            + " u.\"Id\", "
                            + " u.\"FSCode\", "
                            + " sum(coalesce(( "
                            + " select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                            + " from "
                            + " \"SUB_Form\" f, "
                            + " \"SUB_Form2Record\" f2, "
                            + " \"SUB_DIC_TypeResource\" tr "
                            + " where f.\"UserId\"=u.\"Id\" and f2.\"FormId\"=f.\"Id\" and f2.\"TypeResourceId\"=tr.\"Id\" "
                            + " and f.\"ReportYear\" = " + year + " "
                            + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " ), 0)) as consumption "
                            + "  "
                            + " from "
                            + " \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u "
                            + " where "
                            + " rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\" "
                            + " and r.\"ReportYear\" =  " + year + " "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false "
                            + " group by u.\"Id\", u.\"FSCode\" "
                            + " ) as table_data on table_data.\"FSCode\"=types.id "
                            + "  "
                            + " group by types.id, types.name_full, types.name_full_kz "
                            + " order by types.id ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //12
        public Dictionary<string, object> ger_econ_activity_consumption_top100(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " labels.name as consumption_group, "
                            + " count(data.\"Id\") as qty_subject, "
                            + " sum(data.consumption) as consumption, "
                            + " ( "
                            + " select "
                            + " count(distinct u.\"Id\") "
                            + " from "
                            + " \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u "
                            + " where "
                            + " rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\" "
                            + " and r.\"ReportYear\" = " + year + " "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false "
                            + " ) as all_qty, "
                            + " ( "
                            + " select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                            + " from "
                            + " \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u, "
                            + " \"SUB_Form\" f, "
                            + " \"SUB_Form2Record\" f2, "
                            + " \"SUB_DIC_TypeResource\" tr "
                            + " where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\" "
                            + " and f.\"UserId\" = u.\"Id\" and f2.\"FormId\" = f.\"Id\" and f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " and f.\"ReportYear\" = " + year + " "
                            + " and r.\"ReportYear\" = " + year + " "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " ) as all_consumption "
                            + " from "
                            + " ( "
                            + " select 1 as id, 'до 100' as name "
                            + " union all "
                            + " select 2 as id, 'от 100 до 1500' as name "
                            + " union all "
                            + " select 3 as id, 'от 1500 до 10000' as name "
                            + " union all "
                            + " select 4 as id, 'от 10000 до 25000' as name "
                            + " union all "
                            + " select 5 as id, 'от 25000 до 50000' as name "
                            + " union all "
                            + " select 6 as id, 'от 50000 до 75000' as name "
                            + " union all "
                            + " select 7 as id, 'от 75000 до 100000' as name "
                            + " union all "
                            + " select 8 as id, 'от 100000 и выше' as name "
                            + " ) as labels inner join "
                            + " ( "
                            + " select "
                            + " t1.\"Id\", "
                            + " t1.consumption, "
                            + " case "
                            + " when coalesce(consumption, 0) < 100 then 1 "
                            + " when consumption >= 100 and consumption < 1500 then 2 "
                            + " when consumption >= 1500 and consumption < 10000 then 3 "
                            + " when consumption >= 10000 and consumption < 25000 then 4 "
                            + " when consumption >= 25000 and consumption < 50000 then 5 "
                            + " when consumption >= 50000 and consumption < 75000 then 6 "
                            + " when consumption >= 75000 and consumption < 100000 then 7 "
                            + " when consumption >= 100000 then 8 "
                            + " else 0 end as group_id "
                            + " from "
                            + " ( "
                            + " select "
                            + " u.\"Id\", "
                            + " sum(coalesce(( select sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                            + " from \"SUB_Form\" f, \"SUB_Form2Record\" f2, \"SUB_DIC_TypeResource\" tr "
                            + " where f.\"UserId\" = u.\"Id\" and f2.\"FormId\" = f.\"Id\" and f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " and f.\"ReportYear\" = " + year + " "
                            + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " ), 0)) as consumption "
                            + " from \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u "
                            + " where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\" "
                            + " and r.\"ReportYear\" = " + year + " "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false  "
                            + " group by u.\"Id\" "
                            + " ) as t1 "
                            + " ) as data on labels.id = data.group_id "
                            + " group by labels.id, labels.name "
                            + " order by labels.id ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //13
        public Dictionary<string, object> ger_subject_name_consumption(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " u.\"JuridicalName\" as subject_name, "
                            + " coalesce(( "
                            + " select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                            + " from "
                            + " \"RST_ReportReestr\" rr, \"RST_Report\" r, "
                            + " \"SUB_Form\" f, "
                            + " \"SUB_Form2Record\" f2, "
                            + " \"SUB_DIC_TypeResource\" tr "
                            + " where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\"   "
                            + " and f.\"UserId\" = u.\"Id\" and f2.\"FormId\" = f.\"Id\" and f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " and f.\"ReportYear\" = " + year + " "
                            + " and r.\"ReportYear\" = " + year + "  "
                            + " and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false "
                            + " ), 0) as qty_consumption, "
                            + " case '" + lang + "' "
                            + " when 'ru' then kato.\"NameRu\" "
                            + " when 'kz' then kato.\"NameKz\" "
                            + " else kato.\"NameRu\" "
                            + " end as oblast_name "
                            + " from "
                            + " \"SEC_User\" u, \"DIC_Kato\" kato "
                            + " where "
                            + " u.\"Oblast\" = kato.\"Id\" "
                            + " order by 2 desc "
                            + " limit 100 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //14
        public Dictionary<string, object> ger_consumption_by_energy_resource(int year, string lang = "ru")
        {
            #region query
            string query = " select  case 'ru'  when 'ru' then res.\"NameRu\"  when 'kz' then res.\"NameKz\"  else res.\"NameRu\"  end  resource_name,  "
                     + "   ( "
                     + "   select  sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1))   "
                     + "      from  \"RST_ReportReestr\" rr, \"RST_Report\" r      "
                     + "      ,  \"SUB_Form\" f,  \"SUB_Form2Record\" f2,  \"SUB_DIC_TypeResource\" tr "
                     + "      where rr.\"ReportId\" = r.\"Id\"  "
                     + "      and rr.\"FormId\"= f.\"Id\" "
                     + "      and rr.\"IsExcluded\"=false "
                     + "      and f2.\"FormId\" = f.\"Id\" and f2.\"TypeResourceId\"=tr.\"Id\"       "
                     + "      and r.\"ReportYear\"="+year
                     + "      and res.\"Id\"=tr.\"Id\"      "
                     + "      and rr.\"IsDeleted\" = false  and r.\"IsDeleted\" = false      "
                     + "      and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false      "
                     + "   ) consumption_qty  "
                     + "   from   \"SUB_DIC_TypeResource\" as res  where \"Code\" = '2' order by \"Id\"  ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //15
        public Dictionary<string, object> ger_check_uncomplete_data(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " (select case '" + lang + "' "
                            + " when 'ru' then s.\"NameRu\"  "
                            + " when 'kz' then s.\"NameKz\"  "
                            + " else s.\"NameRu\"  "
                            + " end from \"SUB_DIC_Status\" s where s.\"Id\" = 5) status_name, "
                            + " ( "
                            + " select count(distinct u.\"Id\")  "
                            + " from \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u, \"SUB_Form\" f "
                            + " where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\" and f.\"UserId\"=u.\"Id\" "
                            + " and r.\"ReportYear\" =  " + year + " and f.\"ReportYear\" = " + year + " and u.\"FSCode\" = 1 and f.\"StatusId\" = 5 "
                            + " ) as state_inst_qty, "
                            + " ( "
                            + " select count(distinct u.\"Id\")  "
                            + " from \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u, \"SUB_Form\" f "
                            + " where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\" and f.\"UserId\"=u.\"Id\" "
                            + " and r.\"ReportYear\" =  " + year + " and f.\"ReportYear\" = " + year + " and u.\"FSCode\" = 2 and f.\"StatusId\" = 5 "
                            + " ) as quasi_state_inst_qty, "
                            + " ( "
                            + " select count(distinct u.\"Id\")  "
                            + " from \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SEC_User\" u, \"SUB_Form\" f "
                            + " where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" = u.\"Id\" and f.\"UserId\"=u.\"Id\" "
                            + " and r.\"ReportYear\" =  " + year + " and f.\"ReportYear\" = " + year + " and u.\"FSCode\" = 3 and f.\"StatusId\" = 5 "
                            + " ) as jur_qty ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //16
        public Dictionary<string, object> ger_check_uncomplete_data_by_oblast(int year, string lang = "ru")
        {
            #region query
            string query = " select kato.\"NameRu\" as oblast_name , "
                            + " tbl_1.* "
                            + "  from  "
                            + " \"DIC_Kato\" kato, "
                            + " ( "
                            + " 	select "
                            + " 'Запрошены недостающие сведения' as name, "
                            + " 15 as qty_state_inst, "
                            + " 22 as qty_quasi_state_inst, "
                            + " 34 as qty_jur "
                            + " union all "
                            + " select "
                            + " 'Получены ответы на запросы' as name, "
                            + " 75 as qty_state_inst, "
                            + " 54 as qty_quasi_state_inst, "
                            + " 46 as qty_jur "
                            + " ) tbl_1 "
                            + " where kato.\"refParent\"=1 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //17
        public Dictionary<string, object> ger_uncorrect_or_evaded_data_by_republic(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " 'Уклонившихся в отчетный период' as name, "
                            + " 15 as qty_state_inst, "
                            + " 22 as qty_quasi_state_inst, "
                            + " 34 as qty_jur "
                            + " union all "
                            + " select "
                            + " 'Недостоверных в отчетный период' as name, "
                            + " 75 as qty_state_inst, "
                            + " 54 as qty_quasi_state_inst, "
                            + " 46 as qty_jur "
                            + " union all "
                            + " select "
                            + " 'Уклонившихся - представили в полном объеме' as name, "
                            + " 41 as qty_state_inst, "
                            + " 57 as qty_quasi_state_inst, "
                            + " 64 as qty_jur "
                            + " union all "
                            + " select "
                            + " 'Недостоверных - представили в полном объеме' as name, "
                            + " 13 as qty_state_inst, "
                            + " 64 as qty_quasi_state_inst, "
                            + " 11 as qty_jur ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //18
        public Dictionary<string, object> ger_uncorrect_or_evaded_data_by_oblast(int year, string lang = "ru")
        {
            #region query
            string query = " ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //25
        public Dictionary<string, object> ger_energy_consumption_compare_previous(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                                + " res.\"NameRu\" as resource_name, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'гу' "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_state_inst, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'гу' "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_state_inst_prev, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'кв' "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_quasi_state_inst, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'кв' "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_quasi_state_inst_prev, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'юр' "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_jur, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'юр' "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_jur_prev "
                                + " from "
                                + " \"SUB_DIC_TypeResource\" res "
                                + " where res.\"IsDeleted\" = false "
                                + " order by res.\"Code\"  ";
            #endregion
            string sql1 = query;
            query = ResourceSQL.R25;
            string path = @"C:\publish\resourceSql.txt";
            #region write log       
            FileInfo fileInf = new FileInfo(path);
            if (fileInf.Exists)
            {
                using (var sw = System.IO.File.AppendText(path))
                {
                    sw.WriteLine("Date:" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    sw.WriteLine(query);
                }
            }
            else
            {
                using (var sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine("Date:" + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                    sw.WriteLine(query);
                }
            }
            #endregion


            if (!string.IsNullOrEmpty(query))
                query = query.Replace("<year>", year.ToString());

            var item = common.getTableList(query);
            return item;
        }

        //26
        public Dictionary<string, object> ger_energy_consumption_compare_previous_by_oblast(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                                + " kato.\"NameRu\" as oblast_name, "
                                + " res.\"NameRu\" as resource_name, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'гу' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_state_inst, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'гу' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_state_inst_prev, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'кв' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_quasi_state_inst, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'кв' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_quasi_state_inst_prev, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'юр' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_jur, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                                + " and f2.\"TypeResourceId\" = res.\"Id\" and a.\"Code\" = 'юр' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_jur_prev "
                                + " from "
                                + " \"DIC_Kato\" kato, "
                                + " \"SUB_DIC_TypeResource\" res "
                                + " where res.\"IsDeleted\" = false and kato.\"IsDeleted\" = false "
                                + " and "
                                + " kato.\"refParent\" = 1 "
                                + " order by kato.\"Code\", res.\"Code\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //27
        public Dictionary<string, object> ger_energy_consumption_share_by_oblast(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                                + " kato.\"NameRu\" as oblast_name, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and a.\"Code\" = 'гу' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_state_inst, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and a.\"Code\" = 'кв' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_quasi_state_inst, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and a.\"Code\" = 'юр' and rr.\"Oblast\" = kato.\"Id\" "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_jur, "
                                + " coalesce((select "
                                + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) "
                                + " from "
                                + " \"RST_ReportReestr\" rr inner join  "
                                + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                                + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                                + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                                + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                                + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                                + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                                + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + "  "
                                + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                                + " ), 0) as qty_all "
                                + " from "
                                + " \"DIC_Kato\" kato "
                                + " where kato.\"IsDeleted\" = false "
                                + " and kato.\"refParent\" = 1 "
                                + " order by kato.\"Code\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //28
        public Dictionary<string, object> ger_electricity_consumption_by_subject_top100(int year, string lang = "ru")
        {
            #region query
            string query = "select "
                            + " rr.\"IDK\", "
                            + " rr.\"OwnerName\", "
                            + " sum(coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" =  " + year + "  and r.\"ReportYear\" =  " + year + "  "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and tr.\"Id\" = 1 "
                            + " group by rr.\"IDK\", rr.\"OwnerName\" "
                            + " order by 3 desc "
                            + " limit 100 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //29
        public Dictionary<string, object> ger_heat_consumption_by_subject_top100(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " rr.\"IDK\", "
                            + " rr.\"OwnerName\", "
                            + " sum(coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" =  " + year + "  and r.\"ReportYear\" =  " + year + "  "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and tr.\"Id\" = 2 "
                            + " group by rr.\"IDK\", rr.\"OwnerName\" "
                            + " order by 3 desc "
                            + " limit 100 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //30
        public Dictionary<string, object> ger_gas_consumption_by_subject_top100(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " rr.\"IDK\", "
                            + " rr.\"OwnerName\", "
                            + " sum(coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" =  " + year + "  and r.\"ReportYear\" =  " + year + "  "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and tr.\"Id\" = 9 "
                            + " group by rr.\"IDK\", rr.\"OwnerName\" "
                            + " order by 3 desc "
                            + " limit 100 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //31
        public Dictionary<string, object> ger_coal_consumption_by_subject_top100(int year, string lang = "ru")
        {
            #region query
            string query = " select "
                            + " rr.\"IDK\", "
                            + " rr.\"OwnerName\", "
                            + " sum(coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" =  " + year + "  and r.\"ReportYear\" =  " + year + "  "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and tr.\"Id\" = 10 "
                            + " group by rr.\"IDK\", rr.\"OwnerName\" "
                            + " order by 3 desc "
                            + " limit 100 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //32
        public Dictionary<string, object> ger_consumption_compare_by_years(int year, string lang = "ru")
        {
            #region query
            string query = " select  "
                            + " 1 as subject_group, "
                            + " t1.subject_name, "
                            + " coalesce(t1.consumption, 0) as consumption, "
                            + " coalesce(t2.consumption, 0) as consumption_prev, "
                            + " coalesce(t3.consumption, 0) as consumption_prev_prev "
                            + " from ( "
                            + " select "
                            + " rr.\"OwnerName\" as subject_name, "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + " "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " group by rr.\"OwnerName\" "
                            + " having sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) > 100000 "
                            + " ) t1 left join "
                            + " ( "
                            + " select "
                            + " rr.\"OwnerName\" as subject_name, "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " group by rr.\"OwnerName\" "
                            + " ) t2 on t1.subject_name=t2.subject_name left join "
                            + " ( "
                            + " select "
                            + " rr.\"OwnerName\" as subject_name, "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + "-2 and r.\"ReportYear\" = " + year + "-2 "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " group by rr.\"OwnerName\" "
                            + " ) t3 on t1.subject_name=t3.subject_name "
                            + " union all "
                            + " select "
                            + " 2 as subject_group, "
                            + " 'Прочие субъекты ГЭР' as subject_name, "
                            + " coalesce(t1.consumption, 0) as consumption, "
                            + " coalesce(t2.consumption, 0) as consumption_prev, "
                            + " coalesce(t3.consumption, 0) as consumption_prev_prev "
                            + " from ( "
                            + " select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + " and r.\"ReportYear\" = " + year + " "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " ) t1, "
                            + " ( "
                            + " select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + "-1 and r.\"ReportYear\" = " + year + "-1 "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " ) t2, "
                            + " ( "
                            + " select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0)) * coalesce(tr.\"Keof\", 1)) consumption "
                            + " from "
                            + " \"RST_ReportReestr\" rr inner join  "
                            + " \"RST_Report\" r on rr.\"ReportId\" = r.\"Id\" inner join "
                            + " \"SEC_User\" u on rr.\"BINIIN\" = u.\"BINIIN\" inner join "
                            + " \"DIC_TypeApplication\" a on u.\"TypeApplicationId\" = a.\"Id\" inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + "-2 and r.\"ReportYear\" = " + year + "-2 "
                            + " and rr.\"IsDeleted\" = false and r.\"IsDeleted\" = false and u.\"IsDeleted\" = false and a.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " ) t3 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //33
        public Dictionary<string, object> ger_average_consumption_by_oblast(int year, string lang = "ru")
        {
            #region query
            string query = " select  kato.\"NameRu\" as oblast_name"
                            + "   ,(select  coalesce(sum(coalesce(f2.\"EnergyValue\", 0)),0) consumption  "
                            + "       from  \"SEC_User\" u"
                            + "       , \"RST_ReportReestr\" rr "
                            + "       inner join \"RST_Report\" r on r.\"Id\"=rr.\"ReportId\" and r.\"ReportYear\"="+year
                            + "       inner join  \"SUB_Form\" f on rr.\"FormId\"=f.\"Id\" "
                            + "       inner join  \"SUB_Form5Record\" f2 on f2.\"FormId\" = f.\"Id\"  and f2.energyindicator_id=5"
                            + "       inner join  \"DIC_OKED\" oked on rr.usrokedid = oked.\"Id\" and oked.\"RootId\" = 1281"
                            + "       where u.\"IsDeleted\" = false and f.\"IsDeleted\" = false and rr.\"IsDeleted\" = false  "
                            + "       and rr.\"Oblast\" = kato.\"Id\"  "
                            + "       and rr.\"UserId\"=u.\"Id\""
                            + "   ) as consumption_healthcare"
                            + "   ,(select  coalesce(sum(coalesce(f2.\"EnergyValue\", 0)),0) consumption  "
                            + "       from  \"SEC_User\" u"
                            + "       , \"RST_ReportReestr\" rr "
                            + "       inner join \"RST_Report\" r on r.\"Id\"=rr.\"ReportId\" and r.\"ReportYear\"="+year
                            + "       inner join  \"SUB_Form\" f on rr.\"FormId\"=f.\"Id\" "
                            + "       inner join  \"SUB_Form5Record\" f2 on f2.\"FormId\" = f.\"Id\"  and f2.energyindicator_id=5"
                            + "       inner join  \"DIC_OKED\" oked on rr.usrokedid = oked.\"Id\" and oked.\"RootId\" = 1261"
                            + "       where u.\"IsDeleted\" = false and f.\"IsDeleted\" = false and rr.\"IsDeleted\" = false                "
                            + "       and rr.\"Oblast\" = kato.\"Id\"  "
                            + "       and rr.\"UserId\"=u.\"Id\" "
                            + "   ) as consumption_education,  "
                            + "   (select  coalesce(sum(coalesce(f2.\"EnergyValue\", 0)),0) consumption  "
                            + "       from  \"SEC_User\" u"
                            + "       , \"RST_ReportReestr\" rr  "
                            + "       inner join \"RST_Report\" r on r.\"Id\"=rr.\"ReportId\" and r.\"ReportYear\"="+year
                            + "       inner join  \"SUB_Form\" f on rr.\"FormId\"=f.\"Id\" "
                            + "       inner join  \"SUB_Form5Record\" f2 on f2.\"FormId\" = f.\"Id\"  and f2.energyindicator_id=5"
                            + "       inner join  \"DIC_OKED\" oked on rr.usrokedid = oked.\"Id\" and oked.\"RootId\" = 1305"
                            + "       where u.\"IsDeleted\" = false and f.\"IsDeleted\" = false and rr.\"IsDeleted\" = false                "
                            + "       and rr.\"Oblast\" = kato.\"Id\"  "
                            + "       and rr.\"UserId\"=u.\"Id\""
                            + "   ) as consumption_culture  "
                            + "   from  \"DIC_Kato\" kato  where  kato.\"refParent\" = 1  and  kato.\"IsDeleted\" = false  order by kato.\"Code\" ";
            #endregion

            var item = common.getTableList(query);

            return item;
        }

        public Dictionary<string, object> ger_average_consumption_by_oblast_old(int year, string lang = "ru")
        {
            #region query
            string query = " select kato.\"Code\", "
                            + " kato.\"NameRu\" as oblast_name, "
                            + " (select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0))) consumption "
                            + " from "
                            + " \"SEC_User\" u inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + " "
                            + " and u.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and tr.\"Id\" = 2 and u.\"Oblast\" = kato.\"Id\" and oked.\"RootId\" = 1281 "
                            + " ) as consumption_healthcare, "
                            + " (select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0))) consumption "
                            + " from "
                            + " \"SEC_User\" u inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + " "
                            + " and u.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and tr.\"Id\" = 2 and u.\"Oblast\" = kato.\"Id\" and oked.\"RootId\" = 1261 "
                            + " ) as consumption_education, "
                            + " (select "
                            + " sum((coalesce(f2.\"OwnSource\", 0) + coalesce(f2.\"NotOwnSource\", 0))) consumption "
                            + " from "
                            + " \"SEC_User\" u inner join "
                            + " \"SUB_Form\" f on u.\"Id\" = f.\"UserId\" inner join "
                            + " \"SUB_Form2Record\" f2 on f2.\"FormId\" = f.\"Id\" inner join "
                            + " \"SUB_DIC_TypeResource\" tr on f2.\"TypeResourceId\" = tr.\"Id\" inner join "
                            + " \"DIC_OKED\" oked on u.\"OkedId\" = oked.\"Id\" "
                            + " where f.\"ReportYear\" = " + year + " "
                            + " and u.\"IsDeleted\" = false and f.\"IsDeleted\" = false and tr.\"IsDeleted\" = false "
                            + " and tr.\"Id\" = 2 and u.\"Oblast\" = kato.\"Id\" and oked.\"RootId\" = 1305 "
                            + " ) as consumption_culture "
                            + " from "
                            + " \"DIC_Kato\" kato "
                            + " where "
                            + " kato.\"refParent\" = 1 "
                            + " and "
                            + " kato.\"IsDeleted\" = false "
                            + " order by kato.\"Code\" ";
            #endregion

            List<string> keysToInclude = new List<string> { "oblast_name", "consumption_healthcare", "consumption_education", "consumption_culture" };
            var hlist = new List<Dictionary<string, object>>();
            var list = new List<Dictionary<string, object>>();
            string errorMessage = common.getTableWithList(ref list, query);
            foreach (var l in list)
            {
                if (year > 2017 && l["Code"].ToString() == "5100000000")
                {
                    l["consumption_healthcare"] = 0;
                    l["consumption_education"] = 0;
                    l["consumption_culture"] = 0;
                }

                var dict = l as Dictionary<string, object>;
                var keysToRemove = dict.Keys.Except(keysToInclude).ToList();

                foreach (var key in keysToRemove)
                    dict.Remove(key);

            }

            var rItem = new Dictionary<string, object>();
            rItem["ListItems"] = list;
            rItem["ErrorMessage"] = errorMessage;

            return rItem;
        }

        //34
        public Dictionary<string, object> ger_presence_of_measure_equipment_by_oblast(int year, string lang = "ru")
        {

            #region
            string query = "   select kato.\"Id\", kato.\"Code\" , kato.\"NameRu\" as oblast_name,  "
                                + "    (select  coalesce(avg(coalesce(f6.\"Equipment\", 0)),0)  "
                                + "      from   \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SUB_Form6Record\" f6      "
                                + "      where  rr.\"Oblast\" = kato.\"Id\" "
                                + "      and rr.usrfscode=3 "
                                + "      and rr.\"IsExcluded\"=false  "
                                + "      and r.\"ReportYear\"=2018  "
                                + "      and r.\"Id\"=rr.\"ReportId\" "
                                + "      and rr.\"FormId\"= f6.\"FormId\"  "
                                + "    ) state_inst_equipment_avg, "
                                + "    (select  coalesce(avg(coalesce(f6.\"Equipment\", 0)),0) "
                                + "      from   \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SUB_Form6Record\" f6      "
                                + "      where  rr.\"Oblast\" = kato.\"Id\"        "
                                + "      and rr.usrfscode=2 "
                                + "      and rr.\"IsExcluded\"=false  "
                                + "      and r.\"ReportYear\"="+year
                                + "      and r.\"Id\"=rr.\"ReportId\" "
                                + "      and rr.\"FormId\"= f6.\"FormId\"  "
                                + "    ) quasi_state_inst_equipment_avg,    "
                                + "    (select  coalesce(avg(coalesce(f6.\"Equipment\", 0)),0)    "
                                + "      from   \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SUB_Form6Record\" f6     "
                                + "      where  rr.\"Oblast\" = kato.\"Id\"       "
                                + "      and rr.usrfscode=1 "
                                + "      and rr.\"IsExcluded\"=false  "
                                + "      and r.\"ReportYear\"="+year
                                + "      and r.\"Id\"=rr.\"ReportId\" "
                                + "      and rr.\"FormId\"=  f6.\"FormId\" "
                                + "    ) jur_equipment_avg, "
                                + "    (select  coalesce(avg(coalesce(f6.\"Equipment\", 0)),0)    "
                                + "      from   \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SUB_Form6Record\" f6     "
                                + "      where  rr.\"Oblast\" = kato.\"Id\"       "
                                + "      and rr.usrfscode=4 "
                                + "      and rr.\"IsExcluded\"=false  "
                                + "      and r.\"ReportYear\"="+year
                                + "      and r.\"Id\"=rr.\"ReportId\" "
                                + "      and rr.\"FormId\"=f6.\"FormId\" "
                                + "    ) ip_equipment_avg       "
                                + "    from  "
                                + "   (select kto.\"Id\", kto.\"Code\", kto.\"NameRu\" ,  fsall.\"Oblast\" "
                                + "        from \"DIC_Kato\" as kto, "
                                + "       (select rr.\"Oblast\"  "
                                + "            from   \"RST_ReportReestr\" rr, \"RST_Report\" r, \"SUB_Form6Record\" f6      "
                                + "            where  "
                                + "            (rr.usrfscode >0 and rr.usrfscode <5) "
                                + "            and r.\"ReportYear\"="+year
                                + "            and r.\"Id\"=rr.\"ReportId\"  "
                                + "            and rr.\"FormId\"= f6.\"FormId\"  "
                                + "            group by rr.\"Oblast\" ) fsall       "
                                + "    where kto.\"Id\" =fsall.\"Oblast\") kato "
                                + "    order by kato.\"Code\"  ";

            #endregion
            var item = common.getTableList(query);
            
            return item;
        }

        //36
        public Dictionary<string, object> ger_water_consumption(int year, string lang = "ru")
        {
            #region query
            string query = "  ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        //37
        public Dictionary<string, object> ger_water_consumption_by_oblast(int year, string lang = "ru")
        {
            #region query
            string query = "select  "
                            + " kato.\"NameRu\" as oblast_name, "
                            + " res.\"NameRu\" as resource_name, "
                            + " ( "
                            + " select "
                            + " sum(coalesce(f3.\"ConsumptionVolume\", 0)) "
                            + " from "
                            + " \"SUB_Form3Record\" f3 inner join "
                            + " \"SUB_Form\" f on f3.\"FormId\"=f.\"Id\" inner join "
                            + " \"SEC_User\" u on f.\"UserId\"=u.\"Id\"  "
                            + " where "
                            + " u.\"IsCvazy\" = false and u.\"TypeApplicationId\" = 2 "
                            + " and "
                            + " u.\"Oblast\" = kato.\"Id\" "
                            + " and "
                            + " f3.\"KindResourceId\"=res.\"Id\" "
                            + " ) as state_inst_consumption_qty, "
                            + " ( "
                            + " select "
                            + " sum(coalesce(f3.\"ConsumptionVolume\", 0)) "
                            + " from "
                            + " \"SUB_Form3Record\" f3 inner join "
                            + " \"SUB_Form\" f on f3.\"FormId\"=f.\"Id\" inner join "
                            + " \"SEC_User\" u on f.\"UserId\"=u.\"Id\"  "
                            + " where "
                            + " u.\"IsCvazy\" = true "
                            + " and "
                            + " u.\"Oblast\" = kato.\"Id\" "
                            + " and "
                            + " f3.\"KindResourceId\"=res.\"Id\" "
                            + " ) as quasi_state_inst_consumption_qty, "
                            + " ( "
                            + " select "
                            + " sum(coalesce(f3.\"ConsumptionVolume\", 0)) "
                            + " from "
                            + " \"SUB_Form3Record\" f3 inner join "
                            + " \"SUB_Form\" f on f3.\"FormId\"=f.\"Id\" inner join "
                            + " \"SEC_User\" u on f.\"UserId\"=u.\"Id\"  "
                            + " where "
                            + " u.\"IsCvazy\" = false and u.\"TypeApplicationId\" in (1,3,4) "
                            + " and "
                            + " u.\"Oblast\" = kato.\"Id\" "
                            + " and "
                            + " f3.\"KindResourceId\"=res.\"Id\" "
                            + " ) as jur_consumption_qty "
                            + " from "
                            + " \"DIC_Kato\" kato, "
                            + " \"SUB_DIC_KindResource\" res "
                            + " where "
                            + " kato.\"refParent\" = 1  ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

		//42
		public Dictionary<string, object> report_view_by_oblast(int year,string lang="ru")
		{
			#region query
			string query =" /* показывает количество по областям */  		"
                    + " /* R42. обновление от 2019-12-20. убираем из выборки удаленных субъектов*/		"
                    + " /*   ,CASE WHEN u.\"FSCode\"= 1 THEN 'ЮР'    WHEN u.\"FSCode\"= '2' THEN 'КВ'    WHEN u.\"FSCode\"= '3' THEN 'ГУ'    WHEN u.\"FSCode\"= '4' THEN 'ИП' */      		"
                    + " select kt.\"Id\", case when 'ru'='"+lang+"' then kt.\"NameKz\"  else kt.\"NameRu\"  end  \"NameRu\"   		"
                    + " ,coalesce(gu16.countsub,0) gus16 ,coalesce(kv16.countsub,0) kvs16   ,coalesce(ur16.countsub,0) urs16 ,"
                    + " coalesce(ip16.countsub,0) ips16   ,coalesce(all16.countSub,0) alls16      		"
                    + " ,coalesce(gue16.countsub,0) guse16 ,coalesce(kve16.countsub,0) kvse16   ,coalesce(ure16.countsub,0) urse16   ,"
                    + " coalesce(ipe16.countsub,0) ipse16   ,coalesce(alle16.countSub,0) allse16      		"
                    + " ,coalesce(gup16.countsub,0) gusp16   ,coalesce(kvp16.countsub,0) kvsp16 ,coalesce(urp16.countsub,0) ursp16    ,"
                    + " coalesce(ipp16.countsub,0) ipsp16   ,coalesce(allp16.countSub,0) allsp16      		"
                    + " ,coalesce(guu16.countsub,0) gusu16    ,coalesce(kvu16.countsub,0) kvsu16   ,coalesce(uru16.countsub,0) ursu16   ,coalesce(ipu16.countsub,0) ipsu16 ,"
                    + " coalesce(allu16.countSub,0) allsu16   		"
                    + " from \"DIC_Kato\" kt 		"
                    + " /* общее количество по областям в разрезе гу кв юр ип */ 		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub        from  \"RST_ReportReestr\" rr , \"RST_Report\" r 		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"="+year+" and rr.usrfscode=3  		"
                    + " /* + 2019-12-20 */ and rr.\"IsDeleted\"=false         		"
                    + " GROUP by rr.\"Oblast\"       ) as gu16 on gu16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub        from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"="+year+" and rr.usrfscode=2 		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false          		"
                    + " GROUP by rr.\"Oblast\"       ) as kv16 on kv16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub        from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1 		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false          		"
                    + " GROUP by rr.\"Oblast\"       ) as ur16 on ur16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub        from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4   		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false        		"
                    + " GROUP by rr.\"Oblast\"       ) as ip16 on ip16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select t.\"Oblast\", count(t.*) countSub          from  \"RST_ReportReestr\" t , \"RST_Report\" t5   		"
                    + "  where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=" + year + "  		"
                    + "  /* + 2019-12-20*/ and t.\"IsDeleted\"=false		"
                    + " GROUP by t.\"Oblast\"       ) as all16 on all16.\"Oblast\"=kt.\"Id\"   		"
                    + " /* исключенных  */  		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r        		"
                    + "  where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=3 and rr.\"IsExcluded\"=True 		"
                    + "  /* + 2019-12-20*/ and rr.\"IsDeleted\"=false		"
                    + " GROUP by rr.\"Oblast\"       ) as gue16 on gue16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r 		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=2 and rr.\"IsExcluded\"=True		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false		"
                    + " GROUP by rr.\"Oblast\"       ) as kve16 on kve16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1 and rr.\"IsExcluded\"=True           		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false		"
                    + " GROUP by rr.\"Oblast\"       ) as ure16 on ure16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4 and rr.\"IsExcluded\"=True     		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false      		"
                    + " GROUP by rr.\"Oblast\"       ) as ipe16 on ipe16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.\"IsExcluded\"=True		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false           		"
                    + " GROUP by rr.\"Oblast\"       ) as alle16 on alle16.\"Oblast\"=kt.\"Id\"   		"
                    + " /* в реестре - перечень */  		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=3 and   rr.\"IsExcluded\"=False  		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false        		"
                    + " GROUP by rr.\"Oblast\"       ) as gup16 on gup16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=2 and   rr.\"IsExcluded\"=False   		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false       		"
                    + " GROUP by rr.\"Oblast\"       ) as kvp16 on kvp16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1 and   rr.\"IsExcluded\"=False      		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false    		"
                    + " GROUP by rr.\"Oblast\"       ) as urp16 on urp16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4 and   rr.\"IsExcluded\"=False      		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false    		"
                    + " GROUP by rr.\"Oblast\"       ) as ipp16 on ipp16.\"Oblast\"=kt.\"Id\"    		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub          from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.\"IsExcluded\"=False		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false           		"
                    + " GROUP by rr.\"Oblast\"       ) as allp16 on allp16.\"Oblast\"=kt.\"Id\"   		"
                    + " /* уклонисты */  		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=3 and rr.\"ReasonId\"=6           		"
                    + " /* + 2019-12-20*/ and rr.\"IsDeleted\"=false		"
                    + " GROUP by rr.\"Oblast\"       ) as guu16 on guu16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub         from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=2 and rr.\"ReasonId\"=6   		"
                    + "  /* + 2019-12-20*/ and rr.\"IsDeleted\"=false        		"
                    + " GROUP by rr.\"Oblast\"       ) as kvu16 on kvu16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub          from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1 and rr.\"ReasonId\"=6    		"
                    + "  /* + 2019-12-20*/ and rr.\"IsDeleted\"=false       		"
                    + " GROUP by rr.\"Oblast\"       ) as uru16 on uru16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub          from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4 and rr.\"ReasonId\"=6           		"
                    + "  /* + 2019-12-20*/ and rr.\"IsDeleted\"=false		"
                    + " GROUP by rr.\"Oblast\"       ) as ipu16 on ipu16.\"Oblast\"=kt.\"Id\"   		"
                    + " left join (         select rr.\"Oblast\", count(rr.*) countSub          from  \"RST_ReportReestr\" rr , \"RST_Report\" r         		"
                    + " where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.\"ReasonId\"=6           		"
                    + "  /* + 2019-12-20*/ and rr.\"IsDeleted\"=false		"
                    + " GROUP by rr.\"Oblast\"       ) as allu16 on allu16.\"Oblast\"=kt.\"Id\"   		"
                    + " where kt.\"refParent\" =1   order by kt.\"NameRu\" 		";
            #endregion

            var list = new List<Dictionary<string, object>>();
			string errorMessage = common.getTableWithList(ref list, query);

			double all1 = 0, all2 = 0, all3 = 0, all4 = 0, all5 = 0;
			double exp1 = 0, exp2 = 0, exp3 = 0, exp4 = 0, exp5 = 0;
			double ger1 = 0, ger2 = 0, ger3 = 0, ger4 = 0, ger5 = 0;
			double dev1 = 0, dev2 = 0, dev3 = 0, dev4 = 0, dev5 = 0;

			foreach (var item in list)
			{
				#region foreach
				//----Всего				
				all1 += Convert.ToDouble(item["gus16"]);
				all2 += Convert.ToDouble(item["kvs16"]);
				all3 += Convert.ToDouble(item["urs16"]);
				all4 += Convert.ToDouble(item["ips16"]);
				all5 += Convert.ToDouble(item["alls16"]);

				//----Исключение 				
				exp1 += Convert.ToDouble(item["guse16"]);
				exp2 += Convert.ToDouble(item["kvse16"]);
				exp3 += Convert.ToDouble(item["urse16"]);
				exp4 += Convert.ToDouble(item["ipse16"]);
				exp5 += Convert.ToDouble(item["allse16"]);

				//----Перечень ГЭР				 				
				ger1 += Convert.ToDouble(item["gusp16"]);
				ger2 += Convert.ToDouble(item["kvsp16"]);
				ger3 += Convert.ToDouble(item["ursp16"]);
				ger4 += Convert.ToDouble(item["ipsp16"]);
				ger5 += Convert.ToDouble(item["allsp16"]);

				//----Уклонисты			 				
				dev1 += Convert.ToDouble(item["gusu16"]);
				dev2 += Convert.ToDouble(item["kvsu16"]);
				dev3 += Convert.ToDouble(item["ursu16"]);
				dev4 += Convert.ToDouble(item["ipsu16"]);
				dev5 += Convert.ToDouble(item["allsu16"]);
				#endregion
			}

			list.Add(new Dictionary<string, object>());
			list.Last()["NameRu"] = "Всего";

			//----Всего	
			list.Last()["gus16"] = all1;
			list.Last()["kvs16"] = all2;
			list.Last()["urs16"] = all3;
			list.Last()["ips16"] = all4;
			list.Last()["alls16"] = all5;

			//----Исключение 
			list.Last()["guse16"] = exp1;
			list.Last()["kvse16"] = exp2;
			list.Last()["urse16"] = exp3;
			list.Last()["ipse16"] = exp4;
			list.Last()["allse16"] = exp5;

			//----Перечень ГЭР	
			list.Last()["gusp16"] = ger1;
			list.Last()["kvsp16"] = ger2;
			list.Last()["ursp16"] = ger3;
			list.Last()["ipsp16"] = ger4;
			list.Last()["allsp16"] = ger5;

			//----Уклонисты		
			list.Last()["gusu16"] = dev1;
			list.Last()["kvsu16"] = dev2;
			list.Last()["ursu16"] = dev3;
			list.Last()["ipsu16"] = dev4;
			list.Last()["allsu16"] = dev5;

			var rItem = new Dictionary<string, object>();
			rItem["ListItems"] = list;
			rItem["ErrorMessage"] = errorMessage;
			return rItem;
		}
        public Dictionary<string, object> report_view_by_oblast_old(int year, string lang = "ru")
        {
            #region query
            string query = " /* показывает количество по областям */ "
                + " /*  "
                + " ,CASE WHEN u.\"FSCode\"= 1 THEN 'ЮР'  "
                + "  WHEN u.\"FSCode\"= '2' THEN 'КВ'  "
                + "  WHEN u.\"FSCode\"= '3' THEN 'ГУ'  "
                + "  WHEN u.\"FSCode\"= '4' THEN 'ИП' */  "
                + "   "
                + " select kt.\"Id\", case when '" + lang + "'='kz' then kt.\"NameKz\"  else kt.\"NameRu\"  end  \"NameRu\"  "  //kt.\"NameRu\"
                + " ,coalesce(gu16.countsub,0) gus16  "
                + " ,coalesce(kv16.countsub,0) kvs16  "
                + " ,coalesce(ur16.countsub,0) urs16  "
                + " ,coalesce(ip16.countsub,0) ips16  "
                + " ,coalesce(all16.countSub,0) alls16  "
                + "   "
                + " ,coalesce(gue16.countsub,0) guse16  "
                + " ,coalesce(kve16.countsub,0) kvse16  "
                + " ,coalesce(ure16.countsub,0) urse16  "
                + " ,coalesce(ipe16.countsub,0) ipse16  "
                + " ,coalesce(alle16.countSub,0) allse16  "
                + "   "
                + " ,coalesce(gup16.countsub,0) gusp16  "
                + " ,coalesce(kvp16.countsub,0) kvsp16  "
                + " ,coalesce(urp16.countsub,0) ursp16  "
                + " ,coalesce(ipp16.countsub,0) ipsp16  "
                + " ,coalesce(allp16.countSub,0) allsp16  "
                + "   "
                + " ,coalesce(guu16.countsub,0) gusu16  "
                + " ,coalesce(kvu16.countsub,0) kvsu16  "
                + " ,coalesce(uru16.countsub,0) ursu16  "
                + " ,coalesce(ipu16.countsub,0) ipsu16  "
                + " ,coalesce(allu16.countSub,0) allsu16  "
                + " from \"DIC_Kato\" kt   "
                + " /* общее количество по областям в разрезе гу кв юр ип */ "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=3  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as gu16 on gu16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=2  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as kv16 on kv16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as ur16 on ur16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as ip16 on ip16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select t.\"Oblast\", count(t.*) countSub  "
                + "       from  \"RST_ReportReestr\" t , \"RST_Report\" t5  "
                + "       where t.\"ReportId\"=t5.\"Id\" and t5.\"ReportYear\"=" + year + "  "
                + "         GROUP by t.\"Oblast\"  "
                + "     ) as all16 on all16.\"Oblast\"=kt.\"Id\"  "
                + " /* исключенных  */ "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=3 and rr.\"IsExcluded\"=True  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as gue16 on gue16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=2 and rr.\"IsExcluded\"=True  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as kve16 on kve16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1 and rr.\"IsExcluded\"=True  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as ure16 on ure16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4 and rr.\"IsExcluded\"=True  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as ipe16 on ipe16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.\"IsExcluded\"=True  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as alle16 on alle16.\"Oblast\"=kt.\"Id\"  "
                + " /* в реестре - перечень */ "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=3 and   rr.\"IsExcluded\"=False "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as gup16 on gup16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=2 and   rr.\"IsExcluded\"=False "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as kvp16 on kvp16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1 and   rr.\"IsExcluded\"=False "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as urp16 on urp16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4 and   rr.\"IsExcluded\"=False "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as ipp16 on ipp16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.\"IsExcluded\"=False  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as allp16 on allp16.\"Oblast\"=kt.\"Id\"  "
                + " /* уклонисты */ "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=3 and rr.\"ReasonId\"=6  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as guu16 on guu16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=2 and rr.\"ReasonId\"=6  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as kvu16 on kvu16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=1 and rr.\"ReasonId\"=6  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as uru16 on uru16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.usrfscode=4 and rr.\"ReasonId\"=6  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as ipu16 on ipu16.\"Oblast\"=kt.\"Id\"  "
                + " left join (  "
                + "       select rr.\"Oblast\", count(rr.*) countSub  "
                + "       from  \"RST_ReportReestr\" rr , \"RST_Report\" r  "
                + "       where rr.\"ReportId\"=r.\"Id\" and r.\"ReportYear\"=" + year + " and rr.\"ReasonId\"=6  "
                + "         GROUP by rr.\"Oblast\"  "
                + "     ) as allu16 on allu16.\"Oblast\"=kt.\"Id\"  "
                + " where kt.\"refParent\" =1  "
                + " order by kt.\"NameRu\" ";
            #endregion

            var list = new List<Dictionary<string, object>>();
            string errorMessage = common.getTableWithList(ref list, query);

            double all1 = 0, all2 = 0, all3 = 0, all4 = 0, all5 = 0;
            double exp1 = 0, exp2 = 0, exp3 = 0, exp4 = 0, exp5 = 0;
            double ger1 = 0, ger2 = 0, ger3 = 0, ger4 = 0, ger5 = 0;
            double dev1 = 0, dev2 = 0, dev3 = 0, dev4 = 0, dev5 = 0;

            foreach (var item in list)
            {
                #region foreach
                //----Всего				
                all1 += Convert.ToDouble(item["gus16"]);
                all2 += Convert.ToDouble(item["kvs16"]);
                all3 += Convert.ToDouble(item["urs16"]);
                all4 += Convert.ToDouble(item["ips16"]);
                all5 += Convert.ToDouble(item["alls16"]);

                //----Исключение 				
                exp1 += Convert.ToDouble(item["guse16"]);
                exp2 += Convert.ToDouble(item["kvse16"]);
                exp3 += Convert.ToDouble(item["urse16"]);
                exp4 += Convert.ToDouble(item["ipse16"]);
                exp5 += Convert.ToDouble(item["allse16"]);

                //----Перечень ГЭР				 				
                ger1 += Convert.ToDouble(item["gusp16"]);
                ger2 += Convert.ToDouble(item["kvsp16"]);
                ger3 += Convert.ToDouble(item["ursp16"]);
                ger4 += Convert.ToDouble(item["ipsp16"]);
                ger5 += Convert.ToDouble(item["allsp16"]);

                //----Уклонисты			 				
                dev1 += Convert.ToDouble(item["gusu16"]);
                dev2 += Convert.ToDouble(item["kvsu16"]);
                dev3 += Convert.ToDouble(item["ursu16"]);
                dev4 += Convert.ToDouble(item["ipsu16"]);
                dev5 += Convert.ToDouble(item["allsu16"]);
                #endregion
            }

            list.Add(new Dictionary<string, object>());
            list.Last()["NameRu"] = "Всего";

            //----Всего	
            list.Last()["gus16"] = all1;
            list.Last()["kvs16"] = all2;
            list.Last()["urs16"] = all3;
            list.Last()["ips16"] = all4;
            list.Last()["alls16"] = all5;

            //----Исключение 
            list.Last()["guse16"] = exp1;
            list.Last()["kvse16"] = exp2;
            list.Last()["urse16"] = exp3;
            list.Last()["ipse16"] = exp4;
            list.Last()["allse16"] = exp5;

            //----Перечень ГЭР	
            list.Last()["gusp16"] = ger1;
            list.Last()["kvsp16"] = ger2;
            list.Last()["ursp16"] = ger3;
            list.Last()["ipsp16"] = ger4;
            list.Last()["allsp16"] = ger5;

            //----Уклонисты		
            list.Last()["gusu16"] = dev1;
            list.Last()["kvsu16"] = dev2;
            list.Last()["ursu16"] = dev3;
            list.Last()["ipsu16"] = dev4;
            list.Last()["allsu16"] = dev5;

            var rItem = new Dictionary<string, object>();
            rItem["ListItems"] = list;
            rItem["ErrorMessage"] = errorMessage;
            return rItem;
        }
        //43
        public Dictionary<string, object> EquipmentMeteringDevices(int year,string lang="ru")
        {
            string query = "";
            #region sql query
            query = "select  case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name  "
                    + ",  (  select  greatest( count(*), 1 )  "
                    + "     from  \"RST_ReportReestr\" r  "
                    + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                    + "     where  r.\"Oblast\" = kato.\"Id\"   "
                    + "  ) count_subjects "
                    + " "
                    + ",  (  select  count(*)   "
                    + "     from  \"RST_ReportReestr\" r  "
                    + "     inner join  \"SUB_Form6Record\" f6 on f6.\"FormId\" = r.\"FormId\" "
                    + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                    + "     where  r.\"Oblast\" = kato.\"Id\"  and   f6.\"CountDevice\">0 and f6. \"TypeCounterId\"=1 "
                    + "  ) electro "
                    + "      "
                    + "  ,  ( select    count(*) "
                    + "     from  \"RST_ReportReestr\" r  "
                    + "     inner join  \"SUB_Form6Record\" f6 on f6.\"FormId\" = r.\"FormId\" "
                    + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                    + "     where  r.\"Oblast\" = kato.\"Id\"  and   f6.\"CountDevice\">0 and f6. \"TypeCounterId\"=2 "
                    + "  ) teplo "
                    + "   "
                    + "    ,  (  select  count(*)   "
                    + "     from  \"RST_ReportReestr\" r  "
                    + "     inner join  \"SUB_Form6Record\" f6 on f6.\"FormId\" = r.\"FormId\" "
                    + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                    + "     where  r.\"Oblast\" = kato.\"Id\"  and   f6.\"CountDevice\">0 and f6. \"TypeCounterId\"=3 "
                    + "  ) voda "
                    + "   "
                    + "    ,  (  select  count(*)   "
                    + "     from  \"RST_ReportReestr\" r  "
                    + "     inner join  \"SUB_Form6Record\" f6 on f6.\"FormId\" = r.\"FormId\" "
                    + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                    + "     where  r.\"Oblast\" = kato.\"Id\"  and   f6.\"CountDevice\">0 and f6. \"TypeCounterId\"=4 "
                    + "  ) gaz   "
                    + "  from  \"DIC_Kato\" kato  where  kato.\"refParent\"=1 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> ReportConsumptionExpendituresByRegion(int year,string lang="kz")
        {
            #region sql query
            string query = " select case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name  "
                            + " ,  (  select  greatest( count(*), 1 )  "
                            + "      from  \"RST_ReportReestr\" r  "
                            + "      inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                            + "      where  r.\"Oblast\" = kato.\"Id\"   "
                            + "   ) count_subjects "
                            + "  "
                            + " ,  (  select  sum(f2.\"NotOwnSource\")    "
                            + "      from  \"RST_ReportReestr\" r  "
                            + "      inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\" and f2.\"TypeResourceId\"=2 "
                            + "      inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                            + "      where  r.\"Oblast\" = kato.\"Id\"  "
                            + "   ) notownsource "
                            + "       "
                            + "   ,  ( select  sum(f2.\"ExpenceEnergy\") "
                            + "      from  \"RST_ReportReestr\" r  "
                            + "      inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\" and f2.\"TypeResourceId\"=2 "
                            + "      inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                            + "      where  r.\"Oblast\" = kato.\"Id\"  "
                            + "   ) expenceenergy "
                            + "     "
                            + "   from  \"DIC_Kato\" kato  where  kato.\"refParent\"=1 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> ReportCountOfGuIndicators(int year, int? oblastId,string lang="ru")
        {
            #region query
            string query = " select case when '" + lang + "'='kz' then k.\"NameKz\"  else k.\"NameRu\"  end  oblast_name , u.\"JuridicalName\" as juridical_name, r.\"IDK\" as idk , "
                        + " case s.\"IsPlan\" when true then 'да' else 'нет' end as isplan , "
                        + " case s.\"IsEnergyManagementSystem\" when true then 'да' else 'нет' end as isenergymanagement_system,   "
                         + "gu2.\"CountOfEmployees\" as countOfEmployees, gu2.\"CountOfStudents\" as countOfStudents , gu2.\"CountOfBeds\" as countOfBeds, "
                        + " gu3.\"HeatedAreaOfBuilding\" as heatedAreaOfBuilding, gu3.\"TotalAreaOfBuilding\"  as totalAreaOfBuilding, gu3.\"YearOfConstruction\" as yearOfConstruction, "
                        + " case gu3.\"CentralHeating\" when 1 then 'да' else 'нет' end as centralHeating , "
                        + " case gu3.\"IndependentHeating\" when 1 then 'да' else 'нет' end as independentHeating , "
                        + " case gu3.\"AutomateItem\" when 1 then 'да' else 'нет' end as AutomateItem  "
                        + " from \"SUB_Form\" s "
                        + " inner join \"SEC_User\" u on u.\"Id\"=s.\"UserId\" "
                        + " inner join \"DIC_Kato\" k on k.\"Id\"=u.\"Oblast\"  "
                        + " inner join \"RST_ReportReestr\" r on r.\"FormId\"=s.\"Id\"  "
                        + " left join \"SUB_Form2Gu\" gu2 on gu2.\"FormId\"=s.\"Id\" "
                        + " left join \"SUB_Form3Gu\" gu3 on gu3.\"FormId\"=s.\"Id\"     "
                        + " where u.\"IsDeleted\"=false and s.\"ReportYear\"=" + year + " and  "
                        + " u.\"TypeApplicationId\" in (2,6,11,12,15,26,27) and k.\"Id\"=" + oblastId+" order by r.\"IDK\" ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string,object> ReportTutByRegion(int year,string lang="ru")
        {
            #region query
            string query = " select  case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name    , "
                               + "  ( select  greatest( count(*), 0 ) from  \"RST_ReportReestr\" r        "
                               + "  inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                               + "  where  r.\"Oblast\" = kato.\"Id\" ) count_subjects    ,  "
                               + "  (  select   COALESCE(sum(( COALESCE(f2.\"NotOwnSource\",0)+COALESCE(f2.\"OwnSource\",0))*d.\"Keof\"),0)         "
                               + "     from  \"RST_ReportReestr\" r "
                               + "     inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\" "
                               + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                               + "     inner join \"SUB_DIC_TypeResource\" d on d.\"Id\"=f2.\"TypeResourceId\""
                               + "     where  r.\"Oblast\" = kato.\"Id\") tut,         "
                               + "  ( select  greatest( count(*), 0 ) from  \"RST_ReportReestr\" r        "
                               + "  inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                               + "  where  r.\"Oblast\" = kato.\"Id\" and r.\"IsExcluded\" is not true ) count_subjects_woexcl    ,  "
                               + "  (  select   COALESCE(sum(( COALESCE(f2.\"NotOwnSource\",0)+COALESCE(f2.\"OwnSource\",0))*d.\"Keof\"),0)         "
                               + "     from  \"RST_ReportReestr\" r "
                               + "     inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\" "
                               + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                               + "     inner join \"SUB_DIC_TypeResource\" d on d.\"Id\"=f2.\"TypeResourceId\""
                               + "     where  r.\"Oblast\" = kato.\"Id\" and r.\"IsExcluded\" is not true ) tut_woexcl          "
                               + "   from  \"DIC_Kato\" kato  where  kato.\"refParent\"=1 ";
            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> ReportTopTutByRegion(int year, int? oblastId, int? limit = 10,string lang="ru")
        {
            #region query
            string query = " select  case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name ,r.usrjuridicalname, "
                             + " sum(( COALESCE(f2.\"NotOwnSource\",0)+COALESCE(f2.\"OwnSource\",0))*d.\"Keof\") as tut          "
                             + " from  \"RST_ReportReestr\" r  "
                             + " inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\"  "
                             + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                             + " inner join \"SUB_DIC_TypeResource\" d on d.\"Id\"=f2.\"TypeResourceId\" "
                             + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\" "
                             + " where  kato.\"refParent\"=1 ";
            if (oblastId != null && oblastId != 0)
                query += " and kato.\"Id\"=" + oblastId;

            query += " group by  oblast_name ,r.usrjuridicalname  "
                       + " order by tut desc  limit " + limit;

            #endregion
            var item = common.getTableList(query);
            return item;
        }
        
        public Dictionary<string, object> ReportTop100TutByRegion(int year, int? oblastId,string lang="ru")
        {
            #region query
            string query = " select  case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name  ,r.usrjuridicalname, "
                             + " sum(( COALESCE(f2.\"NotOwnSource\",0)+COALESCE(f2.\"OwnSource\",0))*d.\"Keof\") as tut          "
                             + " from  \"RST_ReportReestr\" r  "
                             + " inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\"  "
                             + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                             + " inner join \"SUB_DIC_TypeResource\" d on d.\"Id\"=f2.\"TypeResourceId\" "
                             + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\" "
                             + " inner join \"SEC_User\" u on r.\"UserId\"=u.\"Id\"  "
                             + " inner join \"DIC_OKED\" ok on u.\"OkedId\"=ok.\"Id\" and ok.\"RootId\" in (68,121) "
                             + " where  kato.\"refParent\"=1 ";
            if (oblastId != null && oblastId != 0)
                query += " and kato.\"Id\"=" + oblastId;

            query += " group by  oblast_name ,r.usrjuridicalname  "
                       + " order by tut desc  limit 100";

            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> ReportSocialObjectsByRegion(int year, int? oblastId,string lang="ru")
        {
            #region query
            string query = " select  case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name  ,r.usrjuridicalname, "
                             + " sum(( COALESCE(f2.\"NotOwnSource\",0)+COALESCE(f2.\"OwnSource\",0))*d.\"Keof\") as tut          "
                             + " from  \"RST_ReportReestr\" r  "
                             + " inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\"  "
                             + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                             + " inner join \"SUB_DIC_TypeResource\" d on d.\"Id\"=f2.\"TypeResourceId\" "
                             + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\" "
                             + " inner join \"SEC_User\" u on r.\"UserId\"=u.\"Id\"  "
                             + " inner join \"DIC_OKED\" ok on u.\"OkedId\"=ok.\"Id\" and ok.\"RootId\" in (1281,1261,1305) "
                             + " where  kato.\"refParent\"=1 ";
            if (oblastId != null && oblastId != 0)
                query += " and kato.\"Id\"=" + oblastId;

            query += " group by  oblast_name ,r.usrjuridicalname  "
                       + " order by tut desc ";

            #endregion
            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string,object> ReportLowCostEventsPerformedSubjectsByRegion(int year,int? oblastId,string lang="ru")
        {
            #region query
            string query = " select case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name ,r.usrjuridicalname, f4.\"EventName\", "
                     + " COALESCE(f4.\"ActualInvest\",0) factinvest            "
                     + " from  \"RST_ReportReestr\" r           "
                     + " inner join  \"SUB_Form4Record\" f4 on f4.\"FormId\" = r.\"FormId\"                  "
                     + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=2018                 "
                     + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\"                     "
                     + " where   f4.\"EventName\" is not null and (f4.\"EventName\" <> '') is true           ";

            if (oblastId != null && oblastId != 0)
                query += " and kato.\"Id\"=" + oblastId;

            query += "  order by f4.\"ActualInvest\" limit 100   ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string,object> ReportEffectiveEventsPerformedBySubjectsGer(int year, int? oblastId,string lang="ru")
        {
            #region query
            string where = string.Empty;
            if (oblastId != null && oblastId != 0)
                where = " and kato.\"Id\"=" + oblastId;

            //string query = " select t.* from (             "
            //        + "     select  kato.\"NameRu\" as oblast_name ,r.usrjuridicalname, f4.\"EventName\"  "
            //        + "       , COALESCE(f4.\"ActualInvest\",0) factinvest, COALESCE(f4.\"InMoney\",0) inMoney  "
            //        + "       , COALESCE(f4.\"InMoney\",0)/COALESCE(NULLIF(f4.\"ActualInvest\",0),1) as crat "
            //        + "     from  \"RST_ReportReestr\" r   "
            //        + "     inner join  \"SUB_Form4Record\" f4 on f4.\"FormId\" = r.\"FormId\"              "
            //        + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"="+year
            //        + "     inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\" "
            //        + "     where   f4.\"EventName\" is not null and (f4.\"EventName\" <> '') is true " + where
            //        + "     order by f4.\"ActualInvest\" ) t         "
            //        + " order by t.crat desc ";
            string query = " select t.*, COALESCE(t.crat,0) as crat0 from ("
                           + " select  case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name ,r.usrjuridicalname, f4.\"EventName\", f4.\"TypeCounterId\", tc.\"NameRu\" as tc_nameru,"
                           + " tr.\"NameRu\" as tr_nameru                  "
                           + " ,COALESCE(f4.\"InKind\",0) as inkind                       "
                           + " , COALESCE(f2.\"NotOwnSource\",0)+ COALESCE(f2.\"OwnSource\",0) as f2source                        "
                           + " , COALESCE(f4.\"InKind\",0)/nullif((COALESCE(f2.\"NotOwnSource\" ,0)+COALESCE(f2.\"OwnSource\" ,0))  ,0)* 100 as crat                  "
                           + " from  \"RST_ReportReestr\" r                    "
                           + " inner join  \"SUB_Form4Record\" f4 on f4.\"FormId\" = r.\"FormId\"                          "
                           + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                           + " inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\"                      "
                           + " inner join  \"SUB_DIC_TypeCounter\" tc on tc.\"Id\" = f4.\"TypeCounterId\"                  "
                           + " inner join  \"SUB_DIC_TypeResource\" tr on tr.\"Id\" = f2.\"TypeResourceId\"                            "
                           + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\"                               "
                           + " where   f4.\"EventName\" is not null and (f4.\"EventName\" <> '') is true  " + where
                           + " and f4.\"TypeCounterId\" in (1,2)   "
                           + " and f4.\"TypeCounterId\" = f2.\"TypeResourceId\" "
                           + " order by f4.\"ActualInvest\" ) t  "
                           + " order by t.crat desc nulls last ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string,object> RegionEnergyConsumption(int year, int? oblastId,string lang="kz")
        {
            #region query
            string where = string.Empty;
            if (oblastId != null && oblastId != 0)
                where = " and kato.\"Id\"=" + oblastId;

            string query = " select  case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name ,r.usrjuridicalname, f5.\"energyindicator_id\", ei.nameru as ei_nameru, "
                    + "   f5.\"EnergyValue\" , eiprev.nameru as eiprev_nameru, f5prev.\"EnergyValue\" as prevEnergyValue            "
                    + "  from  \"RST_ReportReestr\" r            "
                    + "     inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\"          "
                    + "     inner join  \"SUB_Form5Record\" f5 on f5.\"FormId\" = r.\"FormId\"                   "
                    + "     inner join  \"sub_dic_energyindicator\" ei on ei.\"id\" = f5.\"energyindicator_id\"           "
                    + "     inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=" + year
                    + "     inner join  \"RST_ReportReestr\" rprev on rprev.\"UserId\"=r.\"UserId\"           "
                    + "     inner join \"RST_Report\" pprev on pprev.\"Id\" = rprev.\"ReportId\" and pprev.\"ReportYear\"=" + (year-1)
                    + "     inner join  \"SUB_Form5Record\" f5prev on f5prev.\"FormId\" = rprev.\"FormId\"                   "
                    + "     inner join  \"sub_dic_energyindicator\" eiprev on eiprev.\"id\" = f5prev.\"energyindicator_id\"           "
                    + "  where f5.\"energyindicator_id\" is not null and f5prev.\"energyindicator_id\" is not null           "
                    + "  and f5.\"energyindicator_id\" = f5prev.\"energyindicator_id\"  " + where
                    + "  order by r.\"Id\" ";

            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> ReportCompareRegionWithOtherRegion(int year,string lang="ru")
        {
            #region query           
            string query = " select case when '" + lang + "'='kz' then kato.\"NameKz\"  else kato.\"NameRu\"  end  oblast_name   ,               "
                               + " (select  greatest( count(*), 0 ) from  \"RST_ReportReestr\" r                      "
                               + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"="+year
                               + " where  r.\"Oblast\" = kato.\"Id\" ) count_subjects,                   "
                               + " (select  greatest( count(*), 0 ) from  \"RST_ReportReestr\" r                      "
                               + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"="+year
                               + " where  r.\"Oblast\" = kato.\"Id\" and r.\"IsExcluded\"is  true ) count_sub_ukl,                   "
                               + " (select  greatest( count(*), 0 ) from  \"RST_ReportReestr\" r                      "
                               + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"="+year
                               + " where  r.\"Oblast\" = kato.\"Id\" and r.\"ReasonId\"=6 ) count_sub_excl,              "
                               + " (select  COALESCE(sum(( COALESCE(f2.\"NotOwnSource\",0)+COALESCE(f2.\"OwnSource\",0))*d.\"Keof\"),0)                       "
                               + " from  \"RST_ReportReestr\" r                 "
                               + " inner join  \"SUB_Form2Record\" f2 on f2.\"FormId\" = r.\"FormId\"               "
                               + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"="+year
                               + " inner join \"SUB_DIC_TypeResource\" d on d.\"Id\"=f2.\"TypeResourceId\"              "
                               + " where  r.\"Oblast\" = kato.\"Id\") tut                       "
                               + " from  \"DIC_Kato\" kato  where  kato.\"refParent\"=1                 "
                               + " order by kato.\"Id\"  ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> ReportConsumptionSubjectsByGer(int year,int? oblastId, int? fscode, string lang="ru")
        {
            #region query
            string where = string.Empty;
            if (oblastId != null && oblastId != 0)
                where = "  and k.\"Id\"=" + oblastId;
            if (fscode != null && fscode != 0)
                where += " and fs.id=" + fscode;

            string query = "  select   case '" + lang + "'  when 'ru' then k.\"NameRu\"  when 'kz' then k.\"NameKz\" else k.\"NameRu\"  end as oblast_name    "
                            + " , rr.\"IDK\"  as idk   "
                            + " , rr.\"BINIIN\" as biniin    "
                            + " , case '" + lang + "'  when 'ru' then fs.name_short  when 'kz' then fs.name_short_kz else fs.name_short  end as fs_name    "
                            + " , rr.\"OwnerName\" as juridical_name  "
                            + " , COALESCE( case '" + lang + "'  when 'ru' then ei.nameru   when 'kz' then ei.namekz else ei.nameru  end     "
                            + "   , COALESCE(r5.\"IndicatorName\",'')) as indicator_name     "
                            + " , COALESCE(r5.\"UnitMeasure\",'') as unit_measure    "
                            + " , COALESCE( (case when (left( r5.\"CalcFormula\",1)='=')  then ''''||r5.\"CalcFormula\" else r5.\"CalcFormula\" end),'') as calc_formula     "
                            + " , COALESCE( r5.\"EnergyValue\",0) as energy_value    "
                            + " from       "
                            + "  \"RST_ReportReestr\" rr     "
                            + "  inner JOIN  \"SUB_Form\" f  on rr.\"FormId\"=f.\"Id\"     "
                            + "  inner join \"SUB_Form5Record\" r5 on r5.\"FormId\"=f.\"Id\"     "
                            + "  left join v_fs_code fs on fs.id=rr.usrfscode      "
                            + "  left join sub_dic_energyindicator ei on ei.id=r5.energyindicator_id     "
                            + "  inner join  \"DIC_Kato\" k on k.\"Id\"=rr.\"Oblast\"    "
                            + "  inner join \"RST_Report\" r on r.\"Id\"=rr.\"ReportId\" and r.\"ReportYear\"="+year
                            + " where      "
                            + "   rr.\"IsDeleted\"=false     "
                            + where
                            + "  order by k.\"Code\", rr.\"IDK\", r5.\"Id\"    ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        /// <summary>
        /// 55
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public Dictionary<string, object> ReportSetOfActivitiesPerformedSubjectsByGER(int year, int oblastId,string lang, int fscode,int okedId)
        {
            #region query
            string where =" where 1=1 ";
            if (oblastId != 0)
            {
                where += " and kato.\"Id\"=" + oblastId;
            }

            string fsCodeWhere = "";
            if (fscode != 0)
                fsCodeWhere += " and fs_i.id=" + fscode;

            string okedWhere = "";
            if (okedId != 0)
                okedWhere += " and oked_i.\"RootId\"=" + okedId;

            string query = "  select  kato.\"NameRu\" as oblast_name 		"
                            + " ,r.\"BINIIN\"		"
                            + " 		"
                            + " ,case '"+lang+"'  when 'ru' then oked_s.\"NameRu\"  when 'kz' then oked_s.\"NameKz\" else oked_s.\"NameRu\" end as oked_root  "
                            + " ,case '"+lang+"'  when 'ru' then fs.name_short  when 'kz' then fs.name_short_kz else fs.name_short  end as fs_short     "
                            + " 		"
                            + " ,r.usrjuridicalname    		"
                            + " ,f.\"IsPlan\"          "
                            + " ,f.\"IsNotEvents\"      		"
                            + " ,f.\"IsEnergyManagementSystem\"        		"
                            + " ,f4.\"EventName\"		"
                            + " ,f4.\"EmplPeriod\"		"
                            + " ,f4.\"PlanExpend\"		"
                            + " ,COALESCE(f4.\"ActualInvest\",0) as f_actualinvest		"
                            + " , case f4.\"TypeCounterId\"  when 5 then coalesce (f4.\"Note\",'') else  tc.\"NameRu\"   end as  tc_nameru    		"
                            + " ,COALESCE(f4.\"InKind\",0) as f_inkind   		"
                            + " ,COALESCE(f4.\"InMoney\",0) as f_inmoney 		"
                            + " 		"
                            + " ,case f4.\"TypeCounterId\" 		"
                            + "       when 1 then 		"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=1)		"
                            + "       when 2 then 		"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=2)		"
                            + "       when 4 then 		"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=9) 	"
                            + "       else 0 		"
                            + "   end as inkind_tut 		"
                            + " 		"
                            + " from  \"RST_ReportReestr\" r		"
                            + " inner join  \"SUB_Form\" f on f.\"Id\" = r.\"FormId\" 		"
                            + " inner join  \"SUB_Form4Record\" f4 on f4.\"FormId\" = r.\"FormId\" 		"
                            + " 		"
                            + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"="+year
                            + " inner join  \"SUB_DIC_TypeCounter\" tc on tc.\"Id\" = f4.\"TypeCounterId\"		"
                            + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\"           		 "
                            + " 		"
                            + " left join  \"DIC_OKED\" oked_r on  oked_r.\"Id\"=r.usrokedid               	"
                            + " left join  \"DIC_OKED\" oked_s on  oked_s.\"Id\"=oked_r.\"RootId\"    		"
                            + " left join  \"v_fs_code\" fs on fs.id=r.usrfscode "
                            + " inner join \"DIC_OKED\" oked_i on  oked_i.\"Id\"=r.usrokedid  "+okedWhere
                            + " inner join \"v_fs_code\" fs_i on fs_i.id=r.usrfscode  "+fsCodeWhere
                            + where
                            +"  union select    "
                            + " t.oblast_name	"
                            + " , t.\"BINIIN\"	"
                            + " ,null as oked_root	"
                            + " ,null as fs_short	"
                            + " , t.usrjuridicalname	"
                            + " ,null as IsPlan	"
                            + " ,null as IsNotEvents	"
                            + " ,null as IsEnergyManagementSystem	"
                            + " ,null as EventName	"
                            + " ,null as EmplPeriod	"
                            + " ,null as PlanExpend	"
                            + " ,null as f_actualinvest	"
                            + " ,null as tc_nameru	"
                            + " ,null as f_inkind	"
                            + " ,null as f_inmoney	"
                            + " , sum (t.inkind_tut) as inkind_tut 	"
                            + " from 	"
                            + " 	"
                            + " (select  kato.\"NameRu\" as oblast_name 	"
                            + " ,r.\"BINIIN\"	"
                            + " ,'Итого по ' ||r.usrjuridicalname as usrjuridicalname	"
                            + " ,case f4.\"TypeCounterId\" 	"
                            + "       when 1 then 	"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=1)	"
                            + "       when 2 then 	"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=2)	"
                            + "       when 4 then 	"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=9)	"
                            + "       else 0 	"
                            + "   end as inkind_tut 	"
                            + " 	"
                            + " from  \"RST_ReportReestr\" r	"
                            + " inner join  \"SUB_Form\" f on f.\"Id\" = r.\"FormId\" 	"
                            + " inner join  \"SUB_Form4Record\" f4 on f4.\"FormId\" = r.\"FormId\" 	"
                            + " 	"
                            + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"="+year
                            + " inner join  \"SUB_DIC_TypeCounter\" tc on tc.\"Id\" = f4.\"TypeCounterId\"	"
                            + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\" 	"
                            + " inner join \"DIC_OKED\" oked_i on  oked_i.\"Id\"=r.usrokedid  " + okedWhere
                            + " inner join \"v_fs_code\" fs_i on fs_i.id=r.usrfscode  " + fsCodeWhere
                            + where
                            + " ) t	"
                            + " group by 1,2,5	"
                            + " order by 1,2,3	";

           // query += where + "  order by  1,2,f4.\"Id\"   ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

        public Dictionary<string, object> ReportSetOfActivitiesPerformedSubjectsByGEROld(int year, int oblastId, int fscode, int okedId)
        {
            #region query
            string where = " where 1=1 ";
            if (oblastId != 0)
                where += " and kato.\"Id\"=" + oblastId;

            if (fscode != 0)
                where += " and fs.id=" + fscode;

            if (okedId != 0)
                where += " and oked_s.\"Id\"=" + okedId;

            string query = "  select  kato.\"NameRu\" as oblast_name 		"
                            + " ,r.\"BINIIN\"		"
                            + " 		"
                            + " ,case 'ru'  when 'ru' then oked_s.\"NameRu\"  when 'kz' then oked_s.\"NameKz\" else oked_s.\"NameRu\" end as oked_root  "
                            + " ,case 'ru'  when 'ru' then fs.name_short  when 'kz' then fs.name_short_kz else fs.name_short  end as fs_short     "
                            + " 		"
                            + " ,r.usrjuridicalname    		"
                            + " ,f.\"IsPlan\"          "
                            + " ,f.\"IsNotEvents\"      		"
                            + " ,f.\"IsEnergyManagementSystem\"        		"
                            + " ,f4.\"EventName\"		"
                            + " ,f4.\"EmplPeriod\"		"
                            + " ,f4.\"PlanExpend\"		"
                            + " ,COALESCE(f4.\"ActualInvest\",0) as f_actualinvest		"
                            + " , case f4.\"TypeCounterId\"  when 5 then coalesce (f4.\"Note\",'') else  tc.\"NameRu\"   end as  tc_nameru    		"
                            + " ,COALESCE(f4.\"InKind\",0) as f_inkind   		"
                            + " ,COALESCE(f4.\"InMoney\",0) as f_inmoney 		"
                            + " 		"
                            + " ,case f4.\"TypeCounterId\" 		"
                            + "       when 1 then 		"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=1)		"
                            + "       when 2 then 		"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=2)		"
                            + "       when 4 then 		"
                            + "        COALESCE(f4.\"InKind\",0)*(select COALESCE(\"SUB_DIC_TypeResource\".\"Keof\",0) from  \"SUB_DIC_TypeResource\" where \"SUB_DIC_TypeResource\".\"Id\"=9) 	"
                            + "       else 0 		"
                            + "   end as inkind_tut 		"
                            + " 		"
                            + " from  \"RST_ReportReestr\" r		"
                            + " inner join  \"SUB_Form\" f on f.\"Id\" = r.\"FormId\" 		"
                            + " inner join  \"SUB_Form4Record\" f4 on f4.\"FormId\" = r.\"FormId\" 		"
                            + " 		"
                            + " inner join \"RST_Report\" p on p.\"Id\" = r.\"ReportId\" and p.\"ReportYear\"=2018		"
                            + " inner join  \"SUB_DIC_TypeCounter\" tc on tc.\"Id\" = f4.\"TypeCounterId\"		"
                            + " inner join \"DIC_Kato\" kato on  kato.\"Id\"=r.\"Oblast\"           		 "
                            + " 		"
                            + " left join  \"DIC_OKED\" oked_r on  oked_r.\"Id\"=r.usrokedid               	"
                            + " left join  \"DIC_OKED\" oked_s on  oked_s.\"Id\"=oked_r.\"RootId\"    		"
                            + " left join  \"v_fs_code\" fs on fs.id=r.usrfscode ";

            query += where + "  order by  1,2,f4.\"Id\"   ";
            #endregion

            var item = common.getTableList(query);
            return item;
        }

    }
}