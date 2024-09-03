using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Shared.BO
{
    public class WebUploadRequest
    {
        public string AccessToken { get; set; }
        public IDictionary<String,String> Data { get; set; }
        public byte[] FileData { get; set; }
        public string Url { get; set; }
    }
}
