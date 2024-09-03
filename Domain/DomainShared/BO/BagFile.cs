using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class BagFile
    {
        public string FileName { get; set; }
        public string CheckSum { get; set; }
        public string TempName { get; set; }
    }
}
