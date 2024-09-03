using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class BagStatus
    {
        public long Files { get; set; }
        public DateTime LastModified { get; set; }
        public BigInteger Size { get; set; }

        public override bool Equals(object obj)
        {
            var bagStatus = obj as BagStatus;
            if (bagStatus == null) return false;

            if (bagStatus.Files != Files) return false;
            if (bagStatus.Size != Size) return false;

            return (bagStatus.LastModified.Equals(LastModified));

        }

    }
}
