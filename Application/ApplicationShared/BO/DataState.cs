using System.Collections.Generic;

namespace Archiv10.Application.Shared.Services
{
    public class FileMapping
    {
        public FileMapping()
        {
            LocalFileOrigins = new List<LocalFileOrigin>();
            BagItOrigins = new List<BagItOrigin>();
        }

        public IList<LocalFileOrigin> LocalFileOrigins { get; set; }
        public IList<BagItOrigin> BagItOrigins { get; set; }

    }
}