using Archiv10.Application.Impl.Services;
using Archiv10.Application.Shared;
using Archiv10.Application.Shared.Services;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Application.Impl.Locator
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IApplication>().ImplementedBy<Application>().LifestyleSingleton());

            //darf kein singleton sein
            container.Register(Component.For<IBagInfoWrapperService>().ImplementedBy<BagInfoWrapperService>().LifestyleTransient());

            //Services
            container.Register(Component.For<IFolderBagitMappingService>().ImplementedBy<FolderBagitMappingService>().LifestyleSingleton());
            container.Register(Component.For<IFileMappingService>().ImplementedBy<FileMappingService>().LifestyleSingleton());
            container.Register(Component.For<IJobCreatorService>().ImplementedBy<JobCreatorService>().LifestyleSingleton());

        }
    }
}
