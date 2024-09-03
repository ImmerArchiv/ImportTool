using Archiv10.Domain.Shared;
using Archiv10.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Impl
{
    class CheckSumCache : ICheckSumCache
    {
        private readonly IFileService _fileService;

        private IDictionary<string, string> dictionary = new Dictionary<string, string>();

        public CheckSumCache(IFileService fileService)
        {
            _fileService = fileService;
            dictionary = _fileService.ReadCache<Dictionary<string, string>>("checksum");
            if(dictionary == null)
                dictionary = new Dictionary<string, string>();
        }

        public void Put(String path, DateTime time, String value)
        {
            var key = getkey(path, time);
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
            _fileService.WriteCache<IDictionary<string, string>>("checksum", dictionary);
        }

        public string Get(String path, DateTime time)
        {
            var key = getkey(path, time);

            if (dictionary.ContainsKey(key))
                return dictionary[key];
            return null;

        }

        private string getkey(string path, DateTime time)
        {
            return string.Format("{0}.{1}", path, time.ToFileTime());
        }
    }
}
