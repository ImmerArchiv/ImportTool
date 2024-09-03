using Archiv10.Infrastructure.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Shared
{
    public interface IWebConnector
    {
        WebResponse Get(WebRequest request);

        WebResponse Post(WebRequest request);
        WebResponse Post(WebRequest webRequest, Func<byte[], int, object> callBackWrite);

        WebResponse Post(WebUploadRequest request);

    }
}
