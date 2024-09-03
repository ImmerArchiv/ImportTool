using Archiv10.Locator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archiv10.Domain.Shared.BO;
using Archiv10.Application.Shared.Services;

namespace Archiv10.Application.Shared.Locator
{
    public class ApplicationLocator
    {
        public static IApplication GetApplication()
        {
            return ContainerHolder.Resolve<IApplication>();
        }

     

        public static IBagInfoWrapperService GetBagInfoWrapperService(BagSnippet snippet)
        {
            return ContainerHolder.Resolve<IBagInfoWrapperService>(new { snippet = snippet });
        }

        public static IJobCreatorService GetJobCreatorService()
        {
            return ContainerHolder.Resolve<IJobCreatorService>();
        }

        public static IFileMappingService GetFileMappingService()
        {
            return ContainerHolder.Resolve<IFileMappingService>();
        }

        public static IFolderBagitMappingService GetFolderBagitMappingService()
        {
            return ContainerHolder.Resolve<IFolderBagitMappingService>();
        }
    }
}
