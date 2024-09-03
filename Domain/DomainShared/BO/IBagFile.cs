using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public interface IBagFile
    {
        string FileName { get; set; }
        string CheckSum { get; set; }
        IList<string> Data { get; set; }
    }
}
