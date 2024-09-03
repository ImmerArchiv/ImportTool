using Archiv10.Domain.Shared.Services;
using Archiv10.Infrastructure.Shared.BO;
using Archiv10.Locator.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Domain.Shared.Locator
{
    public class DomainLocator
    {
        public static IRepositoryService GetRepositoryService()
        {
            return ContainerHolder.Resolve<IRepositoryService>();
        }

        public static IFilenameService GetFileNameService()
        {
            return ContainerHolder.Resolve<IFilenameService>();
        }

        public static ICheckSumService GetCheckSumService(string name)
        {
            return ContainerHolder.Resolve<ICheckSumService>(name);
        }

        public static IRootFolderService GetRootFolderService()
        {
            return ContainerHolder.Resolve<IRootFolderService>();
        }

        public static INameService GetNameService()
        {
            return ContainerHolder.Resolve<INameService>();
        }

        public static IFileFilter GetFileFilter(string[] filter)
        {
            return ContainerHolder.Resolve<IFileFilter>( new { filter = filter });
        }

        public static ICheckSumCache GetCheckSumCache()
        {
            return ContainerHolder.Resolve<ICheckSumCache>();
        }

        public static IFileMappingValidatorService GetFileMappingValidatorService()
        {
            return ContainerHolder.Resolve<IFileMappingValidatorService>();
        }
    }
}
