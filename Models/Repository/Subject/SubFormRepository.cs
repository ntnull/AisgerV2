using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.Linq;
using Aisger.Models.Constants;
using Aisger.Models.Entity.Dictionary;
using Aisger.Models.Repository.Security;
using Aisger.Utils;
using NPOI.SS.Formula.Functions;
using System.Data.Entity;
using Aisger.Helpers;
using Aisger.Models.Entity.Subject;
using Aisger.Models.Entity.Reestr;

namespace Aisger.Models.Repository.Subject
{
    public class SubFormRepository : AObjectRepository<SUB_Form>
    {
        // new methods begin
        public SUB_Form GetSubFormByYearAndUserId(long id, int year, long modelId)
        {
            return AppContext.SUB_Form.FirstOrDefault(e => e.ReportYear == year && e.UserId == id && e.Id != modelId);
        }
        // new methods end

        public override string TitleObject
        {
            get { return ResourceSetting.SubForm; }
        }

        public int CreateReportYear(long? userId)
        {
            if (!AppContext.SUB_Form.Any(e => e.UserId == userId && e.IsDeleted==false))
            {
                return DateTime.Now.Year-1;
            }
            var min = AppContext.SUB_Form.Where(e => e.UserId == userId && e.IsDeleted==false).Min(e => e.ReportYear);
            return min - 1;
        }

		public bool CheckIsExistSubForm(long? userId)
		{
			bool flag = false;
			var reportYear = CreateReportYearActual(userId);
			
			var row = AppContext.Database.SqlQuery<SUB_Form>(" select * from \"SUB_Form\"  where \"UserId\"=" + userId + "  and \"IsDeleted\"=false and \"ReportYear\"=" + reportYear).FirstOrDefault();
			if (row != null)
				flag = true;
			return flag;
		}

        public int GetFSCode(long userId, int year) {
            var row = AppContext.RST_ReportReestr.Include(x => x.RST_Report).FirstOrDefault(x => x.UserId == userId && x.RST_Report.ReportYear==year);
            return row != null ? row.usrfscode == null ? 1 : row.usrfscode.Value : 1;
        }

        public int CreateReportYearActual(long? userId)
		{
            if (userId == null)
                return DateTime.Now.Year - 1;

			string query = " select t2.\"ReportYear\" from \"RST_ReportReestr\" t , \"RST_Report\" t2 where t.\"ReportId\"=t2.\"Id\" and t2.isactive=true and t.\"UserId\"="+userId+" order by t2.\"ReportYear\" desc ";
			int year = AppContext.Database.SqlQuery<int>(query).FirstOrDefault();
			return year;
		}

        public int GetActiveReportYear()
        {
            string query = " select \"ReportYear\" from \"RST_Report\"  where isactive=true ";
            int year = AppContext.Database.SqlQuery<int>(query).FirstOrDefault();
            return year;
        }

        public RST_ReportCustom GetRST_Report()
        {
            string query = " select * from \"RST_Report\"  where isactive=true ";
            var row= AppContext.Database.SqlQuery<RST_ReportCustom>(query).FirstOrDefault();
            return row;
        }

        public SUB_Form GetPreamble(long id)
        {
            var context = CreateDatabaseContext(false);
            var subform = context.SUB_Form
                .Include(e => e.SEC_User1)
                .Include(e => e.SEC_User1.DIC_OKED)
                .Include(e => e.SEC_User1.SEC_UserOked)
                .Include(e => e.SEC_User1.SEC_UserOked.Select( s=> s.DIC_OKED))
                .Include(e => e.SUB_Form2Record)
                .Include(e => e.SUB_Form3Record)
                .Include(e => e.SUB_Form4Record)
                .Include(e => e.SUB_Form5Record)
                .Include(e=>e.SUB_Form2Gu)
                .Include(e=>e.SUB_Form3Gu)
                .Include(e => e.SUB_Form6Record).AsNoTracking().FirstOrDefault(e => e.Id == id);
            if (subform != null)
            {
                var okeds = subform.SubjectOkeds;
                okeds = subform.SubjectMainOked;
            }
            return subform;
        }

        public void UpdateReestr(SUB_Form form)
        {
            var report =AppContext.RST_ReportReestr.FirstOrDefault(e => e.UserId == form.UserId && e.RST_Report.ReportYear == form.ReportYear && e.IsDeleted==false);
            if (report != null && report.FormId==null)
            {
                report.FormId = form.Id;
                //----for log
                report.UserId = MyExtensions.GetCurrentUserId();
                AppContext.SaveChanges();
			}
		}

        public List<UnMappedDictionary> GetYears()
        {
            var reportsYear = AppContext.SUB_Form.Where(x=>x.IsDeleted==false).ToList().Select(e => e.ReportYear).Distinct();
            var list = new List<UnMappedDictionary>();
            foreach (var year in reportsYear)
            {
                list.Add(new UnMappedDictionary(year,year.ToString()));
            }
            list.Add(new UnMappedDictionary(1, "Все")); 
            return list;
        }
        public List<PastYear> GetPastYears(long? userId)
        {
            if (userId == null)
                return null;

            string sql = " select r.\"ReportYear\", rr.\"FormId\" as \"Id\" from \"RST_ReportReestr\" rr, \"RST_Report\" r  "
                         + " where rr.\"ReportId\" = r.\"Id\" and rr.\"UserId\" =" + userId+ "  and rr.\"FormId\" is not null "
                         + "   order by r.\"Id\" ";

            return AppContext.Database.SqlQuery<PastYear>(sql).ToList();
        }
        public List<UnMappedDictionary> GetCurrentYearsOld(long? userId)
        {
            return AppContext.SUB_Form.Where(e => e.UserId == userId && e.IsDeleted == false).ToList().Select(i => new UnMappedDictionary(i.Id, i.ReportYear.ToString(CultureInfo.InvariantCulture))).ToList();
        }

