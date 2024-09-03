using Archiv10.Domain.Shared.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Impl.Services
{
    class FilenameService : IFilenameService
    {
        private static string allowed = "_-.";

        public string CleanName(string name)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in name)
            {
                if (('a' <= c && c <= 'z')
                    || ('A' <= c && c <= 'Z')
                    || ('0' <= c && c <= '9')
                    || allowed.Contains(c))
                    builder.Append(c);
                else
                    builder.Append("_");
            }
            return builder.ToString();
        }

        public string GetTemporaryName(DateTime dateTime, int random, string extension)
        {
            return string.Format("{0:yyyyMMdd_HHmmss}.{1}", dateTime,extension);
        }

        public string SaltedFileName(string originFileName, string originCheckSum)
        {
            String ext = "";
            String name = originFileName;
            int ix = originFileName.IndexOf(".");
            if(ix >= 0)
            {
                name = originFileName.Substring(0, ix);
                ext = originFileName.Substring(ix + 1);
            }
       
            String salt = originCheckSum.Substring(0, 4);
            return string.Format("{0}{1}{2}{3}{4}", name, name.Length > 0?"_" :"", salt, ext.Length > 0?".":"" ,ext);
        }
    }
}
