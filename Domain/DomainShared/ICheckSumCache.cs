using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared
{
    public interface ICheckSumCache
    {
        string Get(string filepath, DateTime lastModified);
        void Put(string filepath, DateTime lastModified, string checkSum);
    }
}
