using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class BagSnippet
    {
        public BagId Id { get; set; }
        public BagInfo Info { get; set; }
        public BagStatus Status { get; set; }
    }
}
