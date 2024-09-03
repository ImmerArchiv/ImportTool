using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class Repository
    {
        public Repository()
        {
            Info = new RepositoryEndpointInfo();
            Status = new RepositoryEndpointStatus();
        }
        public string Url { get; set; }
        public RepositoryEndpointInfo Info { get; set; }
        public string RepositoryName { get; set; }
        public string Token { get; set; }
        public RepositoryEndpointStatus Status { get; set; }
        public string Key {
            get {
                return string.Format("{0}::{1}", Url, RepositoryName);
            }
        }
    }
}