        public bool GetIsValidInfo(long? idUser)
        {
            if (idUser == null)
            {
                return false;
            }
            var user = new SecUserRepository().GetById(idUser.Value);
            if (string.IsNullOrEmpty(user.Email))
            {
                return false;
            }
            if (string.IsNullOrEmpty(user.LastName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(user.FirstName))
            {
                return false;
            }
           
            if (string.IsNullOrEmpty(user.ResponceFIO))
            {
                return false;
            }
            if (string.IsNullOrEmpty(user.ResponcePost))
            {
                return false;
            }
            if (user.TypeApplicationId == 0)
            {
                return false;
            }
            if (user.Oblast == null || user.Oblast == 0)
            {
                return false;
            }
            if (user.Region == null || user.Region == 0)
            {
                return false;
            }
            if (user.OkedId == null || user.OkedId == 0)
            {
                return false;
            }
            return true;
        }

        public SUB_FormHistory SaveHistory(SUB_FormHistory history)
        {
            //----for log
            history.authorid = MyExtensions.GetCurrentUserId();
            history.authorlogin = MyExtensions.GetCurrentUserLogin();

            AppContext.SUB_FormHistory.Add(history);
            AppContext.SaveChanges();
            return history;
        }

        public SUB_FormHistory GetHistory(long id)
        {
            var history = AppContext.SUB_FormHistory.FirstOrDefault(fh => fh.Id == id);
            return history;
        }

        public SUB_FormHistory GetHistoryBySubFormId(long id)
        {
            var history = AppContext.SUB_FormHistory.Where(fh => fh.FormId == id && fh.XmlSign != null)
                .OrderByDescending(fh => fh.Id).FirstOrDefault();
            return history;
        }

        public List<SUB_FormHistory> GetHistoryListBySubFormId(long id)
        {
            var history = AppContext.SUB_FormHistory.Where(fh => fh.FormId == id && fh.XmlSign != null)
                .OrderByDescending(fh => fh.Id).ToList();
            return history;
        }

        public List<SUB_Form> GetListCurrentByUser(long? idUser)
        {
            if (idUser == null)
            {
                return GetAll();
            }
            return GetQueryByDescending(e => !e.IsDeleted && e.UserId == idUser, true, e => e.Id).ToList();
        }

        public SubUpdateField UpdateModel(string code, long modelId, long userId, long recordId, int year, string fieldName, string fieldValue, long editor, long typeId,
            long status = CodeConstManager.REG_STATUS_REESTR_ID)
        {
            SubUpdateField subUpdateField = null;
            SUB_Form form;
            bool isNew = false;
            if (modelId == 0)
            {
                form = AppContext.SUB_Form.FirstOrDefault(e => e.UserId == userId && e.ReportYear == year && !e.IsDeleted) ??
                       new SUB_Form
                       {
                           ReportYear = year,
                           BeginPlanYear = year,
                           EndPlanYear = year + 4,
                           CreateDate = DateTime.Now,
                           UserId = userId,
                           Editor = editor,
                           StatusId = status,
                           authorid = MyExtensions.GetCurrentUserId(),
                           authorlogin = MyExtensions.GetCurrentUserLogin()
                       };
                if (status == CodeConstManager.STATUS_ACCEPT_ID)
                {
                    form.SendDate = DateTime.Now;
                }
                isNew = true;

            }
            else
            {
                form = GetById(modelId);
                if (form == null)
                {
                    return null;
                }
            }
            switch (code)
            {
                case "form2":
                    {
                        subUpdateField = UpdateFrom2(form, recordId, fieldName, fieldValue, code, userId, typeId, editor);
                        break;
                    }
                case "form3":
                    {
                        subUpdateField = UpdateFrom3(form, recordId, fieldName, fieldValue, code, userId, typeId, editor);
                        break;
                    }
                case "form4":
                    {
                        subUpdateField = UpdateFrom4(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form5":
                    {
                        subUpdateField = UpdateFrom5(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form6":
                    {
                        subUpdateField = UpdateFrom6(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form6TypeReource":
                    {
                        subUpdateField = UpdateFrom6TypeResource(form, recordId, fieldName, fieldValue, code, userId, typeId, editor);
                        break;
                    }
                case "form6KindReource":
                    {
                        subUpdateField = UpdateFrom6KindResource(form, recordId, fieldName, fieldValue, code, userId, typeId, editor);
                        break;
                    }
                case "tab1":
                    {
                        subUpdateField = UpdateTab1(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "tab2":
                    {
                        subUpdateField = UpdateTab2(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "tab3":
                    {
                        subUpdateField = UpdateTab3(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "tabKadastr":
                    {
                        subUpdateField = UpdateTabKadastr(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "main":
                    {
                        subUpdateField = UpdateMain(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form2Gu":
                    {
                        subUpdateField = UpdateFrom2Gu(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form3Gu":
                    {
                        subUpdateField = UpdateFrom3Gu(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "formGuLightingInfo":
                    {
                        subUpdateField = UpdateFromGuLightInfo(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form3GuRecord":
                    {
                        subUpdateField = UpdateFrom3GuRecord(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form3aGuRecord1":
                    {
                        subUpdateField = UpdateFrom3aGuRecord1(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }                    
                case "form3aGuRecord2":
                    {
                        subUpdateField = UpdateFrom3aGuRecord2(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
                case "form3aGuRecord3":
                    {
                        subUpdateField = UpdateFrom3aGuRecord3(form, recordId, fieldName, fieldValue, code, userId, editor);
                        break;
                    }
            }

            if (subUpdateField != null)
            {
                subUpdateField.IsNew = isNew;
            }

            return subUpdateField;
        }

        private SubUpdateField UpdateMain(SUB_Form form, long recordId, string fieldName, string fieldValue, string code, long userId, long editor)
        {

            if (fieldName == "IsRent")
            {
                if (fieldValue == "true")
                {
                    form.IsRent = true;
                }
                if (fieldValue == "false")
                {
                    form.IsRent = false;
                }
            }

            switch (fieldName)
            {
                case "Note":
                    {
                        form.Note = fieldValue;

                        var history = new SUB_FormHistory
                        {
                            FormId = form.Id,
                            UserId = MyExtensions.GetCurrentUserId(),
                            RegDate = DateTime.Now,
                            Note = fieldValue
                        };
                        AppContext.SUB_FormHistory.Add(history);
                        break;
                    }
               
            }

            //----for log
            form.authorid = MyExtensions.GetCurrentUserId();
            form.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (form.Id == 0)
            {
                AppContext.SUB_Form.Add(form);
            }
            AppContext.SaveChanges();

            return new SubUpdateField { ModelId = form.Id, RecordId = recordId };   
        }

        private SubUpdateField UpdateTabKadastr(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId, long editor)
        {
            SUB_FormKadastr entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_FormKadastr.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_FormKadastr { SUB_Form = form };
            }
            switch (fieldName)
            {
                case "KadastrNumber":
                    {
                        entity.KadastrNumber = fieldValue;
                        break;
                    }
                case "ObjectName":
                    {
                        entity.ObjectName = fieldValue;
                        break;
                    }
            }
            //----for log
            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (entity.Id == 0)
            {
                AppContext.SUB_FormKadastr.Add(entity);
            }

            AppContext.SaveChanges();

            GetQuid(form.Id, entity.Id, tabName, CodeConstManager.STRING_CODE, typeof(SUB_FormKadastr).FullName, fieldName, fieldValue,
                              editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateTab3(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId, long editor)
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
					fieldValue = fieldValue.Replace(",", "."); 
					floatValue = float.Parse(fieldValue, CultureInfo.InvariantCulture.NumberFormat);
					//floatValue = float.Parse(fieldValue);
				}
            }
            SUB_FormTab3 entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_FormTab3.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_FormTab3 { SUB_Form = form };
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
                AppContext.SUB_FormTab3.Add(entity);
            }


            AppContext.SaveChanges();
             GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_FormTab3).FullName, fieldName, fieldValue,
                              editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id};
        }

        private SubUpdateField UpdateTab2(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId,long editor)
        {
            float? floatValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
					fieldValue = fieldValue.Replace(".", ",");  //Replace(".", ",")
                }

				
				if (double.TryParse(fieldValue, out db))
				{
					fieldValue = fieldValue.Replace(",", "."); 
					floatValue = float.Parse(fieldValue, CultureInfo.InvariantCulture.NumberFormat);
					//floatValue = float.Parse(fieldValue);
				}
            }
            SUB_FormTab2 entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_FormTab2.FirstOrDefault(e => e.FormId == form.Id && e.KindId == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_FormTab2 { KindId = recordId, SUB_Form = form };
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
            }
            if (entity.Id == 0)
            {
                AppContext.SUB_FormTab2.Add(entity);
            }

            AppContext.SaveChanges();
           GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_FormTab2).FullName, fieldName, fieldValue,
                            editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id};
        }

        private SubUpdateField UpdateTab1(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId,long editor)
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
					fieldValue = fieldValue.Replace(",", ".");
					floatValue = float.Parse(fieldValue, CultureInfo.InvariantCulture.NumberFormat);
					//floatValue = float.Parse(fieldValue);
				}
            }
            SUB_FormTab1 entity = null;
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
                entity = AppContext.SUB_FormTab1.FirstOrDefault(e => e.Code == code && e.FormId == form.Id);
            }
            if (entity == null)
            {
                entity = new SUB_FormTab1 { SUB_Form = form, Code = code, KindId = kindId };
            }
            var type = CodeConstManager.STRING_CODE;
            switch (custom[0])
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
                case "Expend1":
                    {
                        entity.Expend1 = floatValue;
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
                AppContext.SUB_FormTab1.Add(entity);
            }

            AppContext.SaveChanges();

             GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_FormTab1).FullName, fieldName, fieldValue,
                     editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateFrom6(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId, long editor)
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
					fieldValue = fieldValue.Replace(",", ".");
					floatValue = float.Parse(fieldValue, CultureInfo.InvariantCulture.NumberFormat);
					//floatValue = float.Parse(fieldValue);
				}
            }
            SUB_Form6Record entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_Form6Record.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_Form6Record { SUB_Form = form };
            }
            var type = CodeConstManager.LONG_CODE;
            switch (fieldName)
            {
                case "CountDevice":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.CountDevice = null;
                        }
                        else
                        {
                            entity.CountDevice = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
                case "Equipment":
                    {
                        entity.Equipment = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "TypeCounterId":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.TypeCounterId = null;
                        }
                        else
                        {
                            entity.TypeCounterId = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
                case "TypeResourceId":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.TypeResourceId = null;
                        }
                        else
                        {
                            entity.TypeResourceId = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
            }

            //----for log
            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (entity.Id == 0)
            {
                AppContext.SUB_Form6Record.Add(entity);
            }

            AppContext.SaveChanges();
             GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_Form6Record).FullName, fieldName, fieldValue,
                         editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateFrom6TypeResource(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId,long typeId, long editor)
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
                    fieldValue = fieldValue.Replace(",", ".");
                    floatValue = float.Parse(fieldValue, CultureInfo.InvariantCulture.NumberFormat);
                    //floatValue = float.Parse(fieldValue);
                }
            }

            SUB_Form6Record entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_Form6Record.FirstOrDefault(e => e.FormId == form.Id && e.TypeResourceId == typeId);
            }
            if (entity == null)
            {
                entity = new SUB_Form6Record { TypeResourceId = typeId, SUB_Form = form };
            }

            var type = CodeConstManager.LONG_CODE;
            switch (fieldName)
            {
                case "CountDevice":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.CountDevice = null;
                        }
                        else
                        {
                            entity.CountDevice = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
                case "Equipment":
                    {
                        entity.Equipment = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "TypeCounterId":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.TypeCounterId = null;
                        }
                        else
                        {
                            entity.TypeCounterId = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
                case "TypeResourceId":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.TypeResourceId = null;
                        }
                        else
                        {
                            entity.TypeResourceId = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
            }

            //----for log
            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (entity.Id == 0)
            {
                AppContext.SUB_Form6Record.Add(entity);
            }

            AppContext.SaveChanges();
            GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_Form6Record).FullName, fieldName, fieldValue,
                        editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateFrom6KindResource(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId,long typeId, long editor)
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
                    fieldValue = fieldValue.Replace(",", ".");
                    floatValue = float.Parse(fieldValue, CultureInfo.InvariantCulture.NumberFormat);
                    //floatValue = float.Parse(fieldValue);
                }
            }

            SUB_Form6Record entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_Form6Record.FirstOrDefault(e => e.FormId == form.Id && e.KindResourceId == typeId);
            }
            if (entity == null)
            {
                entity = new SUB_Form6Record { KindResourceId = typeId, SUB_Form = form };
            }

            var type = CodeConstManager.LONG_CODE;
            switch (fieldName)
            {
                case "CountDevice":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.CountDevice = null;
                        }
                        else
                        {
                            entity.CountDevice = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
                case "Equipment":
                    {
                        entity.Equipment = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "TypeCounterId":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.TypeCounterId = null;
                        }
                        else
                        {
                            entity.TypeCounterId = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
                case "TypeResourceId":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.TypeResourceId = null;
                        }
                        else
                        {
                            entity.TypeResourceId = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
            }

            //----for log
            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (entity.Id == 0)
            {
                AppContext.SUB_Form6Record.Add(entity);
            }

            AppContext.SaveChanges();
            GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_Form6Record).FullName, fieldName, fieldValue,
                        editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateFrom5(SUB_Form form, long recordId, string propertyName, string fieldValue, string tabName, long? userId, long editor)
        {
            string fieldName;
            if (propertyName.Contains("_"))
            {
                if (propertyName.IndexOf("energyindicator_id") != -1)
                    fieldName = "energyindicator_id";
                else
                    fieldName = propertyName.Split('_')[1];
            }
            else
            {
                fieldName = propertyName;

            }

            var kindIndex = 1;
            if (propertyName.Contains("SubForm5RecordsOther"))
            {
                kindIndex = 2;
            }

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
					fieldValue = fieldValue.Replace(",", ".");
					floatValue = float.Parse(fieldValue, CultureInfo.InvariantCulture.NumberFormat);
					//floatValue = float.Parse(fieldValue);
				}
				else
				{
					fieldValue = fieldValue.Replace(",", ".");
				}
            }

            SUB_Form5Record entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_Form5Record.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_Form5Record { SUB_Form = form, KindIndex = kindIndex };
            }
            var type = CodeConstManager.STRING_CODE;
            switch (fieldName)
            {

                case "NormEnergyId":
                    {
                        entity.NormEnergyId = Convert.ToInt64(fieldValue);
                        if (entity.NormEnergyId == 0)
                        {
                            entity.NormEnergyId = null;
                        }
                        else
                        {
                           var unit =AppContext.SUB_DIC_NormEnergy.FirstOrDefault(e => e.Id == entity.NormEnergyId);
                            if (unit != null)
                            {
                                entity.UnitMeasure = unit.UnitName;
                            }
                        }
                        type = CodeConstManager.LONG_CODE;
                        break;
                    }
                case "energyindicator_id":
                    {
                        entity.energyindicator_id = Convert.ToInt64(fieldValue);
                        if (entity.energyindicator_id == 0)
                        {
                            entity.energyindicator_id = null;
                        }
                        else
                        {
                            var unit = AppContext.sub_dic_energyindicator.FirstOrDefault(e => e.id == entity.energyindicator_id);
                            if (unit != null)
                            {
                                entity.UnitMeasure = unit.unitnameru;
                            }
                        }
                        type = CodeConstManager.LONG_CODE;
                        break;
                    }
                case "IndicatorName":
                    {
                        entity.IndicatorName = fieldValue;
                        break;
                    }
                case "RegularStandart":
                    {
                        entity.RegularStandart = fieldValue;
                        break;
                    }
                case "UnitMeasure":
                    {
                        entity.UnitMeasure = fieldValue;
                        break;
                    }
                case "CalcFormula":
                    {
                        entity.CalcFormula = fieldValue;
                        break;
                    }
                case "EnergyValue":
                    {
                        entity.EnergyValue = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "TypeOfHeating":
                    {
                        if (string.IsNullOrWhiteSpace(fieldValue) || fieldValue.Equals("0"))
                            entity.TypeOfHeating = null;
                        else
                            entity.TypeOfHeating = Convert.ToInt16(fieldValue);

                        break;
                    }
            }

            //----for log
            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (entity.Id == 0)
            {
                AppContext.SUB_Form5Record.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string _error = ex.Message;
            }

             GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_Form5Record).FullName, fieldName, fieldValue,
             editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateFrom4(SUB_Form form, long recordId, string propertyName, string fieldValue, string tabName,  long? userId, long editor)
        {
            string fieldName;
            
            //----for log
            form.authorid = MyExtensions.GetCurrentUserId();
            form.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (propertyName.Contains("_"))
            {
                fieldName = propertyName.Split('_')[1];
            }
            else
            {
                fieldName = propertyName;

            }
            var kindrow = 1;
            if (propertyName.Contains("SubForm4RecordsOther"))
            {
                kindrow = 2;
            }

            if (fieldName == "IsPlan")
            {
                if (fieldValue == "true")
                {
                    form.IsPlan = true;
                }
                if (fieldValue == "false")
                {
                    form.IsPlan = false;
                }
                if (form.Id == 0)
                {
                    AppContext.SUB_Form.Add(form);
                }
                AppContext.SaveChanges();
                return new SubUpdateField { ModelId = form.Id, RecordId = 0, Unique = null };
            }

            if (fieldName == "IsEnergyManagementSystem")
            {
                if (fieldValue == "true")
                {
                    form.IsEnergyManagementSystem = true;
                }
                if (fieldValue == "false")
                {
                    form.IsEnergyManagementSystem = false;
                }
                if (form.Id == 0)
                {
                    AppContext.SUB_Form.Add(form);
                }
                AppContext.SaveChanges();
                return new SubUpdateField { ModelId = form.Id, RecordId = 0, Unique = null };
            }

			if (fieldName == "IsNotEvents")
			{
				if (fieldValue == "true")
				{
					form.IsNotEvents = true;
				}
				if (fieldValue == "false")
				{
					form.IsNotEvents = false;
				}
				if (form.Id == 0)
				{
					AppContext.SUB_Form.Add(form);
				}
				AppContext.SaveChanges();
				return new SubUpdateField { ModelId = form.Id, RecordId = 0, Unique = null };
			}

            if (fieldName == "IsConfirmPlan")
            {
                if (fieldValue == "true")
                {
                    form.IsConfirmPlan = true;
                }
                if (fieldValue == "false")
                {
                    form.IsConfirmPlan = false;
                }
                if (form.Id==0)
                {
                    AppContext.SUB_Form.Add(form);
                }
                AppContext.SaveChanges();
                return new SubUpdateField { ModelId = form.Id, RecordId = 0, Unique = null };
            }
            if (fieldName == "BeginPlanYear")
            {
                var years = Convert.ToInt32(fieldValue);
                form.BeginPlanYear = years;
                form.EndPlanYear = years + 4;
                if (form.Id == 0)
                {
                    AppContext.SUB_Form.Add(form);
                }
                AppContext.SaveChanges();
                return new SubUpdateField { ModelId = form.Id, RecordId = 0, Unique = null };
            }

            double? floatValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
                    fieldValue = fieldValue.Replace(".", ",");
                }

				
				if (double.TryParse(fieldValue, out db))
				{
					floatValue = Convert.ToDouble(fieldValue);
					//floatValue = float.Parse(fieldValue);
				}
            }
            SUB_Form4Record entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_Form4Record.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_Form4Record { SUB_Form = form, KindIndex = kindrow};
            }
            var type = "";
            switch (fieldName)
            {
                case "Note":
                    {
                        entity.Note = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "EventName":
                    {
                        entity.EventName = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "PlanExpend":
                    {
                        entity.PlanExpend = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "EmplPeriodStr":
                    {
                        entity.EmplPeriod = DateHelper.GetDate("01/" + fieldValue);
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
                case "InKind":
                    {
                        entity.InKind = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "InMoney":
                    {
                        entity.InMoney = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "ActualInvest":
                    {
                        entity.ActualInvest = floatValue;
                        type = CodeConstManager.FLOAT_CODE;
                        break;
                    }
                case "TypeResourceId":
                    {
                        entity.TypeResourceId = Convert.ToInt64(fieldValue);
                        type = CodeConstManager.LONG_CODE;
                        break;
                    }
                case "EventId":
                    {
                        entity.EventId = Convert.ToInt64(fieldValue);
                        type = CodeConstManager.LONG_CODE;
                        break;
                    }
                case "TypeCounterId":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            entity.TypeCounterId = null;
                        }
                        else
                        {
                            entity.TypeCounterId = Convert.ToInt32(fieldValue);
                        }
                        break;
                    }
            }

            //----for log
            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (entity.Id == 0)
            {
                AppContext.SUB_Form4Record.Add(entity);
            }

            AppContext.SaveChanges();
            GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_Form4Record).FullName, fieldName, fieldValue,
               editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        private SubUpdateField UpdateFrom3(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId, long typeId,long editor)
        {
            double? floatValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
                    fieldValue = fieldValue.Replace(".", ",");
                }
				
				if (double.TryParse(fieldValue, out db))
				{
					floatValue = Convert.ToDouble(fieldValue);
					//floatValue = float.Parse(fieldValue);
				}
            }
            SUB_Form3Record entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_Form3Record.FirstOrDefault(e => e.FormId == form.Id && e.KindResourceId == typeId);
            }
            if (entity == null)
            {
                entity = new SUB_Form3Record { KindResourceId = typeId, SUB_Form = form };
            }
            switch (fieldName)
            {
                case "LosTransportVolume":
                    {
                        entity.LosTransportVolume = floatValue;
                        break;
                    }
                case "LosTransportPrice":
                    {
                        entity.LosTransportPrice = floatValue;
                        break;
                    }
                case "ConsumptionPrice":
                    {
                        entity.ConsumptionPrice = floatValue;
                        break;
                    }
                case "ConsumptionVolume":
                    {
                        entity.ConsumptionVolume = floatValue;
                        break;
                    }

            }

            //----for log
            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();

            if (entity.Id == 0)
            {
                AppContext.SUB_Form3Record.Add(entity);
            }

            AppContext.SaveChanges();
            GetQuid(form.Id, entity.Id, tabName, CodeConstManager.FLOAT_CODE, typeof(SUB_Form3Record).FullName, fieldName, fieldValue,
              editor);
            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id};
        }

        private SubUpdateField  UpdateFrom2(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName,  long? userId, long typeId,long editor)
        {
            double? floatValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
                    fieldValue = fieldValue.Replace(".", ",");
                }

				

				if (double.TryParse(fieldValue, out db))
				{
					floatValue = Convert.ToDouble(fieldValue);
					//floatValue = float.Parse(fieldValue);
				}
            }
            SUB_Form2Record entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_Form2Record.FirstOrDefault(e => e.FormId == form.Id && e.TypeResourceId == typeId);
            }
            if (entity == null)
            {
                entity = new SUB_Form2Record { TypeResourceId = typeId, SUB_Form = form };
            }
            var type = CodeConstManager.FLOAT_CODE;
            switch (fieldName)
            {
                case "ExpenceEnergy":
                    {
                        entity.ExpenceEnergy = floatValue;
                        break;
                    }
                case "ExtractVolume":
                    {
                        entity.ExtractVolume = floatValue;
                        break;
                    }
                case "LosEnergy":
                    {
                        entity.LosEnergy = floatValue;
                        break;
                    }
                case "NotOwnSource":
                    {
                        entity.NotOwnSource = floatValue;
                        break;
                    }
                case "OwnSource":
                    {
                        entity.OwnSource = floatValue;
                        break;
                    }
                case "TransferOtherLegal":
                    {
                        entity.TransferOtherLegal = floatValue;
                        break;
                    }
                case "Note":
                    {
                        entity.Note = fieldValue;
                        type = CodeConstManager.STRING_CODE;
                        break;
                    }
            }

            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();
            if (entity.Id == 0)
            {            
                AppContext.SUB_Form2Record.Add(entity);
            }

