using Archiv10.Domain.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared.BO
{
    public class UIRootFolder
    {
        public bool Active { get; set; }
        public RootFolder Source { get; set; }
        public RootFolderState State { get; set; }
        public UICanvasIcon Icon { get; set; }
    }
}
