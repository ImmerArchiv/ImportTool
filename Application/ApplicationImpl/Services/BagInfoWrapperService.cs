using Archiv10.Application.Shared.Services;
using Archiv10.Domain.Shared.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Impl.Services
{
    class BagInfoWrapperService : IBagInfoWrapperService
    {
        private BagSnippet _snippet;

        public BagInfoWrapperService(BagSnippet snippet)
        {
            _snippet = snippet;
        }

        public string GetDescription()
        {
            var description = "";
            foreach (var kvp in _snippet.Info)
            {
                if (kvp.Key == "Description") description += String.Format("{0}", kvp.Value);

            }

            if (string.IsNullOrWhiteSpace(description))
                description = _snippet.Id.ToString();

            return description;
        }
    }
}
