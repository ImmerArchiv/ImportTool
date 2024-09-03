using System;
using System.Numerics;

namespace Archiv10.Domain.Shared.BO
{
    public class RepositoryEndpointStatus
    {
        public long Bagits { get; set; }
        public long Files { get; set; }
        public DateTime LastModified { get; set; }
        public BigInteger MaxSize { get; set; }
        public BigInteger Size { get; set; }
    }
}