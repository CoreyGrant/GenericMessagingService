using Org.BouncyCastle.Crypto.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Cache
{
    internal class MemoryCacheManager : ICacheManager
    {
        private Dictionary<string, Dictionary<string, object>> cache;

        public async Task<T?> Get<T>(string name, string key) where T : class
        {
            if (cache.ContainsKey(name))
            {
                var innerCache = cache[name];
                if (innerCache.ContainsKey(key))
                {
                    return (T) innerCache[key];
                }
            }
            return default;
        }

        public async Task Set<T>(string name, string key, T value) where T : class
        {
            if (!cache.ContainsKey(name))
            {
                cache[name] = new Dictionary<string, object>();
            }
            cache[name][key] = value;
        }
    }
}
