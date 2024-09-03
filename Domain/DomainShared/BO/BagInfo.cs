using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class BagInfo : List<KeyValuePair<string, string>>
    {
        public object ToKeyValueList()
        {
            throw new NotImplementedException();
        }
    }
}
