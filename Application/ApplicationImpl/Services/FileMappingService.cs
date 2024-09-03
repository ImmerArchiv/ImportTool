using Archiv10.Application.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Application.Shared.BO;
using System.IO;
using Archiv10.Infrastructure.Shared;

namespace Archiv10.Application.Impl.Services
{
    class FileMappingService : IFileMappingService
    {
        private IDictionary<string, IDictionary<string, string>> mapping = new Dictionary<string, IDictionary<string, string>>();
        private readonly IFileService _fileService;

        public FileMappingService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void Add(UIFile file, UIData data)
        {
            string path = Path.Combine(file.SourceFolder.ToString(), file.SourceFile.data.Name);
            IDictionary<string,string> values = new Dictionary<string, string>();

            values.Add("cksum", file.SourceFile.data.CheckSum);
            values.Add("bagit", data.SourceBag.ToString());
            values.Add("name", data.SourceData.Name);
            values.Add("syncronized", ""+data.State.Syncronized);

            if (mapping.ContainsKey(path))
                mapping[path] = values;
            else
                mapping.Add(path, values);
        }

        public void Commit()
        {
            _fileService.WriteCache("filemapping", mapping);
        }

    }
}
