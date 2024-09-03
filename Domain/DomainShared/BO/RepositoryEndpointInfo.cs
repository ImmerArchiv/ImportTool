using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class RepositoryEndpointInfo
    {
        public RepositoryEndpointInfo()
        {
            UrlInfo = "info.json";
        }

        public string ApiVersion { get; set; }
        public string GrantType { get; set; }
        public string CheckSum { get; set; }

        public string UrlInfo { get; set; }
        public string UrlCreate { get; set; }
        public string UrlStatus { get; set; }
        public string UrlPutFilePart { get; set; }
        public string UrlAppendFile { get; set; }
        public string UrlListAll { get; set; }
        public string UrlListOne { get; set; }
        public string UrlGetFilePart { get; set; }
        public string UrlDownloadFile { get; set; }
    }
}
