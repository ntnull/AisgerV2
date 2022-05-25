using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Aisger.Utils
{
    public class ListModelBinder<T> : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form = controllerContext.HttpContext.Request.Form;

            IList<T> result = new List<T>();

            var re = new Regex(string.Format(@"^{0}\[(\d+)]\.*", bindingContext.ModelName), RegexOptions.IgnoreCase | RegexOptions.Compiled);

            var candidates = form.AllKeys.Where(x => re.IsMatch(x));
            var indices = candidates.Select(x => int.Parse(re.Match(x).Groups[1].Value)).Distinct();

            var PropInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (int i in indices)
            {
                T s = Activator.CreateInstance<T>();
                foreach (var prop in PropInfo)
                {
                    var value = form[string.Format("{0}[{1}].{2}", bindingContext.ModelName, i, prop.Name)];
                    if (prop.PropertyType == typeof(Int64) || prop.PropertyType == typeof(Int64?))
                    {
                        try
                        {
                            s.GetType().GetProperty(prop.Name).SetValue(s, Int64.Parse(value), null);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else if (prop.PropertyType == typeof(double) || prop.PropertyType == typeof(double?))
                    {
                        try
                        {
                            s.GetType().GetProperty(prop.Name).SetValue(s, Double.Parse(value), null);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        s.GetType().GetProperty(prop.Name).SetValue(s, value, null);
                    }
                }
                result.Add(s);
            }

            return result;
        }
    }
}