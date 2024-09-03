using Archiv10.Infrastructure.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Impl.BO
{
    class WebResponse : IWebResponse
    {
        public string Data { get; set; }
        public int Status { get; set; }
    }
}
