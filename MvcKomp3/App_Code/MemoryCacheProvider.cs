using System;
using System.Web;
using System.Web.Caching;

namespace Samples
{
    public class MemoryCacheProvider : OutputCacheProvider
    {
        public override object Add(string key, object entry, DateTime utcExpiry)
        {
            object result = HttpRuntime.Cache[key];
            if (result == null)
            {
                this.Set(key, entry, utcExpiry);
                result = entry;
            }
            return result;
        }

        public override object Get(string key)
        {
            return HttpRuntime.Cache[key];
        }

        public override void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public override void Set(string key, object entry, DateTime utcExpiry)
        {
            HttpRuntime.Cache.Insert(key, entry, null, utcExpiry,
                Cache.NoSlidingExpiration, CacheItemPriority.High, null);
        }
    }
}