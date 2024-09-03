using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public enum LocalFileState
    {
        EmptyFile = 0,
        WrongCheckSum,
        NotSynchronized,
        MustUpload,
        Ok
    }
}
