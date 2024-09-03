using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Archiv10.Domain.Impl.Services;
using Archiv10.Domain.Shared.Services;
using Archiv10.Domain.Shared;
using Archiv10.Infrastructure.Shared.BO;

namespace Archiv10.Domain.Impl.Locator
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IRepositoryService>().ImplementedBy<RepositoryService>().LifestyleSingleton());
            container.Register(Component.For<IRootFolderService>().ImplementedBy<RootFolderService>().LifestyleSingleton());
            container.Register(Component.For<IFilenameService>().ImplementedBy<FilenameService>().LifestyleSingleton());
            container.Register(Component.For<INameService>().ImplementedBy<NameService>().LifestyleSingleton());

            container.Register(Component.For<IRepositoryConfig>().ImplementedBy<RepositoryConfig>().LifestyleSingleton());
            container.Register(Component.For<ICheckSumService>().ImplementedBy<Md5SumService>().Named("md5").LifestyleSingleton());

            container.Register(Component.For<ICheckSumCache>().ImplementedBy<CheckSumCache>().LifestyleSingleton());
            container.Register(Component.For<IBagitCacheService>().ImplementedBy<BagitCacheService>().LifestyleSingleton());


            container.Register(Component.For<IFileFilter>().ImplementedBy<FileFilter>().LifestyleTransient());

            container.Register(Component.For<IFileMappingValidatorService>().ImplementedBy<FileMappingValidatorService>().LifestyleTransient()); //reread mapping

            

        }
    }
}
