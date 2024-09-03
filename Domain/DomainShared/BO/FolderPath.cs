using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.BO
{
    public class FolderPath
    {
        private string _path;

        public FolderPath(String path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("path");

            _path = path;
        }
        public override string ToString()
        {
            return _path;
        }
    }
}
