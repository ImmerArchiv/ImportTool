using Archiv10.Locator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Infrastructure.Shared.BO;

namespace Archiv10.Infrastructure.Shared.Locator
{
    public class InfrastructureLocator
    {
        public static IWebConnector GetWebConnector()
        {
            return ContainerHolder.Resolve<IWebConnector>();
        }

        public static IFileService GetFileService()
        {
            return ContainerHolder.Resolve<IFileService>();
        }
    }
}
