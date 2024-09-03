using Archiv10.Domain.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared.BO
{
    public class UIData 
    {
        public UIData()
        {
            SourceRepositories = new List<Repository>();
        }

        public BagData SourceData { get; set; }
        public BagId SourceBag { get; set; }
        public IList<Repository> SourceRepositories { get; set; }
        public UIState State { get; set; }
        public string SearchString { get; set; }



    }
}
