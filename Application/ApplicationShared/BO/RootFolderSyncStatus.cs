using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Shared.BO
{
    public class RootFolderSyncStatus
    {
        public const string StateInit         = "Init";
        public const string StateSynchronized = "Synchronized";
        public const string StateUploading    = "Uploading";
        public const string StateError        = "Error";
        public string state { get; set; }

        //id
        public string path { get; set; }


        //Details
        public long jobCnt { get; set; }

    }
}
