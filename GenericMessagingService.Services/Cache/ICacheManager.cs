using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Cache
{
    public interface ICacheManager
    {
        Task Set<T>(string name, string key, T value) where T : class;
        Task<T?> Get<T>(string name, string key) where T : class;
    }
}
