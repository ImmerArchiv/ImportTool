using Archiv10.Domain.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared.BO
{
    public class UIRepository
    {
        public bool Active { get; set;  }
        public Repository Source { get; set; }
        public RepositoryState State { get; set; }
        public string EndpointState { get; set; }
        public UICanvasIcon Icon { get; set; }
    }
}
