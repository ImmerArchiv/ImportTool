using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.Services
{
    public interface IFilenameService
    {
        string CleanName(string name);
        string GetTemporaryName(DateTime dateTime, int random, string extension);
        string SaltedFileName(string originFileName, string originCheckSum);
    }
}
