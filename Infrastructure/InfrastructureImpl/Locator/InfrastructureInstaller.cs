using Archiv10.Infrastructure.Impl.BO;
using Archiv10.Infrastructure.Shared;
using Archiv10.Infrastructure.Shared.BO;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archiv10.Infrastructure.Impl.Locator
{
    public class InfrastructureInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //BO's
            container.Register(Component.For<IWebRequest>().ImplementedBy<WebRequest>().LifestyleTransient());
            container.Register(Component.For<IWebResponse>().ImplementedBy<WebResponse>().LifestyleTransient());

            container.Register(Component.For<IWebConnector>().ImplementedBy<WebConnector>().LifestyleSingleton());
        }
    }
}
