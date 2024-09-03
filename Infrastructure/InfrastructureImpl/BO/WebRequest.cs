﻿using Archiv10.Infrastructure.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Impl.BO
{
    class WebRequest : IWebRequest
    {
        public string AccessToken { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }

    }
}
