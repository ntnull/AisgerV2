using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aisger.Helpers;
using Aisger.Models.Entity.Dictionary;
using Aisger.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.Data.Entity;

namespace Aisger.Models.Repository.Action
{
    public class SubActionPlanRepository : AObjectRepository<SUB_ActionPlan>
    {
        public override string TitleObject
        {
            get { return ResourceSetting.SubActionPlan; }
        }

        public SUB_ActionPlan GetPreamble(long id)
        {
            var context = CreateDatabaseContext(false);
            var preamble = context.SUB_ActionPlan.Include(e => e.SUB_ActionTab1).Include(e => e.SUB_ActionTab2).Include(e => e.SUB_ActionTab3).AsNoTracking().FirstOrDefault(e => e.Id == id);
            return preamble;
        }

        public List<UnMappedDictionary> GetYears()
        {
            var reportsYear = GetAll().Where(e=>e.StatusId!=1).Select(e => e.ReportYear).Distinct();
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
        public int GetCountInbox()
        {
            return AppContext.SUB_ActionPlan.Count(
               e => e.StatusId == CodeConstManager.STATUS_SEND_ID);
        }
        public SUB_ActionHistory SaveHistory(SUB_ActionHistory history)
        {
            AppContext.SUB_ActionHistory.Add(history);
            AppContext.SaveChanges();
            return history;
        }
        public List<SUB_ActionPlan> GetListByOblast(List<string> oblasts)
        {
            return
                GetQueryByDescending(
                    e =>
                        !e.IsDeleted && e.StatusId != CodeConstManager.REG_STATUS_REESTR_ID &&
                        oblasts.Contains(e.SEC_User1.DIC_Kato.Code), true, e => e.Id).ToList();
        }

        public List<TermSearch> GetTerm(string term, long? userId)
        {
            var query = AppContext.SUB_DIC_Event.Where(e => e.NameRu.Contains(term) && e.UserId == userId).Select(x => new TermSearch() { Id = x.Id, Term = x.NameRu }).Distinct();
            return query.ToList();
        }
        public int CreateReportYear(long? userId)
        {
            if (!AppContext.SUB_ActionPlan.Any(e => e.UserId == userId))
            {
                return DateTime.Now.Year-1;
            }
            var min = AppContext.SUB_ActionPlan.Where(e => e.UserId == userId).Min(e => e.ReportYear);
            return min - 1;
        }
        public List<SUB_ActionPlan> GetListCurrentByUser(long? idUser)
        {
            if (idUser == null)
            {
                return GetAll();
            }
            return GetQueryByDescending(e => !e.IsDeleted && e.UserId == idUser, true, e => e.Id).ToList();
        }
        public SUB_ActionComment GetComments(long modelId, string nameTable, int colIndex, int rowIndex)
        {
            var model =
                AppContext.SUB_ActionComment.FirstOrDefault(
                    e => e.ColumnIndex == colIndex && e.ActionId == modelId && e.RowIndex == rowIndex && e.TableName == nameTable);
            if (model == null)
            {
                return new SUB_ActionComment { IsError = true };
            }
            return model;
        }
        public void SaveComment(long modelId, string nameTable, int colIndex, int rowIndex, bool isError, string comment, long rowId, string fieldName, string fieldValue, long? getCurrentUserId)
        {
            var model =
               AppContext.SUB_ActionComment.FirstOrDefault(
                   e => e.ColumnIndex == colIndex && e.ActionId == modelId && e.RowIndex == rowIndex && e.TableName == nameTable) ??
               new SUB_ActionComment
               {
                   CreateDate = DateTime.Now,
                   ActionId = modelId,
                   RowIndex = rowIndex,
                   ColumnIndex = colIndex,
                   TableName = nameTable,
               };
            if (fieldName == "TypeResourceId")
            {
                long typeresourceId;
                if (long.TryParse(fieldValue, out typeresourceId))
                {
                    var typeResource = AppContext.SUB_DIC_TypeResource.FirstOrDefault(e => e.Id == typeresourceId);
                    if (typeResource != null)
                    {
                        fieldValue = typeResource.NameRu + " (" + typeResource.UnitName + ")";
                    }
                }

            }

            model.IsError = isError;
            model.SUB_ActionComRecord.Add(new SUB_ActionComRecord { CreateDate = DateTime.Now, Note = comment, UserId = getCurrentUserId, SUB_ActionComment = model, ValueField = fieldValue });
            if (model.Id == 0)
            {

                var history =
                    AppContext.SUB_ActionRecordHistory.FirstOrDefault(
                        e => e.TabName == nameTable && e.RecordId == rowId && e.FieldName == fieldName);
                if (history != null)
                {
                    model.Unique = history.Unique;
                }
                AppContext.SUB_ActionComment.Add(model);
            }
            AppContext.SaveChanges();
        }
        public SubUpdateField UpdateModel(string code, long modelId, long userId, long recordId, int year, string fieldName, string fieldValue, long editor, long typeId, long status = CodeConstManager.REG_STATUS_REESTR_ID)
        {
            SUB_ActionPlan form;
            if (modelId == 0)
            {
                form = AppContext.SUB_ActionPlan.FirstOrDefault(e => e.UserId == userId && e.ReportYear == year && !e.IsDeleted) ??
                       new SUB_ActionPlan
                       {
                           ReportYear = year,
                           BeginPlanYear = year,
                           EndPlanYear = year + 4,
                           CreateDate = DateTime.Now,
                           UserId = userId,
                           Editor = editor,
                           StatusId = status
                       };
            }
            else
            {
                form = GetById(modelId);
                if (form == null)
                {
                    return null;
                }
                if (form.ReportYear != year)
                {
                    form.ReportYear = year;
                }
            }
            switch (code)
            {
              
                case "tab1":
                    {
                        return UpdateTab1(form, recordId, fieldName, fieldValue, code, userId);
                    }
                case "tab2":
                    {
                        return UpdateTab2(form, recordId, fieldName, fieldValue, code, userId);
                    }
                case "tab3":
                    {
                        return UpdateTab3(form, recordId, fieldName, fieldValue, code, userId);
                    }
                case "main":
                    {
                        return UpdateMain(form, recordId, fieldName, fieldValue, code, userId);
                    }
            }
            return null;
        }
        private SubUpdateField UpdateTab3(SUB_ActionPlan form, long recordId, string fieldName, string fieldValue, string tabName, long? userId)
        {
            float? floatValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
                    fieldValue = fieldValue.Replace(".", ",");
                }
                if (double.TryParse(fieldValue, out db))
                {
                    floatValue = float.Parse(fieldValue);
                }
            }
            SUB_ActionTab3 entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_ActionTab3.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_ActionTab3 { SUB_ActionPlan = form };
            }
            var type = CodeConstManager.FLOAT_CODE;
            switch (fieldName)
            {
                case "ShareIndex":
                    {
                        entity.ShareIndex = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "UnitKoef":
                    {
                        entity.UnitKoef = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "Volume1":
                    {
                        entity.Volume1 = floatValue;
                        break;
                    }
                case "Volume2":
                    {
                        entity.Volume2 = floatValue;
                        break;
                    }
                case "Volume3":
                    {
                        entity.Volume3 = floatValue;
                        break;
                    }
                case "Volume4":
                    {
                        entity.Volume4 = floatValue;
                        break;
                    }
                case "Volume5":
                    {
                        entity.Volume5 = floatValue;
                        break;
                    }
            }
            if (entity.Id == 0)
            {
                AppContext.SUB_ActionTab3.Add(entity);
            }


            AppContext.SaveChanges();
            GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_ActionTab3).FullName, fieldName, fieldValue,
                             userId);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateTab2(SUB_ActionPlan form, long recordId, string fieldName, string fieldValue, string tabName, long? userId)
        {
            float? floatValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
                    fieldValue = fieldValue.Replace(".", ",");
                }
                if (double.TryParse(fieldValue, out db))
                {
                    floatValue = float.Parse(fieldValue);
                }
            }
            SUB_ActionTab2 entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_ActionTab2.FirstOrDefault(e => e.ActionId == form.Id && e.KindId == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_ActionTab2 { KindId = recordId, SUB_ActionPlan = form };
            }
            var type = CodeConstManager.FLOAT_CODE;
            switch (fieldName)
            {
                case "Volume1":
                    {
                        entity.Volume1 = floatValue;
                        break;
                    }
                case "Volume2":
                    {
                        entity.Volume2 = floatValue;
                        break;
                    }
                case "Volume3":
                    {
                        entity.Volume3 = floatValue;
                        break;
                    }
                case "Volume4":
                    {
                        entity.Volume4 = floatValue;
                        break;
                    }
                case "Volume5":
                    {
                        entity.Volume5 = floatValue;
                        break;
                    }
                case "Expend1":
                    {
                        entity.Expend1 = floatValue;
                        break;
                    }
                case "Expend2":
                    {
                        entity.Expend2 = floatValue;
                        break;
                    }
                case "Expend3":
                    {
                        entity.Expend3 = floatValue;
                        break;
                    }
                case "Expend4":
                    {
                        entity.Expend4 = floatValue;
                        break;
                    }
                case "Expend5":
                    {
                        entity.Expend5 = floatValue;
                        break;
                    }
                case "Potential":
                    {
                        entity.Potential = floatValue;
                        break;
                    }
                case "Note":
                    {
                        entity.Note = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "Possible":
                    {
                        entity.Possible = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "UnitName":
                    {
                        entity.UnitName = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
            }
            if (entity.Id == 0)
            {
                AppContext.SUB_ActionTab2.Add(entity);
            }

            AppContext.SaveChanges();
            GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_ActionTab2).FullName, fieldName, fieldValue,
                             userId);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateTab1(SUB_ActionPlan form, long recordId, string fieldName, string fieldValue, string tabName, long? userId)
        {
            float? floatValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
                    fieldValue = fieldValue.Replace(".", ",");
                }
                if (double.TryParse(fieldValue, out db))
                {
                    floatValue = float.Parse(fieldValue);
                }
            }
            SUB_ActionTab1 entity = null;
            var custom = fieldName.Split('_');
            var kindId = Convert.ToInt32(custom[1]);
            var kind = AppContext.SUB_DIC_KindTabOne.FirstOrDefault(e => e.Id == kindId);
            if (kind == null)
            {
                return null;
            }
            var code = kind.IndexCode + ".";
            if (kindId < 10)
            {
                code += "0";
            }
            code += recordId;

            if (recordId > 0 && form.Id > 0)
            {
                entity = AppContext.SUB_ActionTab1.FirstOrDefault(e => e.Code == code && e.ActionId == form.Id);
            }
            if (entity == null)
            {
                entity = new SUB_ActionTab1 { SUB_ActionPlan = form, Code = code, KindId = kindId };
            }
            var type = CodeConstManager.STRING_CODE;
			var cfield=custom[0];
            switch (cfield.ToString())
            {
                case "Year1":
                    {
                        entity.Year1 = fieldValue;
                        break;
                    }
                case "Year2":
                    {
                        entity.Year2 = fieldValue;
                        break;
                    }
                case "Year3":
                    {
                        entity.Year3 = fieldValue;
                        break;
                    }
                case "Year4":
                    {
                        entity.Year4 = fieldValue;
                        break;
                    }
                case "Year5":
                    {
                        entity.Year5 = fieldValue;
                        break;
                    }
                case "Note":
                    {
                        entity.Note = fieldValue;
                        break;
                    }
                case "Events":
                    {
                        entity.Events = fieldValue;
                        break;
                    }
                case "UnitName":
                    {
                        entity.UnitName = fieldValue;
                        break;
                    }
                case "BeginDateStr":
                    {
                        entity.BeginDateStr = fieldValue;
                        break;
                    }
                case "EndDateStr":
                    {
                        entity.EndDateStr = fieldValue;
                        break;
                    }
                case "Event":
                    {
                        var dicevent =
                            AppContext.SUB_DIC_Event.FirstOrDefault(
                                e => e.NameRu.ToLower().Trim() == fieldValue.ToLower().Trim());
                        if (dicevent == null)
                        {
                            dicevent = new SUB_DIC_Event();
                            dicevent.NameRu = fieldValue;
                            dicevent.UserId = userId;
                            AppContext.SUB_DIC_Event.Add(dicevent);
                            AppContext.SaveChanges();
                        }
                        entity.EventId = dicevent.Id;
                        break;
                    }                      
                case "Expend1":
                    {
                        entity.Expend1 = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "PaybackPeriod":
                    {
                        entity.PaybackPeriod = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "Expend2":
                    {
                        entity.Expend2 = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "Expend3":
                    {
                        entity.Expend3 = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "Expend4":
                    {
                        entity.Expend4 = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "Expend5":
                    {
                        entity.Expend5 = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
            }
            if (entity.Id == 0)
            {
                AppContext.SUB_ActionTab1.Add(entity);
            }

            AppContext.SaveChanges();

            GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_ActionTab1).FullName, fieldName, fieldValue,
                    userId);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        private void GetQuid(long formId, long recordId, string tabName, string type, string className, string fieldName, string fieldValue, long? userId)
        {
            var model = new SUB_ActionRecordHistory
            {
                ClassName = className,
                FieldName = fieldName,
                FieldValue = fieldValue,
                ActionId = formId,
                RecordId = recordId,
                TabName = tabName,
                TypeValue = type,
                UserId = userId,

            };

            var entity =
                AppContext.SUB_ActionRecordHistory.FirstOrDefault(
                    e => e.ClassName == className && e.RecordId == recordId);
            if (entity == null)
            {
                model.Unique = Guid.NewGuid().ToString();
            }
            else
            {
                model.Unique = entity.Unique;
            }


            AppContext.SUB_ActionRecordHistory.Add(model);
            AppContext.SaveChanges();

        }

        private SubUpdateField UpdateMain(SUB_ActionPlan form, long recordId, string fieldName, string fieldValue, string code, long userId)
        {

            switch (fieldName)
            {
                case "Note":
                    {
                        form.Note = fieldValue;
                        break;
                    }

                case "BeginPlanYear":
                    {
                        var years = Convert.ToInt32(fieldValue);
                        form.BeginPlanYear = years;
                        form.EndPlanYear = years + 4;
                        break;
                    }
            }
            if (form.Id == 0)
            {
                AppContext.SUB_ActionPlan.Add(form);
            }
            AppContext.SaveChanges();

            return new SubUpdateField { ModelId = form.Id, RecordId = recordId };
        }

        public void DeleteRecord(string code, long recordId)
        {
            switch (code)
            {
               
                case "tab3":
                    {
						var form5 = AppContext.SUB_ActionTab3.FirstOrDefault(e => e.Id == recordId);
                        if (form5 != null)
                        {
                            AppContext.SUB_ActionTab3.Remove(form5);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
             
                default:
                    {
                        if (code.Contains("tab1_"))
                        {
                            var codes = code.Split('_');
                            long actionId;
							if (!long.TryParse(codes[1], out actionId))
                            {
                                return;
                            }
                            var codeIndex = codes[2] + ".";
                            if (recordId < 10)
                            {
                                codeIndex = codeIndex + "0";
                            }
                            codeIndex = codeIndex + recordId;
							var tab1 = AppContext.SUB_ActionTab1.FirstOrDefault(e => e.ActionId == actionId && e.Code == codeIndex);
                            if (tab1 != null)
                            {
								AppContext.SUB_ActionTab1.Remove(tab1);
                                AppContext.SaveChanges();
                            }
                        }
                        break;
                    }
            }
        }

        public IEnumerable<SUB_ActionPlanFilter> GetCommonReestrsByFilter(SubActionCommonFilter filter)
        {
            var nameIndex = CultureHelper.GetDictionaryName("NameRu");

            string query =
                "select r.\"Id\",r.\"UserId\",r.\"IsBack\", u.\"IDK\",r.\"SendDate\" as \"SendDate\", u.\"Address\",u.\"BINIIN\",u.\"JuridicalName\" as SubjectName, " +
                "r.\"StatusId\"," +
                " s.\"" + nameIndex + "\"  as \"StatusName\", " +
                "k.\"" + nameIndex + "\" as \"OblastName\", " +
                "(case s.\"Code\" when 'ACCEPT' then 1   when 'FINISHED' then 1 when'REJECT' then 2 else 0 end) as \"StatusIndex\" " +
                "from public.\"SUB_ActionPlan\"  as r  " +
                "inner join public.\"SUB_DIC_Status\" as s on s.\"Id\"=r.\"StatusId\"  " +
                "inner join public.\"SEC_User\" as u on u.\"Id\"=r.\"UserId\"  " +
                "left join public.\"DIC_Kato\" as k on k.\"Id\"=u.\"Oblast\" " +
                "where ";
            if (!string.IsNullOrEmpty(filter.BINIIN))
            {
                query = query + " u.\"BINIIN\" like '%" + filter.BINIIN + "%' AND ";
            }
            if (!string.IsNullOrEmpty(filter.IDK))
            {
                query = query + " u.\"IDK\" like '%" + filter.IDK + "%' AND ";
            }
            if (!string.IsNullOrEmpty(filter.Adress))
            {
                query = query + " LOWER(u.\"Address\") like LOWER('%" + filter.Adress + "%') AND ";
            }
            if (!string.IsNullOrEmpty(filter.SubjectName))
            {
                query = query + " LOWER(u.\"JuridicalName\") like LOWER('%" + filter.SubjectName + "%') AND ";
            }

            if (filter.Statuses != null && filter.Statuses.Count > 0)
            {
                query = query + " r.\"StatusId\" IN (" + string.Join(",", filter.Statuses) + ") AND ";
            }
          

            if (filter.Oblasts != null && filter.Oblasts.Count > 0)
            {
                query = query + " r.\"Oblast\" IN (" + string.Join(",", filter.Oblasts) + ") AND ";
            }
            if (filter.SendId != null && filter.SendId.Value > 0)
            {
                switch (filter.SendId.Value)
                {
                    case CodeConstManager.SUB_REASON_SEND_ID:
                        {
                            query = query + "  f.\"IsBack\"='true' AND ";
                            break;
                        }
                    case CodeConstManager.SUB_REASON_NOTSEND_ID:
                        {
                            query = query + "  f.\"IsBack\"<>'true' AND ";
                            break;
                        }
                }
            }
           
            query = query + " r.\"StatusId\"<>1 and r.\"ReportYear\"= " + filter.ReportYear;
            query = query + " order by r.\"SendDate\" DESC NULLS LAST, r.\"Id\" DESC NULLS LAST";
            return AppContext.Database.SqlQuery<SUB_ActionPlanFilter>(query);
        }
        public List<SUB_ActionHistory> GetReestrReportByUserId(long id)
        {
            var model = GetById(id);
            if (model == null)
            {
                return new List<SUB_ActionHistory>();
            }

            return AppContext.SUB_ActionHistory.Where(e => e.ActionId==id).ToList();

        }
    }
}