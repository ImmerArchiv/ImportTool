using Archiv10.Domain.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using Archiv10.Infrastructure.Shared.Locator;
using System.IO;
using Archiv10.Domain.Shared.Locator;
using Archiv10.Domain.Shared;

namespace Archiv10.Domain.Impl.Services
{
    class RootFolderService : IRootFolderService
    {
        private readonly ICheckSumCache _checkSumCache;

        public RootFolderService(ICheckSumCache checkSumCache)
        {
            _checkSumCache = checkSumCache;
        }

        public bool Init(RootFolder rootfolder)
        {
            var fileservice = InfrastructureLocator.GetFileService();
            return fileservice.CheckDirectory(rootfolder.Path);
        }

        public IList<LocalFolder> ListAll(RootFolder rootfolder)
        {
            var fileservice = InfrastructureLocator.GetFileService();
            var list = new List<LocalFolder>();

            foreach (var path in fileservice.ResolveDirectories(rootfolder.Path))
            {
                var folder = new LocalFolder();
                folder.Path = new FolderPath(path);
                list.Add(folder);
            }

            return list;
        }

        public IList<LocalFile> ListFiles(FolderPath path,string[] filter)
        {
            var fileservice = InfrastructureLocator.GetFileService();
            var list = new List<LocalFile>();

            var checkSumService = DomainLocator.GetCheckSumService("md5");
            foreach (var filepath in fileservice.ListFiles(path.ToString(), DomainLocator.GetFileFilter(filter)))
            {
                var lastModified = fileservice.GetLastModified(filepath);
                if((DateTime.Now - lastModified) < TimeSpan.FromSeconds(30))
                {
                    //Ignore
                    continue;
                }

                var file = new LocalFile();
                file.data = new BagData();
                file.data.Name = Path.GetFileName(filepath);
                file.data.Length = fileservice.GetFileLength(filepath);

                //get Checksum from Cache or calculate it
                file.data.CheckSum = _checkSumCache.Get(filepath, lastModified);
                if (file.data.CheckSum == null)
                {
                    file.data.CheckSum = checkSumService.CalcForFile(filepath);
                    _checkSumCache.Put(filepath, lastModified, file.data.CheckSum);
                }
                list.Add(file);
            }
            return list;
        }
    }
}
