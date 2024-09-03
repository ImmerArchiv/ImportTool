using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.Services
{
    public interface ICheckSumService
    {
        string CalcForFile(string filename);
    }
}
