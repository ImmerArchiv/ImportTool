using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class Bag
    {
        public Bag()
        {
            Data = new List<BagData>();
        }
        public BagId   Id { get; set; }
        public BagInfo Info { get; set; }
        public BagStatus Status { get; set; }
        public IList<BagData> Data { get; set; }

    }
}
