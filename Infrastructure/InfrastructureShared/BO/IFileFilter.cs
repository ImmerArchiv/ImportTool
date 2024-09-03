using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Shared.BO
{
    public interface IFileFilter
    {
        bool Match(string file);
    }
}
