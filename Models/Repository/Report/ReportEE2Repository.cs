using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aisger.Models.Repository.Report
{
    public class ReportEE2Repository
    {
        Common common = new Common();
        aisgerEntities dbContext = new aisgerEntities();

        //----для отчет
        public Dictionary<string, object> getReportEE2(int? oblast_id, string lang = "ru")
        {
            List<ReportEE2Item> reportItem = new List<ReportEE2Item>(); ;
            getR(ref reportItem, oblast_id, lang);

            int oked1 = 0, oked2 = 0, oked3 = 0, oked4 = 0, summa = 0;
            int okedTotal1 = 0, okedTotal2 = 0, okedTotal3 = 0, okedTotal4 = 0, summaTotal = 0;
            string oblast_name = "";

            var list = new List<Dictionary<string, object>>();

            var dicKato = dbContext.Database.SqlQuery<DIC_Kato>(" select  * from \"DIC_Kato\" d where d.\"refParent\"=1 and (-1=" + oblast_id + " or d.\"Id\"=" + oblast_id + ") order by d.\"NameRu\"   ").ToList();

            #region
            for (int i = 0; i < dicKato.Count; i++)
            {
                var buffer = reportItem.Where(x => x.Oblast_Id == dicKato[i].Id).ToList();
                oked1 = 0; oked2 = 0; oked3 = 0; oked4 = 0; summa = 0;

                foreach (var item in buffer)
                {
                    //----
                    if (item.Oked == 1261)
                    {
                        oked1++;
                        okedTotal1++;
                    }

                    //----
                    if (item.Oked == 1281)
                    {
                        oked2++;
                        okedTotal2++;
                    }

                    //----
                    if (item.Oked == -1)
                    {
                        oked3++;
                        okedTotal3++;
                    }

                    //----
                    if (item.Oked == 0)
                    {
                        oked4++;
                        okedTotal4++;
                    }

                    summa = oked1 + oked2 + oked3 + oked4;
                    
                }

                list.Add(new Dictionary<string, object>());
                list.Last()["oblast_name"] = (lang.Equals("kz")) ? dicKato[i].NameKz : dicKato[i].NameRu;
                list.Last()["oked1"] = oked1;
                list.Last()["oked2"] = oked2;
                list.Last()["oked3"] = oked3;
                list.Last()["oked4"] = oked4;
                list.Last()["summa"] = summa;
                summaTotal = summaTotal + summa;
            }
            #endregion

            if (oblast_id == -1) {
                list.Add(new Dictionary<string, object>());
                list.Last()["oblast_name"] = (lang.Equals("kz")) ? "БАРЛЫҒЫ" : "ИТОГО";
                list.Last()["oked1"] = okedTotal1;
                list.Last()["oked2"] = okedTotal2;
                list.Last()["oked3"] = okedTotal3;
                list.Last()["oked4"] = okedTotal4;
                list.Last()["summa"] = summaTotal;
            }

            var dict = new Dictionary<string, object>();
            dict["ListItems"] = list;

            return dict;
        }

        public Dictionary<string, object> getReportEE2Analyse(int oblast_id, string lang)
        {
            var dict = new Dictionary<string, object>();

            try
            {
                List<ReportEE2Item> reportItem = new List<ReportEE2Item>();

                getR(ref reportItem, oblast_id, lang);

                int oked1 = 0, oked2 = 0, oked3 = 0, oked4 = 0, summa = 0;
                string oblast_name = "";

                var list = new List<Dictionary<string, object>>();
                var dicKato = dbContext.Database.SqlQuery<DIC_Kato>(" select  * from \"DIC_Kato\" d where d.\"refParent\"=1 and (-1=" + oblast_id + " or d.\"Id\"=" + oblast_id + ") order by d.\"NameRu\"   ").ToList();

                #region
                for (int i = 0; i < dicKato.Count; i++)
                {
                    var buffer = reportItem.Where(x => x.Oblast_Id == dicKato[i].Id).ToList();
                    oked1 = 0; oked2 = 0; oked3 = 0; oked4 = 0; summa = 0;

                    foreach (var item in buffer)
                    {
                        //----
                        if (item.Oked == 1261)
                        {
                            oked1++;
                        }

                        //----
                        if (item.Oked == 1281)
                        {
                            oked2++;
                        }

                        //----
                        if (item.Oked == -1)
                        {
                            oked3++;
                        }

                        //----
                        if (item.Oked == 0)
                            oked4++;

                        summa = oked1 + oked2 + oked3 + oked4;
                    }

                    list.Add(new Dictionary<string, object>());
                    list.Last()["oblast_name"] = (lang.Equals("kz")) ? dicKato[i].NameKz : dicKato[i].NameRu;
                    list.Last()["oked1"] = oked1;
                    list.Last()["oked2"] = oked2;
                    list.Last()["oked3"] = oked3;
                    list.Last()["oked4"] = oked4;
                    list.Last()["summa"] = summa;
                }
                #endregion

                #region categories
                var dic_oked = dbContext.Database.SqlQuery<DIC_OKED>("select * from \"DIC_OKED\" where \"Id\" in (1261,1281)  order by \"Id\" ").ToList();
                var categories = new List<Dictionary<string, object>>();
                foreach (var item in dic_oked)
                {
                    categories.Add(new Dictionary<string, object>());
                    categories.Last()["oked_id"] = item.Id;
                    categories.Last()["oked_name"] = (lang.Equals("kz")) ? item.NameKz : item.NameRu;
                }
                categories.Add(new Dictionary<string, object>());
                categories.Last()["oked_id"] = -1; categories.Last()["oked_name"] = (lang.Equals("kz")) ? "Басқалары" : "Прочие";

                categories.Add(new Dictionary<string, object>());
                categories.Last()["oked_id"] = 0; categories.Last()["oked_name"] = (lang.Equals("kz")) ? "ОКЭД жоқ" : "Без ОКЭД";
                #endregion


                dict["ListItems"] = list;
                dict["Categories"] = categories;
                dict["ErrorMessage"] = "";
            }
            catch (Exception ex)
            {
                dict["ErrorMessage"] = ex.Message;
            }

            return dict;
        }

        public void getR(ref List<ReportEE2Item> ListItems, int? oblast_id, string lang)
        {
            #region query
            string query = " select * from ( select t.\"Id\" ,  t.\"JuridicalName\" ,t3.\"Id\" as \"Oblast_Id\",  "
                            + "  case '" + lang + "' when 'kz' then t3.\"NameKz\" else t3.\"NameRu\" end as \"Oblast_Name\" , t.\"OkedId\" , "
                            + "  case '" + lang + "' when 'kz' then t4.\"NameKz\" else t4.\"NameRu\" end as \"Oked_Name\" , t2.\"RootId\" as \"Oked\""
                            + "  from \"SEC_User\" t , \"SEC_UserKind\" t1 , \"DIC_OKED\" t2 , \"DIC_Kato\" t3 , \"DIC_OKED\" t4"
                            + "  where t.\"Id\"=t1.\"UserId\" and t1.\"KindId\"=5 and t.\"OkedId\"=t2.\"Id\" and t.\"Oblast\"=t3.\"Id\"  and (t2.\"Id\"=1261 or t2.\"RootId\"=1261) and t2.\"RootId\"=t4.\"Id\" and ( t3.\"Id\"=" + oblast_id + "  or -1=" + oblast_id + ")) o1"
                            + " union  "
                            + "  select * from ( select t.\"Id\" ,  t.\"JuridicalName\" ,t3.\"Id\" as \"Oblast_Id\",  "
                            + "  case '" + lang + "' when 'kz' then t3.\"NameKz\" else t3.\"NameRu\" end as \"Oblast_Name\" , t.\"OkedId\" , "
                            + "  case '" + lang + "' when 'kz' then t4.\"NameKz\" else t4.\"NameRu\" end as \"Oked_Name\" , t2.\"RootId\" as \"Oked\""
                            + "  from \"SEC_User\" t , \"SEC_UserKind\" t1 , \"DIC_OKED\" t2 , \"DIC_Kato\" t3 , \"DIC_OKED\" t4"
                            + "  where t.\"Id\"=t1.\"UserId\" and t1.\"KindId\"=5 and t.\"OkedId\"=t2.\"Id\" and t.\"Oblast\"=t3.\"Id\"  and (t2.\"Id\"=1281 or t2.\"RootId\"=1281) and t2.\"RootId\"=t4.\"Id\" and ( t3.\"Id\"=" + oblast_id + " or -1=" + oblast_id + ")) o2"
                            + " union  "
                            + "  select * from (    select t.\"Id\" ,  t.\"JuridicalName\" ,t3.\"Id\" as \"Oblast_Id\",  "
                            + "  case '" + lang + "' when 'kz' then t3.\"NameKz\" else t3.\"NameRu\" end as \"Oblast_Name\" , t.\"OkedId\" , "
                            + "  case '" + lang + "' when 'kz' then 'Басқалары' else 'Прочие' end as  \"Oked_Name\" , -1 as \"Oked\""
                            + "  from \"SEC_User\" t , \"SEC_UserKind\" t1 , \"DIC_OKED\" t2 , \"DIC_Kato\" t3 , \"DIC_OKED\" t4"
                            + "  where t.\"Id\"=t1.\"UserId\" and t1.\"KindId\"=5 and t.\"OkedId\"=t2.\"Id\" and t.\"Oblast\"=t3.\"Id\"  and (t2.\"Id\"<>1281 and t2.\"RootId\"<>1261  and t2.\"Id\"<>1261 and t2.\"RootId\"<>1281) and  t2.\"RootId\"=t4.\"Id\" and ( t3.\"Id\"=" + oblast_id + " or -1=" + oblast_id + ")) o3"
                            + " union "
                            + "  select * from ( select t.\"Id\" ,  t.\"JuridicalName\" ,t3.\"Id\" as \"Oblast_Id\",  "
                            + "  case '" + lang + "' when 'kz' then t3.\"NameKz\" else t3.\"NameRu\" end as \"Oblast_Name\" ,0 as \"OkedId\" , "
                            + "  case '" + lang + "' when 'kz' then 'ОКЭД жоқ' else 'Без ОКЭД' end as  \"Oked_Name\" , 0 as \"Oked\" "
                            + "  from \"SEC_User\" t , \"SEC_UserKind\" t1 , \"DIC_Kato\" t3 "
                            + "  where t.\"Id\"=t1.\"UserId\" and t1.\"KindId\"=5  and t.\"Oblast\"=t3.\"Id\"  and t.\"OkedId\" is null  and ( t3.\"Id\"=" + oblast_id + " or -1=" + oblast_id + ")) o4 ";

            #endregion

            try
            {

                var reportItem = dbContext.Database.SqlQuery<ReportEE2Item>(query).ToList();
                ListItems = reportItem;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class ReportEE2Item
    {
        public long Id { get; set; }
        public string JuridicalName { get; set; }
        public long Oblast_Id { get; set; }
        public string Oblast_Name { get; set; }
        public int OkedId { get; set; }
        public string Oked_Name { get; set; }
        public int Oked { get; set; }
        public int? RCount { get; set; }
    }
}