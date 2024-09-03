using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Application.Shared.Services
{
    public interface IFolderBagitMappingService
    {
        BagId Get(FolderPath sourceFolder);
        void Incr(FolderPath sourceFolder, BagId sourceBag,string itemKey);
        void Commit();
    }
}
