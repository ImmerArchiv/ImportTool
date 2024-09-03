using Archiv10.Domain.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared.BO
{
    public class UIBagSnippet
    {
        public UIBagSnippet()
        {
            SourceRepositories = new List<Repository>();
            Status = new Dictionary<string, BagStatus>();
        }
        public string Description { get; set; }
        public BagId SourceId { get; set; }
        public IList<Repository> SourceRepositories { get; set; }
        public UIState State { get; set; }
        public IDictionary<string,BagStatus> Status { get; set; }
        public string SearchString { get; set; }

    }
}
