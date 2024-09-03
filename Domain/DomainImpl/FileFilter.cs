using Archiv10.Infrastructure.Shared.BO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Archiv10.Domain.Impl
{
    class FileFilter : IFileFilter
    {
        private readonly Regex _filter;

        public FileFilter(string[] filter)
        {

            var pattern = "";
            for (var i = 0; i < filter.Length; i++)
            {
                if (i > 0) pattern += "|";
                pattern += WildCardToRegular(filter[i]); 
            }
            _filter = new Regex(pattern, RegexOptions.IgnoreCase);

        }
        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }


        public bool Match(string file)
        {
            var fileName = Path.GetFileName(file);
            return _filter.Match(fileName).Success;
           
        }
    }
}
