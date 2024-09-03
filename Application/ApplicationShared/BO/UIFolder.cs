using Archiv10.Domain.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared.BO
{
    public class UIFolder
    {
        public RootFolder RootSourceFolder { get; set; }
        public FolderPath SourceFolder { get; set; }
    }
}
