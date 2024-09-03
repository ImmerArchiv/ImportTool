using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;

namespace Archiv10.Domain.Shared.Services
{
    public interface INameService
    {
        string CreateBagitName(string rootpath, string path, NameType nameType);
    }
}
