using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public interface IRepositoryEndpointInfo
    {

        string ApiVersion { get; set; }
        string GrantType { get; set; }
        string CheckSum { get; set; }
        string UrlCreate { get; set; }
        string UrlAppendFile { get; set; }
        string UrlListAll { get; set; }
        string UrlListOne { get; set; }
        string UrlGetFile { get; set; }

    }
}
