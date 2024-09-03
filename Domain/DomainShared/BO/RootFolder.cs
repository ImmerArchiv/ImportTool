using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class RootFolder
    {
        public string Path { get; set; }

        public string[] Filter { get; set; }

        public NameType Naming { get; set; }

    }
}
