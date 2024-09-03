using Archiv10.Domain.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using Archiv10.Infrastructure.Shared;
using System.IO;

namespace Archiv10.Domain.Impl.Services
{
    class FileMappingValidatorService : IFileMappingValidatorService
    {
        private Dictionary<string, IDictionary<string, string>> mapping;

        public FileMappingValidatorService(IFileService fileService)
        {
            mapping = fileService.ReadCache<Dictionary<string, IDictionary<string, string>>>("filemapping");
            if (mapping == null)
                mapping = new Dictionary<string, IDictionary<string, string>>();
        }
        public LocalFileState Validate(FolderPath path, BagData data)
        {
            var filename = Path.Combine(path.ToString(), data.Name);

            if(!mapping.ContainsKey(filename))
            {
                //Noch nicht hochgeladen
                if (data.CheckSum.Equals("d41d8cd98f00b204e9800998ecf8427e"))
                    return LocalFileState.EmptyFile;
                return LocalFileState.MustUpload;
            }

            var values = mapping[filename];
            var cksum = values["cksum"];
            if (!cksum.Equals(data.CheckSum))
                return LocalFileState.WrongCheckSum;

            if(!values["syncronized"].Equals("True"))
                return LocalFileState.NotSynchronized;
       

            return LocalFileState.Ok;
        }
    }
}