			try
			{
				AppContext.SaveChanges();
				GetQuid(form.Id, entity.Id, tabName, type, typeof(SUB_Form2Record).FullName, fieldName, fieldValue,
					  editor);
			}
			catch (Exception ex)
			{
				string err = ex.Message;
			}

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }

        #region gu
        private SubUpdateField UpdateFrom2Gu(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId, long editor)
        {
            int? intValue = null;
            if (!string.IsNullOrEmpty(fieldValue))
            {
                intValue = Convert.ToInt16(fieldValue);
            }

            SUB_Form2Gu entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_Form2Gu.FirstOrDefault(e => e.FormId == form.Id);
            }
            if (entity == null)
            {
                entity = new SUB_Form2Gu { FormId = form.Id };
            }
            var type = CodeConstManager.FLOAT_CODE;
            switch (fieldName)
            {
                case "CountOfEmployees":
                    {
                        entity.CountOfEmployees = intValue;
                        break;
                    }
                case "CountOfStudents":
                    {
                        entity.CountOfStudents = intValue;
                        break;
                    }
                case "CountOfBeds":
                    {
                        entity.CountOfBeds = intValue;
                        break;
                    }
            }

            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();
            if (entity.Id == 0)
            {
                AppContext.SUB_Form2Gu.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        private SubUpdateField UpdateFrom3Gu(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId, long editor)
        {
            int? intValue = null;
            double? doubleValue = null;

            if (!string.IsNullOrEmpty(fieldValue))
            {
                Int16 x;
                if (Int16.TryParse(fieldValue, out x))
                {
                    intValue = Convert.ToInt16(fieldValue);
                }

                //---- if value double 
                double db;
                if (!double.TryParse(fieldValue, out db))
                {
                    fieldValue = fieldValue.Replace(".", ",");
                }

                if (double.TryParse(fieldValue, out db))
                {
                    doubleValue = Convert.ToDouble(fieldValue);
                }
            }

            SUB_Form3Gu entity = null;
            if (form.Id > 0)
            {
                entity = AppContext.SUB_Form3Gu.FirstOrDefault(e => e.FormId == form.Id);
            }
            if (entity == null)
            {
                entity = new SUB_Form3Gu { FormId = form.Id };
            }
            var type = CodeConstManager.FLOAT_CODE;
            switch (fieldName)
            {
                case "YearOfConstruction":
                    {
                        entity.YearOfConstruction = fieldValue;
                        break;
                    }
                case "AutomateItem":
                    {
                        entity.AutomateItem =Convert.ToInt16(intValue);
                        break;
                    }
                case "TotalAreaOfBuilding":
                    {
                        entity.TotalAreaOfBuilding = doubleValue;
                        break;
                    }
                case "HeatedAreaOfBuilding":
                    {
                        entity.HeatedAreaOfBuilding = doubleValue;
                        break;
                    }
                case "IndependentHeating":
                    {
                        entity.IndependentHeating = intValue;
                        break;
                    }
                case "CentralHeating":
                    {
                        entity.CentralHeating = intValue;
                        break;
                    }
            }

            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();
            if (entity.Id == 0)
            {
                AppContext.SUB_Form3Gu.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        private SubUpdateField UpdateFromGuLightInfo(SUB_Form form, long recordId, string fieldName, string fieldValue, string tabName, long? userId, long editor)
        {
            int? intValue = null;
            if (fieldName == "Amount")
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    intValue = Convert.ToInt16(fieldValue);
                }
            }

            SUB_FormGuLightingInfo entity = null;
            if (recordId > 0)
            {                
                entity = AppContext.SUB_FormGuLightingInfo.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_FormGuLightingInfo { FormId = form.Id };
            }

            switch (fieldName)
            {
                case "LampType":
                    {
                        entity.LampType = fieldValue;
                        break;
                    }
                case "Amount":
                    {
                        entity.Amount = intValue;
                        break;
                    }
            }

            if (entity.Id == 0)
            {
                AppContext.SUB_FormGuLightingInfo.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        #endregion

        #region gu new 
        private SubUpdateField UpdateFrom3GuRecord(SUB_Form form, long recordId, string propertyName, string fieldValue, string tabName, long? userId, long editor)
        {
            string fieldName;
            if (propertyName.Contains("_"))
            {
                fieldName = propertyName.Split('_')[2];
            }
            else
            {
                fieldName = propertyName;

            }

            int? intValue = null;            
            double? doubleValue = null;

            if (fieldName.Equals("TotalAreaOfBuilding") || fieldName.Equals("HeatedAreaOfBuilding"))
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    double db;
                    if (!double.TryParse(fieldValue, out db))
                    {
                        fieldValue = fieldValue.Replace(".", ",");
                    }

                    if (double.TryParse(fieldValue, out db))
                    {
                        doubleValue = Convert.ToDouble(fieldValue);
                    }
                }
            }
            else
            {

                if (!string.IsNullOrEmpty(fieldValue))
                {
                    intValue = Convert.ToInt32(fieldValue);
                }
            }

            SUB_Form3GuRecord entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_Form3GuRecord.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_Form3GuRecord { FormId = form.Id, isdeleted = false, AutomateItem = 2 };
            }
            
            var type = CodeConstManager.FLOAT_CODE;

            switch (fieldName)
            {
                case "CountOfBuildings":
                    {
                        entity.CountOfBuildings = intValue;
                        break;
                    }
                case "YearOfConstruction":
                    {
                        entity.YearOfConstruction = intValue;
                        break;
                    }
                case "AutomateItem":
                    {
                        if (intValue == 0 || intValue == null)
                            entity.AutomateItem = null;
                        else entity.AutomateItem = (Int16)intValue;
                        break;
                    }
                case "TotalAreaOfBuilding":
                    {
                        entity.TotalAreaOfBuilding = doubleValue;
                        break;
                    }
                case "HeatedAreaOfBuilding":
                    {
                        entity.HeatedAreaOfBuilding = doubleValue;
                        break;
                    }
                case "CountOfEmployees":
                    {
                        entity.CountOfEmployees = intValue;
                        break;
                    }
                case "CountOfStudents":
                    {
                        entity.CountOfStudents = intValue;
                        break;
                    }
                case "CountOfBeds":
                    {
                        entity.CountOfBeds = intValue;
                        break;
                    }
            }

            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();
            if (entity.Id == 0)
            {
                AppContext.SUB_Form3GuRecord.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        private SubUpdateField UpdateFrom3aGuRecord1(SUB_Form form, long recordId, string propertyName, string fieldValue, string tabName, long? userId, long editor)
        {
            string fieldName;
            if (propertyName.Contains("_"))
            {
                fieldName = propertyName.Split('_')[2];
            }
            else
            {
                fieldName = propertyName;
            }

            var kindIndex = 1;
            if (propertyName.Contains("SUB_Form3aGuRecord1sOther"))
            {
                kindIndex = 2;
            }

            int? intValue = null;
            double? doubleValue = null;

            if (fieldName.Equals("PowerOfHeatingSources"))
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    double db;
                    if (!double.TryParse(fieldValue, out db))
                    {
                        fieldValue = fieldValue.Replace(".", ",");
                    }

                    if (double.TryParse(fieldValue, out db))
                    {
                        doubleValue = Convert.ToDouble(fieldValue);
                    }
                }
            }
            else
            {
                if (!fieldName.Equals("SourceName"))
                {
                    if (!string.IsNullOrEmpty(fieldValue))
                    {
                        intValue = Convert.ToInt32(fieldValue);
                    }
                }
            }

            SUB_Form3aGuRecord1 entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_Form3aGuRecord1.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_Form3aGuRecord1 { FormId = form.Id, isdeleted = false, KindIndex = kindIndex };
            }
            
            switch (fieldName)
            {
                case "DicId":
                    {
                        if (intValue == null || intValue == 0)
                            entity.DicId = null;
                        else entity.DicId = intValue;
                        break;
                    }
                case "CountOfHeatingSources":
                    {
                        entity.CountOfHeatingSources = intValue;
                        break;
                    }
                case "CoefficientOfPerformance":
                    {
                        entity.CoefficientOfPerformance = intValue;
                        break;
                    }
                case "PowerOfHeatingSources":
                    {
                        entity.PowerOfHeatingSources = doubleValue;
                        break;
                    }
                case "YearOfCommissioning":
                    {
                        entity.YearOfCommissioning = intValue;
                        break;
                    }
                case "SourceName":
                    {
                        entity.SourceName = fieldValue;
                        break;
                    }
            }

            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();
            if (entity.Id == 0)
            {
                AppContext.SUB_Form3aGuRecord1.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        private SubUpdateField UpdateFrom3aGuRecord2(SUB_Form form, long recordId, string propertyName, string fieldValue, string tabName, long? userId, long editor)
        {
            string fieldName;
            if (propertyName.Contains("_"))
            {
                fieldName = propertyName.Split('_')[2];
            }
            else
            {
                fieldName = propertyName;
            }
            
            var kindIndex = 1;
            if (propertyName.Contains("SUB_Form3aGuRecord2sOther"))
            {
                kindIndex = 2;
            }

            int? intValue = null;
            double? doubleValue = null;

            if (fieldName.Equals("Power") || fieldName.Equals("HoursPerDay"))
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    double db;
                    if (!double.TryParse(fieldValue, out db))
                    {
                        fieldValue = fieldValue.Replace(".", ",");
                    }

                    if (double.TryParse(fieldValue, out db))
                    {
                        doubleValue = Convert.ToDouble(fieldValue);
                    }
                }
            }
            else if(fieldName.Equals("Amount") || fieldName.Equals("DicId"))
            {

                if (!string.IsNullOrEmpty(fieldValue))
                {
                    intValue = Convert.ToInt32(fieldValue);
                }
            }

            SUB_Form3aGuRecord2 entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_Form3aGuRecord2.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_Form3aGuRecord2 { FormId = form.Id, isdeleted = false, KindIndex = kindIndex };
            }

            switch (fieldName)
            {
                case "DicId":
                    {
                        if (intValue == null || intValue == 0)
                            entity.DicId = null;
                        else
                            entity.DicId = intValue;
                        break;
                    }
                case "Amount":
                    {
                        entity.Amount = intValue;
                        break;
                    }
                case "Power":
                    {
                        entity.Power = doubleValue;
                        break;
                    }
                case "HoursPerDay":
                    {
                        entity.HoursPerDay = doubleValue;
                        break;
                    }
                case "DeviceName":
                    {
                        entity.DeviceName = fieldValue;
                        break;
                    }
            }

            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();
            if (entity.Id == 0)
            {
                AppContext.SUB_Form3aGuRecord2.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        private SubUpdateField UpdateFrom3aGuRecord3(SUB_Form form, long recordId, string propertyName, string fieldValue, string tabName, long? userId, long editor)
        {
            string fieldName;
            if (propertyName.Contains("_"))
            {
                fieldName = propertyName.Split('_')[2];
            }
            else
            {
                fieldName = propertyName;
            }

            var kindIndex = 1;
            if (propertyName.Contains("SUB_Form3aGuRecord3sOther"))
            {
                kindIndex = 2;
            }

            int? intValue = null;
            double? doubleValue = null;

            if (fieldName.Equals("Power") || fieldName.Equals("HoursPerDay"))
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    double db;
                    if (!double.TryParse(fieldValue, out db))
                    {
                        fieldValue = fieldValue.Replace(".", ",");
                    }

                    if (double.TryParse(fieldValue, out db))
                    {
                        doubleValue = Convert.ToDouble(fieldValue);
                    }
                }
            }
            else if (fieldName.Equals("Amount") || fieldName.Equals("DicId"))
            {

                if (!string.IsNullOrEmpty(fieldValue))
                {
                    intValue = Convert.ToInt32(fieldValue);
                }
            }

            SUB_Form3aGuRecord3 entity = null;
            if (recordId > 0)
            {
                entity = AppContext.SUB_Form3aGuRecord3.FirstOrDefault(e => e.Id == recordId);
            }
            if (entity == null)
            {
                entity = new SUB_Form3aGuRecord3 { FormId = form.Id, isdeleted = false, KindIndex = kindIndex };
            }

            switch (fieldName)
            {
                case "DicId":
                    {
                        if (intValue == null || intValue == 0)
                            entity.DicId = null;
                        else
                            entity.DicId = intValue;
                        break;
                    }
                case "Amount":
                    {
                        entity.Amount = intValue;
                        break;
                    }
                case "Power":
                    {
                        entity.Power = doubleValue;
                        break;
                    }
                case "HoursPerDay":
                    {
                        entity.HoursPerDay = doubleValue;
                        break;
                    }
                case "EnergyConsumEquipName":
                    {
                        entity.EnergyConsumEquipName = fieldValue;
                        break;
                    }
            }

            entity.authorid = MyExtensions.GetCurrentUserId();
            entity.authorlogin = MyExtensions.GetCurrentUserLogin();
            if (entity.Id == 0)
            {
                AppContext.SUB_Form3aGuRecord3.Add(entity);
            }

            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            return new SubUpdateField { ModelId = form.Id, RecordId = entity.Id };
        }
        #endregion

        private void GetQuid(long formId, long recordId, string tabName, string type, string className, string fieldName, string fieldValue,  long? userId)
        {
			string ErrorMessage="";
			try
			{
				var model = new SUB_FormRecordHistory
				{
					ClassName = className,
					FieldName = fieldName,
					FieldValue = fieldValue,
					FormId = formId,
					RecordId = recordId,
					TabName = tabName,
					TypeValue = type,
					UserId = userId,
					RegDate = DateTime.Now

				};

				var entity =
					AppContext.SUB_FormRecordHistory.FirstOrDefault(
						e => e.ClassName == className && e.RecordId == recordId);
				if (entity == null)
				{
					model.Unique = Guid.NewGuid().ToString();
				}
				else
				{
					model.Unique = entity.Unique;
				}


				AppContext.SUB_FormRecordHistory.Add(model);
				AppContext.SaveChanges();
			}
			catch (Exception ex) {
				ErrorMessage = ex.Message;
			}
        }
        
        public void DeleteRecord(string code, long recordId)
        {
            switch (code)
            {
                case "form4":
                    {
                        var form4 = AppContext.SUB_Form4Record.FirstOrDefault(e => e.Id == recordId);
                        if (form4 != null)
                        {
                            //----for log
                            form4.authorid = MyExtensions.GetCurrentUserId();
                            form4.authorlogin = MyExtensions.GetCurrentUserLogin();

                            AppContext.SUB_Form4Record.Remove(form4);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                case "form5":
                    {
                        var form5 = AppContext.SUB_Form5Record.FirstOrDefault(e => e.Id == recordId);
                        if (form5 != null)
                        {
                            //----for log
                            form5.authorid = MyExtensions.GetCurrentUserId();
                            form5.authorlogin = MyExtensions.GetCurrentUserLogin();
                            AppContext.SUB_Form5Record.Remove(form5);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
				case "form6":
					{
						var form6 = AppContext.SUB_Form6Record.FirstOrDefault(e => e.Id == recordId);
						if (form6 != null)
						{
                            //----for log
                            form6.authorid = MyExtensions.GetCurrentUserId();
                            form6.authorlogin = MyExtensions.GetCurrentUserLogin();
                            AppContext.SUB_Form6Record.Remove(form6);
							AppContext.SaveChanges();
						}
						break;
					}
                case "tab3":
                    {
                        var form5 = AppContext.SUB_FormTab3.FirstOrDefault(e => e.Id == recordId);
                        if (form5 != null)
                        {
                       
                            AppContext.SUB_FormTab3.Remove(form5);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                case "tabKadastr":
                    {
                        var tabKadastr = AppContext.SUB_FormKadastr.FirstOrDefault(e => e.Id == recordId);
                        if (tabKadastr != null)
                        {
                            //----for log
                            tabKadastr.authorid = MyExtensions.GetCurrentUserId();
                            tabKadastr.authorlogin = MyExtensions.GetCurrentUserLogin();

                            AppContext.SUB_FormKadastr.Remove(tabKadastr);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                case "formGuLightingInfo":
                    {
                        var formGuLightingInfo= AppContext.SUB_FormGuLightingInfo.FirstOrDefault(e => e.Id == recordId);
                        if (formGuLightingInfo != null)
                        {
                            AppContext.SUB_FormGuLightingInfo.Remove(formGuLightingInfo);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                case "form3GuRecord":
                    {
                        var form3GuRecord = AppContext.SUB_Form3GuRecord.FirstOrDefault(e => e.Id == recordId);
                        if (form3GuRecord != null)
                        {
                            AppContext.SUB_Form3GuRecord.Remove(form3GuRecord);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                case "form3aGuRecord1":
                    {
                        var form3aGuRecord1 = AppContext.SUB_Form3aGuRecord1.FirstOrDefault(e => e.Id == recordId);
                        if (form3aGuRecord1 != null)
                        {
                            AppContext.SUB_Form3aGuRecord1.Remove(form3aGuRecord1);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                case "form3aGuRecord2":
                    {
                        var form3aGuRecord2 = AppContext.SUB_Form3aGuRecord2.FirstOrDefault(e => e.Id == recordId);
                        if (form3aGuRecord2 != null)
                        {
                            AppContext.SUB_Form3aGuRecord2.Remove(form3aGuRecord2);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                case "form3aGuRecord3":
                    {
                        var form3aGuRecord3 = AppContext.SUB_Form3aGuRecord3.FirstOrDefault(e => e.Id == recordId);
                        if (form3aGuRecord3 != null)
                        {
                            AppContext.SUB_Form3aGuRecord3.Remove(form3aGuRecord3);
                            AppContext.SaveChanges();
                        }
                        break;
                    }
                default:
                    {
                        if (code.Contains("tab1_"))
                        {
                            var codes = code.Split('_');
                            long modelId;
                            if (!long.TryParse(codes[1], out modelId))
                            {
                                return;
                            }
                            var codeIndex = codes[2] + ".";
                            if (recordId < 10)
                            {
                                codeIndex = codeIndex + "0";
                            }
                            codeIndex = codeIndex + recordId;
                            var tab1 = AppContext.SUB_FormTab1.FirstOrDefault(e => e.FormId == modelId && e.Code == codeIndex);
                            if (tab1 != null)
                            {
                                AppContext.SUB_FormTab1.Remove(tab1);
                                AppContext.SaveChanges();
                            }
                        }
                        break;
                    }
            }
        }

        public int GetCountReject(long? getCurrentUserId)
        {
            try
            {
                if (AppContext.Database.Connection.State == System.Data.ConnectionState.Closed) { AppContext.Database.Connection.Open(); }
                return AppContext.SUB_Form.Count(
                     e => e.UserId == getCurrentUserId && e.StatusId == CodeConstManager.STATUS_REJECT_ID && !e.IsDeleted);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int GetCountInbox()
        {
            return AppContext.SUB_Form.Count(
               e => e.StatusId == CodeConstManager.STATUS_SEND_ID);
        }
        public int GetCountInboxByOblast(List<string> oblasts)
        {
            return AppContext.SUB_Form.Count(
               e => e.StatusId == CodeConstManager.STATUS_SEND_ID && !e.IsDeleted && oblasts.Contains(e.SEC_User1.DIC_Kato.Code));
        }
        public int GetCountInboxByCurrentEmployee(long? idUser)
        {
            if (idUser == null)
            {
                return AppContext.SUB_Form.Count(
               e => e.StatusId == CodeConstManager.STATUS_SEND_ID && !e.IsDeleted);
            }
            var user = new SecUserRepository().GetById(idUser.Value);
            var list = GetQueryByDescending(e => !e.IsDeleted && e.StatusId != CodeConstManager.REG_STATUS_REESTR_ID, true, e => e.Id);
            if (user.DIC_Organization != null && user.DIC_Organization.DIC_Kato != null)
            {
                return AppContext.SUB_Form.Count(e => e.SEC_User1.Oblast == user.DIC_Organization.DIC_Kato.Id);
            }
            return AppContext.SUB_Form.Count(
              e => e.StatusId == CodeConstManager.STATUS_SEND_ID && !e.IsDeleted);
        }
        public List<TermSearch> GetTerm(string term)
        {
            var serchTerm = string.IsNullOrEmpty(term) ? term : term.ToLower();

            var query = AppContext.SEC_User.Where(e => (e.BINIIN.ToLower().Contains(serchTerm) || e.JuridicalName.ToLower().Contains(serchTerm)) && e.IsGuest)
                .Select(x => new TermSearch() { Id = x.Id, Term = x.BINIIN + " - " + x.JuridicalName }).Distinct();
            return query.ToList();
        }

		public void UpdateUserInfo(int ReportYear, long ownerId, string fieldName, string fieldValue)
        {
            //var user = AppContext.SEC_User.FirstOrDefault(e => e.Id == ownerId);
			var user = AppContext.RST_ReportReestr.FirstOrDefault(x => x.RST_Report.ReportYear == ReportYear && x.UserId == ownerId);
            if (user == null)
            {
                return;
            }            

            switch (fieldName)
            {
                case "ApplicationName":
                    {
                        if (user.usrtypeapplicationid != CodeConstManager.APP_TYPE_PHYS_PERSON)
                        {
                            user.usrjuridicalname = fieldValue;
                        }
                        break;
                    }
                case "Address":
                    {
                        user.usraddress = fieldValue;
                        break;
                    }
                case "FullName":
                    {
                        var fio = fieldValue.Split(' ');
                        user.usrlastname = fio[0];
                        if (fio.Length > 1)
                        {
                            user.usrfirstname = fio[1];
                        }
                        if (fio.Length > 2)
                        {
                            user.usrsecondname = fio[2];
                        }
                        break;
                    }
                case "Post":
                    {
                        user.usrpost = fieldValue;
                        break;
                    }
                case "IsCvazy":
                    {
                        user.usriscvazy = fieldValue=="True";
                        user.usrfscode = 2;
                        break;
                    }
                case "ResponceFIO":
                    {
                        user.usrresponcefio = fieldValue;
                        break;
                    }
                case "ResponcePost":
                    {
                        user.usrresponcepost = fieldValue;
                        break;
                    }
                case "OkedId":
                    {
                        user.usrokedid = Convert.ToInt64(fieldValue);
                        break;
                    }
                    
                case "Wastes":
                    {					
                        var okeds = fieldValue.Split(',');
                        var aquticOblasts = AppContext.Set<rst_reportreestroked>().Where(e => e.reportreestrid == user.Id);
                        var oblastId = new List<long>();
                        foreach (var country in okeds)
                        {
                            var idolbast = Convert.ToInt64(country);
                            if (aquticOblasts.SingleOrDefault(e => e.okedid == idolbast) == null)
                            {
								var way = new rst_reportreestroked
                                {
                                    reportreestrid = user.Id,
                                    okedid = idolbast                                   
                                };
                                AppContext.Set<rst_reportreestroked>().Add(way);
                            }
                            else
                            {
                                oblastId.Add(idolbast);
                            }
                        }
                        var listdelete = aquticOblasts.Where(e => !oblastId.Contains(e.okedid));
                        foreach (var crRoutesAquticOblast in listdelete)
                        {
                            AppContext.Set<rst_reportreestroked>().Remove(crRoutesAquticOblast);
                        }
                        break;
                    }
            }
            AppContext.SaveChanges();
        }

        public List<SUB_Form> GetCurrentEmployee(long? idUser)
        {
            if (idUser == null)
            {
                return GetCollectionList();
            }
            var user = new SecUserRepository().GetById(idUser.Value);
            var list = GetQueryByDescending(e => !e.IsDeleted && e.StatusId != CodeConstManager.REG_STATUS_REESTR_ID, true, e => e.Id);
            if (user.DIC_Organization != null && user.DIC_Organization.DIC_Kato != null)
            {
                return list.Where(e => e.SEC_User1.Oblast == user.DIC_Organization.DIC_Kato.Id).ToList();
            }
            return list.ToList();
        }

        public List<SUB_Form> GetListByOblast(List<string> oblasts, int? reportYear)
        {
            if (reportYear > 2000 && reportYear < 2030)
            {
                return GetQueryByDescending(e => !e.IsDeleted && e.ReportYear == reportYear && e.StatusId != CodeConstManager.REG_STATUS_REESTR_ID && oblasts.Contains(e.SEC_User1.DIC_Kato.Code), true, e => e.Id).ToList();
            }
            return GetQueryByDescending(e => !e.IsDeleted && e.StatusId != CodeConstManager.REG_STATUS_REESTR_ID && oblasts.Contains(e.SEC_User1.DIC_Kato.Code), true, e => e.Id).ToList();

        }
		
        public SUB_FormComment GetComments(long modelId, string nameTable, int colIndex, int rowIndex)
        {
            var model =
                AppContext.SUB_FormComment.FirstOrDefault(
                    e => e.ColumnIndex == colIndex && e.FormId == modelId && e.RowIndex == rowIndex && e.TableName == nameTable);
            if (model == null)
            {
                return new SUB_FormComment { IsError = true };
            }
            return model;
        }

        public void SaveComment(long modelId, string nameTable, int colIndex, int rowIndex, bool isError, string comment, long rowId, string fieldName, string fieldValue, long? getCurrentUserId)
        {
            var model =
               AppContext.SUB_FormComment.FirstOrDefault(
                   e => e.ColumnIndex == colIndex && e.FormId == modelId && e.RowIndex == rowIndex && e.TableName == nameTable) ??
               new SUB_FormComment
                   {
                       CreateDate = DateTime.Now,
                       FormId = modelId,
                       RowIndex = rowIndex,
                       ColumnIndex = colIndex,
                       TableName = nameTable,
                   };
            if (fieldName == "TypeResourceId")
            {
                long typeresourceId;
                if(long.TryParse(fieldValue, out typeresourceId))
                {
                    var typeResource = AppContext.SUB_DIC_TypeResource.FirstOrDefault(e => e.Id == typeresourceId);
                    if (typeResource != null)
                    {
                        fieldValue = typeResource.NameRu + " (" + typeResource.UnitName + ")";
                    }
                }
              
            }

            //----for log
            model.authorid = MyExtensions.GetCurrentUserId();
            model.authorlogin = MyExtensions.GetCurrentUserLogin();
            model.IsError = isError;
            model.SUB_FormComRecord.Add(new SUB_FormComRecord { CreateDate = DateTime.Now, Note = comment, UserId = getCurrentUserId, SUB_FormComment = model, ValueField = fieldValue, authorid= MyExtensions.GetCurrentUserId() , authorlogin= MyExtensions.GetCurrentUserLogin() });
            if (model.Id == 0)
            {

                var history =
                    AppContext.SUB_FormRecordHistory.FirstOrDefault(
                        e => e.TabName == nameTable && e.RecordId == rowId && e.FieldName == fieldName);
                if (history != null)
                {
                    model.Unique = history.Unique;
                }
                AppContext.SUB_FormComment.Add(model);
            }
            AppContext.SaveChanges();
        }

        public string GetIsCommentNotFill(long? getCurrentUserId)
        {
            if (getCurrentUserId == null)
            {
                return "";
            }
            var list = new List<string>();
            var form2Records = AppContext.SUB_Form2Record.Where(e => e.SUB_Form.UserId == getCurrentUserId);
            foreach (var subForm2Record in form2Records)
            {
                if (subForm2Record.OwnSource > 0 && string.IsNullOrEmpty(subForm2Record.Note))
                {
                    if (subForm2Record.FormId != null) list.Add(subForm2Record.FormId.Value.ToString(CultureInfo.InvariantCulture));
                }
            }
            return string.Join(",", list);
        }

            
        public string GetIsForm4NotFill(long? getCurrentUserId)
        {
            if (getCurrentUserId == null)
            {
                return "";
            }
            var list = new List<string>();
            var form4Records = AppContext.SUB_Form4Record.Where(e => e.SUB_Form.UserId == getCurrentUserId);
            if (!form4Records.Any())
            {
                return "";
            }
            foreach (var record in form4Records)
            {
                bool empty = record.EmplPeriod==null || record.ActualInvest == null ||  record.InKind == null || record.InMoney == null;

                if (empty)
                {
                    if (record.FormId != null) list.Add(record.FormId.Value.ToString(CultureInfo.InvariantCulture));
                }
            }
            return string.Join(",", list);
        }


        public string GetIsForm4NotFillGu(long? getCurrentUserId)
        {
            if (getCurrentUserId == null)
            {
                return "";
            }
            var list = new List<string>();
            var form4Records = AppContext.SUB_Form4Record.Where(e => e.SUB_Form.UserId == getCurrentUserId);
            if (!form4Records.Any())
            {
                return "";
            }
            foreach (var record in form4Records)
            {
                bool empty = true;

                if (record.EmplPeriod == null || record.ActualInvest == null || record.InMoney == null)
                    empty = false;

                if (empty)
                {
                    if (record.FormId != null) list.Add(record.FormId.Value.ToString(CultureInfo.InvariantCulture));
                }
            }
            return string.Join(",", list);
        }

        public bool SaveOrUpdateSubjectForm(object form, string industryFormCode)
        {
            bool isSuccess = true;
            switch (industryFormCode)
            {
                case SubjectFormConsts.SubjectForm1:
                    break;
                case SubjectFormConsts.SubjectForm2:
                    var form2 = (SUB_Form2Record)form;
                    AppContext.SUB_Form2Record.AddOrUpdate(form2);
                    break;
                case SubjectFormConsts.SubjectForm3:
                    var form3 = (SUB_Form3Record)form;
                    AppContext.SUB_Form3Record.AddOrUpdate(form3);
                    break;
                case SubjectFormConsts.SubjectForm4:
                    var form4 = (SUB_Form4Record)form;
                    AppContext.SUB_Form4Record.AddOrUpdate(form4);
                    break;
                case SubjectFormConsts.SubjectForm5:
                    var form5 = (SUB_Form5Record)form;
                    AppContext.SUB_Form5Record.AddOrUpdate(form5);
                    break;
                case SubjectFormConsts.SubjectForm6:
                    var form6 = (SUB_Form6Record)form;
                    AppContext.SUB_Form6Record.AddOrUpdate(form6);
                    break;
            }
            try
            {
                AppContext.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public IEnumerable<SUB_FormRecord> GetCommonReestrsByFilter(SUB_FormFilter filter)
        {
            string query =
               "select r.\"UserId\", r.\"Id\",r.\"IsBack\", r.\"Editor\",r.\"SendDate\" as \"SendDate\",u.\"BINIIN\", u.\"IDK\",u.\"JuridicalName\",r.\"StatusId\",u.\"Oblast\"," +
               "sf.\"NameRu\" as \"StatusName\", " +
              "k.\"NameRu\" as \"OblastName\" " +
               "from public.\"SUB_Form\"  as r  " +
               "inner join public.\"SUB_DIC_Status\" as sf on sf.\"Id\"=r.\"StatusId\" " +
               "inner join public.\"SEC_User\" as u on u.\"Id\"=r.\"UserId\" " +
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
            if (!string.IsNullOrEmpty(filter.JuridicalName))
            {
                query = query + " LOWER(u.\"JuridicalName\") like LOWER('%" + filter.JuridicalName + "%') AND ";
            }

            if (filter.Statuses != null && filter.Statuses.Count > 0)
            {
                query = query + " r.\"StatusId\" IN (" + string.Join(",", filter.Statuses) + ") AND ";
            }
         
            if (filter.Oblasts != null && filter.Oblasts.Count > 0)
            {
                query = query + " u.\"Oblast\" IN (" + string.Join(",", filter.Oblasts) + ") AND ";
            }
            if (filter.ReportYear != null && filter.ReportYear > 2000)
            {
                query = query + " r.\"ReportYear\"=" + filter.ReportYear + " AND";
            }
            if (filter.SendId != null && filter.SendId.Value > 0)
            {
                switch (filter.SendId.Value)
                {
                    case CodeConstManager.SUB_REASON_SEND_ID:
                    {
                        query = query + " r.\"IsBack\"='true' AND";
                        break;
                    }
                    case CodeConstManager.SUB_REASON_NOTSEND_ID:
                    {
                        query = query + " r.\"IsBack\"<>'true' AND";
                        break;
                    }
                }
            }
            query = query + " r.\"StatusId\">1 order by r.\"SendDate\", r.\"Id\" DESC";
            return AppContext.Database.SqlQuery<SUB_FormRecord>(query);
        }

        public List<SUB_FormRecordHistory> GetSubFormRecordHistories(long modelId,string tableName, string fieldName, long? rowId)
        {
            return
                AppContext.SUB_FormRecordHistory.Where(
                    e => e.FormId == modelId && e.TabName == tableName && e.FieldName == fieldName && e.RecordId == rowId.Value).ToList();
        }

        public void RegistredInReport(long modelId, int year, long userId)
        {
            var report = AppContext.RST_ReportReestr.FirstOrDefault(e => e.RST_Report.ReportYear == year && e.UserId == userId);
            if (report != null)
            {
                report.FormId = modelId;
                AppContext.SaveChanges();
            }

        }

		public string GetOblastName(long userId,int reportYear,ref string oblastName, string lang)
		{
			string ErrorMessage = "";
			try
			{
				var rst = AppContext.RST_ReportReestr.FirstOrDefault(x => x.UserId == userId && x.RST_Report.ReportYear==reportYear);
				if (rst != null)
				{
					oblastName = (lang.Equals("kz")) ? rst.DIC_Kato.NameKz : rst.DIC_Kato.NameRu;
					//string query = " select t2.* from \"SEC_User\" t,\"RST_ReportReestr\" t1 , \"DIC_Kato\" t2 ,\"RST_Report\" t3 where t.\"Id\"="+idUser+" and t.\"BINIIN\"=t1.\"BINIIN\" and  "
					//			   + " t1.\"Oblast\"=t2.\"Id\" and t1.\"ReportId\"=t3.\"Id\" and t3.isactive=true  ";
										
					//var row = AppContext.Database.SqlQuery<DIC_Kato>(query).FirstOrDefault();
					//if (row != null)
					//{
					//	oblastName = (lang.Equals("kz")) ? row.NameKz : row.NameRu;
					//}
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public List<string> GetRstReportReestrOked(long userId, int ReportYear)
		{
			List<string> list = new List<string>();

			var reportreestr = AppContext.Database.SqlQuery<RST_ReportReestr>("select t.* from \"RST_ReportReestr\" t ,\"RST_Report\"  t1 where t.\"ReportId\"=t1.\"Id\" and t.\"UserId\"=" + userId + " and t1.\"ReportYear\"=" + ReportYear).FirstOrDefault();
			if (reportreestr != null)
			{
				var rows = AppContext.rst_reportreestroked.Where(x => x.reportreestrid == reportreestr.Id).ToList();
				foreach (var row in rows)
				{
					list.Add(row.okedid.ToString());
				}
			}
			return list;
		}
		public string InsertRstReportReestrOked(long reportreestrId, List<string> waves)
		{
			String ErrorMessage = "";
			try
			{
				var rows = AppContext.rst_reportreestroked.Where(x => x.reportreestrid == reportreestrId).ToList();
				foreach (var item in rows)
				{
					bool flag = false;
					foreach (var str in waves)
						if (item.okedid.ToString().Equals(str))
						{
							flag = true;
						}

					if (!flag)
					{
						AppContext.rst_reportreestroked.Remove(item);
						AppContext.SaveChanges();
					}
				}

				foreach (var item in waves)
				{

					if (!string.IsNullOrWhiteSpace(item))
					{
						var check_row = rows.FirstOrDefault(x => x.okedid == Convert.ToInt64(item));
						if (check_row == null)
						{
							var new_waves = new rst_reportreestroked
							{
								reportreestrid = reportreestrId,
								okedid = Convert.ToInt64(item)
							};

							AppContext.rst_reportreestroked.Add(new_waves);
							AppContext.SaveChanges();
						}
					}

				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

		public void CopySecUserToRstReportReestr()
		{
			var userId = MyExtensions.GetCurrentUserId();
			int ReportYear = new SubFormRepository().CreateReportYearActual(userId);
		    var report =AppContext.RST_ReportReestr.FirstOrDefault(e => e.UserId == userId.Value && e.RST_Report.ReportYear == ReportYear);

			#region
			string query = " UPDATE \"RST_ReportReestr\" r "
				+ " SET "
				+ "   usrfirstname         =u.\"FirstName\" , "
				+ "   usrlastname          =u.\"LastName\"   , "
				+ "   usrsecondname        =u.\"SecondName\"  , "
				+ "   usrjuridicalname     =u.\"JuridicalName\" , "
				+ "   usrpost              =u.\"Post\"           , "
				+ "   usrmobile            =u.\"Mobile\"          , "
				+ "   usrworkphone         =u.\"WorkPhone\"        , "
				+ "   usrinternalphone     =u.\"InternalPhone\"    , "
				+ "   usraddress           =u.\"Address\"          , "
				+ "   usriscvazy           =u.\"IsCvazy\"          , "
				+ "   usrresponcefio       =u.\"ResponceFIO\"      , "
				+ "   usrresponcepost      =u.\"ResponcePost\"     , "
				+ "   usroblast            =u.\"Oblast\"           , "
				+ "   usrregion            =u.\"Region\"           , "
				+ "   usrsubregion         =u.\"SubRegion\"        , "
				+ "   usrvillage           =u.\"Village\"          , "
				+ "   usrtypeapplicationid =u.\"TypeApplicationId\", "
				+ "   usrokedid            =u.\"OkedId\"           , "
				+ "   usrfscode            =u.\"FSCode\"           , "
				+ "   usridk               =u.\"IDK\", "
                + "   usremail             =u.\"Email\" "
                + " FROM (SELECT * FROM   \"SEC_User\" ) AS u "
				+ " WHERE r.\"UserId\"=u.\"Id\" and r.\"Id\"=" + report.Id + " and u.\"Id\"=" +userId;
			int result = AppContext.Database.ExecuteSqlCommand(query);
			#endregion
		}

        public void ChangeContactInfo(int reportYear,string fieldName,string val)
        {
            var userId = MyExtensions.GetCurrentUserId();
            var report = AppContext.RST_ReportReestr.FirstOrDefault(e => e.UserId == userId.Value && e.RST_Report.ReportYear == reportYear);

            #region
            string query = " UPDATE \"RST_ReportReestr\"  "
                + " SET usrresponcefio='" + val + "' "
                + " WHERE \"Id\"=" + report.Id;

            if (fieldName.Equals("ResponcePost"))
            {
                query = " UPDATE \"RST_ReportReestr\" "
                               + " SET usrresponcepost='" + val + "'"
                               + " WHERE \"Id\"=" + report.Id;
            }

            int result = AppContext.Database.ExecuteSqlCommand(query);
            #endregion
        }

        public RST_ReportReestr GetRstReportReestrByUserId(long userId, int ReportYear)
		{
			var report = AppContext.RST_ReportReestr.FirstOrDefault(e => e.UserId == userId && e.RST_Report.ReportYear == ReportYear);
			return report;
		}

		//----
		public string ChangeStatus(long userId, int reportYear, int statusId,int formId)
		{
			string ErrorMessage = "";
			try
			{
                var row = AppContext.SUB_Form.FirstOrDefault(x => x.Id == formId);
				if (row != null)
				{
					row.StatusId = statusId;

					var history = new SUB_FormHistory
					{
						FormId = row.Id,
						StatusId = row.StatusId,
						UserId = MyExtensions.GetCurrentUserId(),
						RegDate = DateTime.Now
					};
					AppContext.SUB_FormHistory.Add(history);

					AppContext.SaveChanges();
					//new SendMessageManager().SendSmsToSubjectByChangeStatus(userId, reportYear, statusId);
				}
			}
			catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}
			return ErrorMessage;
		}

        //----
        public string ChangeIDK(long userId, long reestrId, string idk)
        {
            string ErrorMessage = "";
            try
            {
                #region
                string query = " UPDATE \"RST_ReportReestr\"  SET \"IDK\"='" + idk + "' WHERE \"Id\"=" + reestrId+";"
                               + " UPDATE \"SEC_User\"  SET \"IDK\"='" + idk + "' WHERE \"Id\"=" + userId + ";";
                int result = AppContext.Database.ExecuteSqlCommand(query);
                #endregion

                //var user_row = AppContext.SEC_User.FirstOrDefault(x => x.Id == userId);
                //if (user_row != null)
                //{
                //    user_row.IDK = idk;
                //}

                //AppContext.SaveChanges();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            return ErrorMessage;
        }

        //----
        public string ChangeStatusByManager(SUB_Form form)
		{
			string ErrorMessage = "";
			try
			{
                #region kazirwe
                var row = AppContext.SUB_Form.FirstOrDefault(x=>x.Id==form.Id);
				row.StatusId = form.StatusId;
				row.EditDate =(form.SendDate!=null)?form.SendDate.Value.AddDays(1):Convert.ToDateTime("30.03.2019 11:00");
				row.DesignNote = form.DesignNote;
				row.Editor = form.Editor;
				row.DesignDate = (form.SendDate != null) ? form.SendDate.Value.AddDays(1) : Convert.ToDateTime("30.03.2019 11:00");
                AppContext.SaveChanges();
				var user = new AccountRepository().GetUserById(MyExtensions.GetCurrentUserId().Value);
				if (user != null)
				{
					RegJurnalManager.Instance.EditObject(TitleObject, form.Id, "SUB_Form", user.Id, user.Login);
				}
                #endregion

                #region origin
                //var row = AppContext.SUB_Form.FirstOrDefault(x => x.Id == form.Id);
                //row.StatusId = form.StatusId;
                //row.EditDate = DateTime.Now;
                //row.DesignNote = form.DesignNote;
                //row.Editor = form.Editor;
                //row.DesignDate = form.DesignDate;
                //AppContext.SaveChanges();
                //var user = new AccountRepository().GetUserById(MyExtensions.GetCurrentUserId().Value);
                //if (user != null)
                //{
                //    RegJurnalManager.Instance.EditObject(TitleObject, form.Id, "SUB_Form", user.Id, user.Login);
                //}
                #endregion
            }
            catch (Exception ex)
			{
				ErrorMessage = ex.Message;
			}

			return ErrorMessage;
		}

        //----
        public string UpdateSendDate(long reestrId, string sendDate, long? userId)
        {
            string errorMessage = "";
            try
            {
                var reportReestr = AppContext.RST_ReportReestr.FirstOrDefault(e => e.Id == reestrId);
                if (reportReestr == null)
                {
                    errorMessage = "Not found reportReestr (Id=" + reestrId + ")";
                }
                var subForm = AppContext.SUB_Form.FirstOrDefault(x => x.Id == reportReestr.FormId);
                if (subForm == null)
                {
                    errorMessage="Not found subForm (Id=" + reportReestr.FormId + ")";
                }

                var arr = sendDate.Split('/');
                sendDate = arr[1] + "." + arr[0] + "." + arr[2];
                subForm.SendDate = Convert.ToDateTime(sendDate);

                var history = new RST_ReestrReportHistory
                {
                    StatusId = reportReestr.StatusId,
                    Author = userId,
                    UserId = reportReestr.UserId,
                    Oblast = reportReestr.Oblast,
                    ReportYear = reportReestr.RST_Report.ReportYear,
                    RegDate = DateTime.Now,
                    RST_ReportReestr = reportReestr,
                    ReasonId = reportReestr.ReasonId,
                    Note = "",
                    Expectant = reportReestr.Expectant
                };
                AppContext.RST_ReestrReportHistory.Add(history);
                AppContext.SaveChanges();
            }catch(Exception ex)
            {
                errorMessage = ex.Message;
            }
            return errorMessage;
        }

        public List<SubCommentsEntity> GetAllCommentsByFormId(long formId,bool isGu)
        {
            #region
            string query = " select sc.\"Id\" , sr.\"Note\", sr.\"CreateDate\" ,sc.\"IsError\" , sc.\"RowIndex\" , sc.\"ColumnIndex\" , "
                        + "  s.\"LastName\" , s.\"FirstName\" , s.\"SecondName\" ,  "
                        + "  CASE WHEN sc.\"TableName\"='mainTable' THEN 'Форма 1'  "
                        + "  WHEN sc.\"TableName\"='form2' THEN 'Форма 2' "
                        + "  WHEN sc.\"TableName\"='form3' THEN 'Форма 2а' "
                        + "  WHEN sc.\"TableName\"='form4' THEN 'Форма 3' "
                        + "  WHEN sc.\"TableName\"='form5' THEN 'Форма 4' "
                        + "  WHEN sc.\"TableName\"='form6' THEN 'Форма 5' "
                        + "  WHEN sc.\"TableName\"='contactTable' THEN 'Контакты' "
                        + "  WHEN sc.\"TableName\"='tabKadastr' THEN 'Кадастр' "
                        + "  else sc.\"TableName\" end as \"TableName\" "
                        + "  from \"SUB_FormComment\" sc , \"SUB_FormComRecord\" sr  "
                        + "  left join \"SEC_User\"  s on sr.\"UserId\"=s.\"Id\" "
                         + " where sc.\"Id\"=sr.\"CommentId\"  and sc.\"FormId\"="+formId+" order by sc.\"Id\" ";

            if (isGu)
            {
                query = " select sc.\"Id\" , sr.\"Note\", sr.\"CreateDate\" ,sc.\"IsError\" , sc.\"RowIndex\" , sc.\"ColumnIndex\" , "
                        + "  s.\"LastName\" , s.\"FirstName\" , s.\"SecondName\" ,  "
                        + "  CASE WHEN sc.\"TableName\"='mainTable' THEN 'Форма 1'  "
                        + "  WHEN sc.\"TableName\"='form2' THEN 'Форма 2' "
                        + "  WHEN sc.\"TableName\"='form3' THEN 'Форма 2' "
                        + "  WHEN sc.\"TableName\"='form4' THEN 'Форма 3' "
                        + "  WHEN sc.\"TableName\"='form5' THEN 'Форма 4' "
                        + "  WHEN sc.\"TableName\"='form6' THEN 'Форма 5' "
                        + "  WHEN sc.\"TableName\"='contactTable' THEN 'Контакты' "
                        + "  WHEN sc.\"TableName\"='tabKadastr' THEN 'Кадастр' "
                        + "  else sc.\"TableName\" end as \"TableName\" "
                        + "  from \"SUB_FormComment\" sc , \"SUB_FormComRecord\" sr  "
                        + "  left join \"SEC_User\"  s on sr.\"UserId\"=s.\"Id\" "
                         + " where sc.\"Id\"=sr.\"CommentId\"  and sc.\"FormId\"=" + formId + " order by sc.\"Id\" ";
            }
            #endregion
            var list = AppContext.Database.SqlQuery<SubCommentsEntity>(query).ToList();
            foreach (var item in list)
            {
                item.CreateDateStr = (item.CreateDate != null) ? item.CreateDate.Value.ToString("dd.MM.yyyy HH:mm") : "";
              
            }

            return list;
        }

        public List<PdfTable> GetPdfTable(string katoCode)
        {
            string query = "select * from \"PdfTable\" where \"KatoCode\"='"+katoCode+ "' order by \"Id\" ";
            var list = AppContext.Database.SqlQuery<PdfTable>(query).ToList();
            return list; 
        }

        public List<PdfTable> GetPdfTable2()
        {
            string query = "select * from \"PdfTable\"  order by \"Id\" ";
            var list = AppContext.Database.SqlQuery<PdfTable>(query).ToList();
            return list;
        }
    }
}