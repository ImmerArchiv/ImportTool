using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared
{
    public class RepositoryConfig
    {
        public const int DataPartSize = 3 * 16; //1 Line as Base64
        public const int DataSize = DataPartSize * 1024 * 10; //48KB
    }
}
