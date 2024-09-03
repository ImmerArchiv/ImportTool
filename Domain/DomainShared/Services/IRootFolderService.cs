using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Domain.Shared.Services
{
    public interface IRootFolderService
    {
        bool Init(RootFolder source);
      
        IList<LocalFolder> ListAll(RootFolder source);
        IList<LocalFile> ListFiles(FolderPath path, string[] filter);
    }
}
