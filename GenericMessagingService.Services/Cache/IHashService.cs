using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Cache
{
    public interface IHashService
    {
        string GetHash(string str);
    }

    public class HashSerice
    {
        private readonly FastHashes.FastHash64 hash;

        public HashSerice()
        {
            hash = new FastHashes.FastHash64();
        }

        public string GetHash(string str)
        {
            var byteStr = Encoding.UTF8.GetBytes(str);
            var hashBytes = hash.ComputeHash(byteStr);
            return Encoding.UTF8.GetString(hashBytes);
        }
    }
}
