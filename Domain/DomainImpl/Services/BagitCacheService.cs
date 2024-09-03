using Archiv10.Domain.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using Archiv10.Infrastructure.Shared;

namespace Archiv10.Domain.Impl.Services
{
    class BagitCacheService : IBagitCacheService
    {
        private IFileService _fileService;
        private IFilenameService _filenameService;

        public BagitCacheService(IFileService fileService,IFilenameService filenameService)
        {
            _fileService = fileService;
            _filenameService = filenameService;
        }

     
        public void Add(string repositoryKey, BagId sourceId, Bag bag)
        {

            var name = _filenameService.CleanName(repositoryKey);
            var cache = _fileService.ReadCache<Dictionary<string, Bag>>(name);
            if (cache == null)
                cache = new Dictionary<string, Bag>();
            cache[sourceId.ToString()] = bag;
            _fileService.WriteCache<Dictionary<string, Bag>>(name,cache);
        }

     

        public Bag Get(string repositoryKey, BagId sourceId)
        {
            var name = _filenameService.CleanName(repositoryKey);
            var cache = _fileService.ReadCache<Dictionary<string, Bag>>(name);
            if (cache == null)
                return null;

            if (!cache.ContainsKey(sourceId.ToString()))
                return null;
            return cache[sourceId.ToString()];


        }
    }
}
