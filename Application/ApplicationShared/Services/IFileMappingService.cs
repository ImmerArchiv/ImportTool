using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Application.Shared.BO;

namespace Archiv10.Application.Shared.Services
{
    public interface IFileMappingService
    {
        void Add(UIFile file, UIData data);
        void Commit();
    }
}
