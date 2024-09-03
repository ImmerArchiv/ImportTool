using Archiv10.Domain.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using System.IO;

namespace Archiv10.Domain.Impl.Services
{
    class NameService : INameService
    {
        public string CreateBagitName(string rootpath, string path, NameType nameType)
        {

            if (String.IsNullOrWhiteSpace(rootpath))
                throw new ArgumentException("rootpath");

            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path");

            switch (nameType)
            {
                case NameType.NameFromFolderName:
                    return Path.GetFileName(path);
                case NameType.NameFromRelativePath:
                    var rp = Path.GetFileName(rootpath);
                    var ix = rootpath.Length - rp.Length;
                    if(ix < 0)
                        throw new Exception(rootpath + " is not a path");
                    return path.Substring(ix).Replace(@"\","/");
                default:
                    throw new Exception(nameType.ToString() + " not handled in CreateBagitName");

            }
        }
    }
}
