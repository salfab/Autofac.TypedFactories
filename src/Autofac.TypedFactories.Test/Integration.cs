using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Autofac.TypedFactories.Test
{
    using System;

    using Autofac.TypedFactories.Exceptions;
    using Autofac.TypedFactories.Test.TestDomain;

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
            containerBuilder.RegisterTypedFactory<IDependencyServiceFactory>().ForConcreteType<DependencyService>();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IDependencyServiceFactory>();
            Assert.IsNotNull(dependencyServiceFactory);
            var createdInstance = dependencyServiceFactory.Create();
            Assert.IsNotNull(createdInstance);
            var createdInstance2 = dependencyServiceFactory.Create();
            Assert.AreNotSame(createdInstance, createdInstance2);
        }

        [TestMethod]
        public void ParameteredFactory()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterTypedFactory<IParameteredServiceFactory>().ForConcreteType<ParameteredService>();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IParameteredServiceFactory>();
            Assert.IsNotNull(dependencyServiceFactory);

            var createdInstance = dependencyServiceFactory.Create(1);
            Assert.IsNotNull(createdInstance);
            Assert.AreEqual(1, createdInstance.Number);

            var createdInstance2 = dependencyServiceFactory.Create(2);
            Assert.IsNotNull(createdInstance2);
            Assert.AreEqual(1, createdInstance.Number);
            Assert.AreEqual(2, createdInstance2.Number);

            Assert.AreNotSame(createdInstance, createdInstance2);
        }

        [TestMethod]
        public void ParameteredFactoryForObjectsWithDependencies()
        {
            var containerBuilder = new ContainerBuilder();

            // normal dependency, unrelated to factories
            containerBuilder.RegisterType<DependencyService>().As<IDependencyService>();

            containerBuilder.RegisterTypedFactory<IParameteredServiceWithDependencyFactory>().ForConcreteType<ParameteredServiceWithDependency>();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IParameteredServiceWithDependencyFactory>();
            Assert.IsNotNull(dependencyServiceFactory);

            var createdInstance = dependencyServiceFactory.Create(1);
            Assert.IsNotNull(createdInstance);
            Assert.AreEqual(1, createdInstance.Number);

            var createdInstance2 = dependencyServiceFactory.Create(2);
            Assert.IsNotNull(createdInstance2);
            Assert.AreEqual(2, createdInstance2.Number);

            Assert.AreNotSame(createdInstance, createdInstance2);
        }

        [TestMethod]
        public void ParameterlessFactoryReturningConcreteType()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterTypedFactory<IConcreteDependencyServiceFactory>().ReturningConcreteType();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IConcreteDependencyServiceFactory>();
            Assert.IsNotNull(dependencyServiceFactory);
            var createdInstance = dependencyServiceFactory.Create();
            Assert.IsNotNull(createdInstance);
            var createdInstance2 = dependencyServiceFactory.Create();
            Assert.AreNotSame(createdInstance, createdInstance2);
        }

        [TestMethod]
        public void NonGenericRegistration_ParameterlessFactoryReturningConcreteType()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterTypedFactory(typeof(IConcreteDependencyServiceFactory)).ReturningConcreteType();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IConcreteDependencyServiceFactory>();
            Assert.IsNotNull(dependencyServiceFactory);
            var createdInstance = dependencyServiceFactory.Create();
            Assert.IsNotNull(createdInstance);
            var createdInstance2 = dependencyServiceFactory.Create();
            Assert.AreNotSame(createdInstance, createdInstance2);
        }

        [TestMethod]
        public void NonGenericRegistration_ParameterlessFactory()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterTypedFactory(typeof(IDependencyServiceFactory)).ForConcreteType<DependencyService>();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IDependencyServiceFactory>();
            Assert.IsNotNull(dependencyServiceFactory);
            var createdInstance = dependencyServiceFactory.Create();
            Assert.IsNotNull(createdInstance);
            var createdInstance2 = dependencyServiceFactory.Create();
            Assert.AreNotSame(createdInstance, createdInstance2);
        }

        [TestMethod]
        public void ParameterlessMultipleFactories()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<DependencyService>().As<IDependencyService>();
            containerBuilder.RegisterTypedFactory<IParameteredServiceWithDependencyFactory>().ForConcreteType<ParameteredServiceWithDependency>();
            containerBuilder.RegisterTypedFactory<IOtherParameteredServiceWithDependencyFactory>().ForConcreteType<ParameteredServiceWithDependency>();
            var container = containerBuilder.Build();

            // factory 1
            var dependencyServiceFactory = container.Resolve<IParameteredServiceWithDependencyFactory>();
            Assert.IsNotNull(dependencyServiceFactory);
            var createdInstance = dependencyServiceFactory.Create(1);
            Assert.IsNotNull(createdInstance);

            // factory 2
            var dependencyServiceFactory2 = container.Resolve<IOtherParameteredServiceWithDependencyFactory>();
            Assert.IsNotNull(dependencyServiceFactory2);
            var specifiedDependencyService = new DependencyService();
            var createdOtherInstance = dependencyServiceFactory2.Create(2, specifiedDependencyService);
            Assert.IsNotNull(createdOtherInstance);

            Assert.AreSame(specifiedDependencyService, createdOtherInstance.Dependency);

        }

        [TestMethod]
        [ExpectedException(typeof(FactorySignatureMismatchException))]
        public void DetectMisalignedFactorySignatures()
        {
            var containerBuilder = new ContainerBuilder();

            // normal dependency, unrelated to factories
            containerBuilder.RegisterType<DependencyService>().As<IDependencyService>();

            containerBuilder.RegisterTypedFactory<IParameteredServiceFactory>().ForConcreteType<MisalignedParameteredService>();
            var container = containerBuilder.Build();
            var dependencyServiceFactory = container.Resolve<IParameteredServiceFactory>();
            Assert.IsNotNull(dependencyServiceFactory);

            var createdInstance = dependencyServiceFactory.Create(1);
            Assert.IsNotNull(createdInstance);
            Assert.AreEqual(1, createdInstance.Number);
        }
    }
}
