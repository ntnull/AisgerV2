using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Aisger.Models;
using System.Net.Mail;
using Aisger.Models.Repository.Map;
using Aisger.Models.Repository.Security;
using NPOI.SS.Formula.Functions;
using Aisger.Models.Repository.Dictionary;
using Aisger.Models.Repository.Subject;

namespace Aisger.Utils
{
    public class SendMessageManager
    {
        public void SendMapDesign(long refReestr, string note, string[] files)
        {
            try
            {
                var model = new MapApplicationRepository().GetById(refReestr);
                if (model == null)
                {
                    return;
                }
                model.DesignNote = note;
                new MapApplicationRepository().SaveOrUpdate(model, MyExtensions.GetCurrentUserId());
                var user = model.SEC_User1;
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    return;
                }


                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.mail.ru");
                mail.From = new MailAddress("no-reply-info@aisger.kz");

                AddMailTo(mail, model.SEC_User1);
//                mail.To.Add("diweb@mail.ru");
                var subject = "№ Заявления:" + model.AppNumber;
                mail.Subject = subject;

                var builder = new StringBuilder();
                builder.AppendLine("№ Заявления:" + model.AppNumber).Append("<br>");
                builder.AppendLine("Дата регистрации:" + model.SendDateStr).Append("<br>");

                if (model.MAP_DIC_Status != null)
                {
                    builder.AppendLine("Статус:" + model.MAP_DIC_Status.NameRu).Append("<br>");

                }
                builder.AppendLine("Решение:" + note).Append("<br>");
                mail.IsBodyHtml = true;
                mail.Body = builder.ToString();
                foreach (var file in files)
                {
                    mail.Attachments.Add(new Attachment(file));
                }
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("no-reply-info@aisger.kz", "Hsdfd78#$asd");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public void SendMapRegistred(long refReestr)
        {
            try
            {
                var model = new MapApplicationRepository().GetById(refReestr);
                if (model == null)
                {
                    return;
                }
                var users = new AccountRepository().GetByRightRole(CodeConstManager.MapNotify);
                if (users == null || !users.Any())
                {
                    return;
                }


                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.mail.ru");
                mail.From = new MailAddress("no-reply-info@aisger.kz");
                foreach (var secUser in users)
                {
                    AddMailTo(mail, secUser);
                }

                var subject = "№ Заявления:" + model.AppNumber;
                mail.Subject = subject;

                var builder = new StringBuilder();
                builder.AppendLine("№ Заявления:" + model.AppNumber).Append("<br>");
                builder.AppendLine("Дата регистрации:" + model.SendDateStr).Append("<br>");

                builder.AppendLine("№ Заявитель:" + model.SEC_User1.ApplicationName).Append("<br>");
                mail.IsBodyHtml = true;
                mail.Body = builder.ToString();
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("no-reply-info@aisger.kz", "Hsdfd78#$asd");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public void SendSubForm(SUB_Form form)
        {
            try
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.mail.ru");
                mail.From = new MailAddress("no-reply-info@aisger.kz");
				var subUser = new AccountRepository().GetUserById(form.UserId);
                AddMailTo(mail, subUser);

                var subject = "Отчетность ГЭР, за отчетный период " + form.ReportYear;
                mail.Subject = subject;

                var builder = new StringBuilder();
                builder.AppendLine("Дата отправки:" + form.DesignDateStr).Append("<br>");
				var dic_status = new SubDicStatusRepository().GetAll().FirstOrDefault(x=>x.Id==form.StatusId);
				builder.AppendLine("Решение :" + dic_status.NameRu).Append("<br>");

                builder.AppendLine("Ответ: " + form.DesignNote).Append("<br>");

				mail.IsBodyHtml = true;
                mail.Body = builder.ToString();
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("no-reply-info@aisger.kz", "Hsdfd78#$asd");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public void SendSubActionPlan(SUB_ActionPlan form)
        {
            try
            {
                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.mail.ru");
                mail.From = new MailAddress("no-reply-info@aisger.kz");
                AddMailTo(mail, form.SEC_User1);

                var subject = "План мероприятий, за отчетный период " + form.ReportYear;
                mail.Subject = subject;

                var builder = new StringBuilder();
                builder.AppendLine("Дата отправки:" + form.DesignDateStr).Append("<br>");
                builder.AppendLine("Решение :" + form.SUB_DIC_Status.NameRu).Append("<br>");

                builder.AppendLine("Ответ: " + form.DesignNote).Append("<br>");
                mail.IsBodyHtml = true;
                mail.Body = builder.ToString();

                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("no-reply-info@aisger.kz", "Hsdfd78#$asd");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public void SendMapApplicationEE2(long userId) {

            try
            {
                var currUser = new AccountRepository().GetUserById(userId);

                var users = new AccountRepository().GetByRightRole(CodeConstManager.MapApplicationEE2Notify);
                if (users == null || !users.Any())
                {
                    return;
                }


                var mail = new MailMessage();
                var smtpServer = new SmtpClient("smtp.mail.ru");
                mail.From = new MailAddress("no-reply-info@aisger.kz");
                foreach (var secUser in users)
                {
                    AddMailTo(mail, secUser);
                }

               // AddMailTo(mail,currUser);

                var subject = "Наименование организации:" + currUser.JuridicalName;
                mail.Subject = subject;

                var builder = new StringBuilder();
                builder.AppendLine("№ Заявитель:" + currUser.JuridicalName+"  "+currUser.DIC_Kato.NameRu).Append("<br>");
                builder.AppendLine("Статус:Предоставил").Append("<br>");
                builder.AppendLine("Дата:" + DateTime.Now).Append("<br>");

                mail.IsBodyHtml = true;
                mail.Body = builder.ToString();
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("no-reply-info@aisger.kz", "Hsdfd78#$asd");
                smtpServer.EnableSsl = true;

                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                return;
            }

        }

		public void SendSmsToSubjectByCreateReport(long? userId,int year,string type,string signFio)
		{
			try
			{
				var currUser = new AccountRepository().GetUserById(userId);
			
				var mail = new MailMessage();
				var smtpServer = new SmtpClient("smtp.mail.ru");
				mail.From = new MailAddress("no-reply-info@aisger.kz");
				
				//----
				var builder = new StringBuilder();
				if (type.Equals("subject"))
				{					
			    	AddMailTo(mail, currUser);					

					var subject = "Уведомление об успешной отправке отчета ГЭР";
					mail.Subject = subject;

					builder.AppendLine("Отчет по ГЭР был успешно отправлен. ").Append("<br>");
					builder.AppendLine("Отчетный период: " + year + " год. ").Append("<br>");
					builder.AppendLine("Субъект:" + currUser.JuridicalName).Append("<br>");
					builder.AppendLine("ФИО отправителя:" + signFio).Append("<br>");
					builder.AppendLine("Дата и время отправки:" + DateTime.Now).Append("<br>");
				}
				else { 

					//---- send to mananger
					List<SEC_User> users = new List<SEC_User>();
					string ErrorMessage=new SecUserRepository().GetManagerListForNotifyGer(ref users, userId);
					if (ErrorMessage != "" || users == null || users.Count == 0)
						return;

					//---- send to subject
					foreach (var secUser in users)
					{
						AddMailTo(mail, secUser);
					}

					var subject = "Новый отчет по ГЭР от "+currUser.JuridicalName;
					mail.Subject = subject;

					builder.AppendLine("Поcтупил новый отчет по ГЭР от Субъекта. ").Append("<br>");
					builder.AppendLine("БИН:"+currUser.BINIIN+", "+currUser.JuridicalName).Append("<br>");
					builder.AppendLine("Область:"+currUser.Oblast).Append("<br>");
					builder.AppendLine("Отчетный период: " + year + " год. ").Append("<br>");
					builder.AppendLine("Дата и время поступления:" + DateTime.Now).Append("<br>");				
				}

				builder.AppendLine("Портал АИС ГЭР http://aisger.kz").Append("<br>");
				builder.AppendLine("Данное письмо было сформировано автоматически, отвечать на него не нужно.").Append("<br>");

				mail.IsBodyHtml = true;
				mail.Body = builder.ToString();
				smtpServer.Port = 587;
				smtpServer.Credentials = new System.Net.NetworkCredential("no-reply-info@aisger.kz", "Hsdfd78#$asd");
				smtpServer.EnableSsl = true;

				smtpServer.Send(mail);
			}
			catch (Exception ex)
			{
				return;
			}

		}

		public void SendSmsToSubjectByChangeStatus(long userId, int year, long statusId)
		{
			try
			{
				var currUser = new AccountRepository().GetUserById(MyExtensions.GetCurrentUserId());

				var mail = new MailMessage();
				var smtpServer = new SmtpClient("smtp.mail.ru");
				mail.From = new MailAddress("no-reply-info@aisger.kz");
				var subjectUser = new AccountRepository().GetUserById(userId);
				AddMailTo(mail, subjectUser);

				//----
				var builder = new StringBuilder();

				var dic_status = new SubDicStatusRepository().GetAll().FirstOrDefault(x => x.Id == statusId);
				string jurdicalName = "";

				var rst_reportreestr = new SubFormRepository().GetRstReportReestrByUserId(userId, year);
				if (rst_reportreestr != null)
					jurdicalName = rst_reportreestr.usrjuridicalname;

				builder.AppendLine("Данное письмо было сформировано автоматически в ИС «Государственный энергетический реестр».").Append("<br>");
				builder.AppendLine("Субъект ГЭР:" + jurdicalName).Append("<br>");
				builder.AppendLine("Отчетный период: " + year + " год. ").Append("<br>");
				builder.AppendLine("Дата и время отправки уведомления:" + DateTime.Now).Append("<br>");
				builder.AppendLine("Портал АИС ГЭР http://aisger.kz").Append("<br><br>");
				builder.AppendLine("Статус представленного отчета по ГЭР изменился на " + dic_status.NameRu + " ").Append("<br><br>");
				if (statusId != 3)
					builder.AppendLine("Пожалуста войдите в систему АИС ГЭР (http://aisger.kz), откорректируйте отчет и повторно сдайте.").Append("<br>");

				builder.AppendLine("Данное письмо было сформировано автоматически, отвечать на него не нужно.").Append("<br>");


				//---- send to subject
			

				var subject = "Уведомление об изменении статуса отчета";
				mail.Subject = subject;

				mail.IsBodyHtml = true;
				mail.Body = builder.ToString();
				smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("no-reply-info@aisger.kz", "Hsdfd78#$asd");//"no-reply@aisger.kz", "qTD67u-szIyY");
				smtpServer.EnableSsl = true;
				smtpServer.Send(mail);

			}
			catch (Exception ex)
			{
				return;
			}

		}

		private static void AddMailToUsersD(MailMessage mail, SEC_User user, string type)
		{
			if (user == null)
				return;
			
			mail.To.Add(user.Email);
			return;
		}

		private static void AddMailToUsers(MailMessage mail, SEC_User user, string type)
		{
			if (user == null)
				return;
			if (type.Equals("subject"))
			{
				mail.To.Add("berik91_20@mail.ru");
			}
			else
			{
				List<SEC_User> list = new List<SEC_User>();
				string errorMessage = new SecUserRepository().GetGerNotifyManagers(ref list);
				foreach (var item in list)
				{
					if (item.Email != null)
						mail.To.Add(item.Email);
				}
			}

			return;
		}

        private static void AddMailTo(MailMessage mail, SEC_User user)
        {		
            if (user == null)
            {
                return;
            }
            if (mail.To.All(e => e.Address != user.Email))
            {
                mail.To.Add(user.Email);
            }
        }


    }
}