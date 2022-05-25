using Aisger.Utils;
using Npgsql;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace Aisger.Models.Repository.Security
{
    public class GerNavigateLogger : System.Web.Mvc.FilterAttribute, System.Web.Mvc.IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            string conString = ConfigurationManager.AppSettings["conName"];
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            using (NpgsqlConnection connect = new NpgsqlConnection(conString))//"Server=92.46.109.245; Port=5432; User Id=postgres; Password=672azXC9eI; Database=aisgerTest18; Pooling=true;"))
            {
                connect.Open();

                object count = null;
                using (NpgsqlCommand c = connect.CreateCommand())
                {
                    c.CommandText = @"select count(*) from public.""SEC_UA_CONTROLLERS"" where ""CONTROLLER_NAME"" = '" + controller + "'";
                    count = c.ExecuteScalar();
                }

                if (count.ToString() != "1")
                {
                    using (NpgsqlCommand c = connect.CreateCommand())
                    {
                        c.CommandText = @"insert into public.""SEC_UA_CONTROLLERS"" (""CONTROLLER_NAME"", ""DESCR"") values ('" + controller + "', '" + controller + "')";
                        count = c.ExecuteNonQuery();
                    }
                }

                if (count.ToString() == "1")
                {
                    using (NpgsqlCommand c = connect.CreateCommand())
                    {
                        c.CommandText = @"select count(*) from public.""SEC_UA_ACTIONS"" where ""SEC_UA_CONTROLLER"" = '" + controller + @"' and ""ACTION_NAME"" = '" + action + "'";
                        count = c.ExecuteScalar();
                    }
                }

                if (count.ToString() != "1")
                {
                    using (NpgsqlCommand c = connect.CreateCommand())
                    {
                        c.CommandText = @"insert into public.""SEC_UA_ACTIONS"" (""SEC_UA_CONTROLLER"", ""ACTION_NAME"", ""DESCR"") values ('" + controller + "', '" + action + "', '" + controller + "\\" + action + "')";
                        count = c.ExecuteNonQuery();
                    }
                }

                if (count.ToString() == "1")
                {
                    var user = (SEC_User)HttpContext.Current.Session[CodeConstManager.SESSION_USER];
                    if (user == null)
                        return;

                    using (NpgsqlCommand c = connect.CreateCommand())
                    {
                        c.CommandText = @"
                        insert into public.""SEC_UA_EVENTS"" (""RealDate"", ""RealUser"", ""SEC_UA_EVENTTYPE"", ""AuthorId"", ""AuthorLogin"", ""Controller"", ""Action"") 
                        values (current_timestamp, current_user, 10, " + user.Id + ", '" + user.Login + "', '" + controller + "', '" + action + "')";
                        count = c.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}