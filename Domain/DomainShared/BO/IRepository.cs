using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public interface IRepository
    {
        string Url { get; set; }
        IRepositoryEndpointInfo Info { get; set; }
        string Repository { get; set; }
        string Token { get; set; }
    }
}
