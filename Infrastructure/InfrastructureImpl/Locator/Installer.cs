using Archiv10.Infrastructure.Shared;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Archiv10.Infrastructure.Impl.Locator
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IWebConnector>().ImplementedBy<WebConnector>().LifestyleSingleton());
            container.Register(Component.For<IFileService>().ImplementedBy<FileService>().LifestyleSingleton());
        }
    }
}
