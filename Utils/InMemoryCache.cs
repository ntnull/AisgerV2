#region

using System;
using System.Web;
using log4net;

#endregion

namespace Aisger.Utils
{
    public class InMemoryCache : ICacheService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (InMemoryCache));
        public T Get<T>(string cacheID, Func<T> getItemCallback) where T : class
        {
            var item = HttpRuntime.Cache.Get(cacheID) as T;
            if (item != null)
            {
                _logger.InfoFormat("used object from cashe {0}", cacheID);
                return item;
            }
            _logger.InfoFormat("cashe {0} is null fill Cache", cacheID);
            item = getItemCallback();
            HttpContext.Current.Cache.Insert(cacheID, item);
            return item;
        }
    }

    internal interface ICacheService
    {
        T Get<T>(string cacheID, Func<T> getItemCallback) where T : class;
    }
}