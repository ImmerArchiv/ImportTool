using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared.BO
{
    public class ServiceStatus
    {

        public bool Working { get; set; }

        public string State { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
