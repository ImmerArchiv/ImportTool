using Archiv10.Application.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using Archiv10.Infrastructure.Shared;

namespace Archiv10.Application.Impl.Services
{
    class FolderBagitMappingService : IFolderBagitMappingService
    {
        private IDictionary<string, IDictionary<string, HashSet<string>>> mapping = new Dictionary<string, IDictionary<string, HashSet<string>>>();
        private readonly IFileService _fileService;

        public FolderBagitMappingService(IFileService fileService)
        {
            _fileService = fileService;
        }
     
        public BagId Get(FolderPath sourceFolder)
        {
            String folderKey = sourceFolder.ToString();

            if (!mapping.ContainsKey(folderKey)) return null;

            if (mapping[folderKey].Count == 0)
                throw new ArgumentException("no bagit registered for " + folderKey);

            int max = 0;
            string bagid = null;
            foreach(var kv in mapping[folderKey])
            {
                if(bagid == null || max < kv.Value.Count)
                {
                    max = kv.Value.Count;
                    bagid = kv.Key;
                }
            }

            return new BagId(bagid);

        }

        public void Incr(FolderPath sourceFolder, BagId sourceBag, string itemKey)
        {

            String folderKey = sourceFolder.ToString();
            String bagKey = sourceBag.ToString();

            if (!mapping.ContainsKey(folderKey))
                mapping[folderKey] = new Dictionary<string, HashSet<string>>();

            if (!mapping[folderKey].ContainsKey(bagKey))
                mapping[folderKey][bagKey] = new HashSet<string>();

            mapping[folderKey][bagKey].Add(itemKey);

        }


        public void Commit()
        {
            _fileService.WriteCache("folderbagitmapping", mapping);
        }


    }
}
