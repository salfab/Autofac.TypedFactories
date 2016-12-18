using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac.TypedFactories;

namespace Autofac.TypedFactories.Test
{
    using Autofac.Builder;

    [TestClass]
    public class Integration
    {
        [TestMethod]
        public void NoRegressionInTypicalScenario()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<DependencyService>().As<IDependencyService>();
            var container = containerBuilder.Build();
            var instance = container.Resolve<IDependencyService>();
            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void ParameterlessFactory()
        {
            var containerBuilder = new ContainerBuilder();
            //containerBuilder.RegisterType<DependencyService>().As<IDependencyService>();            
            containerBuilder.RegisterTypedFactory<IDependencyServiceFactory>().ForConcreteType<DependencyService>();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IDependencyServiceFactory>();
            Assert.IsNotNull(dependencyServiceFactory);
            var createdInstance = dependencyServiceFactory.Create();
            Assert.IsNotNull(createdInstance);
        }
    }
}
