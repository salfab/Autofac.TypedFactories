using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autofac.TypedFactories
{
    using Autofac.TypedFactories.Contracts;

    /// <summary>
    /// Defines extension methods for providing custom typed factories based on a factory interface.
    /// </summary>
    public static class AutofacTypedFactoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <param name="factoryContractType">
        /// The factory interface.
        /// </param>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <returns>
        /// The holder object which facilitates the fluent interface.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="factoryContractType"/> does not represent an interface type.
        /// </exception>
        public static ITypedFactoryRegistration RegisterTypedFactory(this ContainerBuilder containerBuilder,
                                                                     Type factoryContractType)
        {
            if (!factoryContractType.IsInterface)
            {
                throw new ArgumentException("The factory contract does not represent an interface!", "factoryContractType");
            }

            var typedFactoryRegistration = new TypedFactoryRegistration(containerBuilder, factoryContractType);
            return typedFactoryRegistration;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <typeparam name="TFactory">
        /// The factory interface.
        /// </typeparam>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <returns>
        /// The holder object which facilitates the fluent interface.
        /// </returns>
        public static ITypedFactoryRegistration RegisterTypedFactory<TFactory>(this ContainerBuilder containerBuilder)
            where TFactory : class
        {
            if (!typeof(TFactory).IsInterface)
            {
                throw new ArgumentException("The factory contract does not represent an interface!");
            }

            var typedFactoryRegistration = new TypedFactoryRegistration<TFactory>(containerBuilder);
            return typedFactoryRegistration;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <typeparam name="TFactory">
        /// The factory interface.
        /// </typeparam>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <param name="name">
        /// Name that will be used to request the type.
        /// </param>
        /// <returns>
        /// The holder object which facilitates the fluent interface.
        /// </returns>
        public static ITypedFactoryRegistration RegisterTypedFactory<TFactory>(this ContainerBuilder containerBuilder,
                                                                               string name)
            where TFactory : class
        {
            return containerBuilder.RegisterTypedFactory(typeof(TFactory), name);
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <param name="factoryContractType">
        /// The factory interface.
        /// </param>
        /// <param name="name">
        /// Name that will be used to request the type.
        /// </param>
        /// <returns>
        /// The holder object which facilitates the fluent interface.
        /// </returns>
        public static ITypedFactoryRegistration RegisterTypedFactory(this ContainerBuilder containerBuilder,
                                                                     Type factoryContractType,
                                                                     string name)
        {
            var typedFactoryRegistration = new TypedFactoryRegistration(containerBuilder, factoryContractType, name);
            return typedFactoryRegistration;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <param name="factoryContractType">
        /// The factory interface.
        /// </param>
        /// <param name="toType">
        /// The concrete type that the factory will instantiate.
        /// </param>
        /// <returns>
        /// The Unity container to continue its fluent interface.
        /// </returns>
        public static ContainerBuilder RegisterTypedFactory(this ContainerBuilder containerBuilder,
                                                                     Type factoryContractType,
                                                                     Type toType)
        {
            containerBuilder.RegisterTypedFactory(factoryContractType).ForConcreteType(toType);
            return containerBuilder;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <typeparam name="TFactory">
        /// The factory interface.
        /// </typeparam>
        /// <typeparam name="TConcreteType">
        /// The concrete type that the factory will instantiate.
        /// </typeparam>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <returns>
        /// The Unity container to continue its fluent interface.
        /// </returns>
        public static ContainerBuilder RegisterTypedFactory<TFactory, TConcreteType>(this ContainerBuilder containerBuilder)
            where TFactory : class
        {
            return containerBuilder.RegisterTypedFactory(typeof(TFactory), typeof(TConcreteType));
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <param name="factoryContractType">
        /// The factory interface.
        /// </param>
        /// <param name="toType">
        /// The concrete type that the factory will instantiate.
        /// </param>
        /// <param name="name">
        /// Name that will be used to request the type.
        /// </param>
        /// <returns>
        /// The Unity container to continue its fluent interface.
        /// </returns>
        public static ContainerBuilder RegisterTypedFactory(this ContainerBuilder containerBuilder,
                                                                     Type factoryContractType,
                                                                     Type toType,
                                                                     string name)
        {
            containerBuilder.RegisterTypedFactory(factoryContractType, name).ForConcreteType(toType);
            return containerBuilder;
        }

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <typeparam name="TFactory">
        /// The factory interface.
        /// </typeparam>
        /// <typeparam name="TConcreteType">
        /// The concrete type that the factory will instantiate.
        /// </typeparam>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <param name="name">
        /// Name that will be used to request the type.
        /// </param>
        /// <returns>
        /// The Unity container to continue its fluent interface.
        /// </returns>
        public static ContainerBuilder RegisterTypedFactory<TFactory, TConcreteType>(this ContainerBuilder containerBuilder,
                                                                              string name)
            where TFactory : class
        {
            return containerBuilder.RegisterTypedFactory(typeof(TFactory), typeof(TConcreteType), name);
        }

        #endregion
    }

    internal class TypedFactoryRegistration<TFactory> : TypedFactoryRegistration
      where TFactory : class
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedFactoryRegistration"/> class.
        /// </summary>
        /// <param name="container">
        ///     The target Unity container on which to perform the registrations.
        /// </param>
        /// <param name="name">
        ///     Name that will be used to request the type.
        /// </param>
        public TypedFactoryRegistration(ContainerBuilder container,
                                        string name = null)
            : base(container, typeof(TFactory), name)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <typeparam name="TTo">
        /// The concrete type which the factory will instantiate.
        /// </typeparam>
        public override void ForConcreteType<TTo>()
        {
            //var injectionFactory = new InjectionFactory(container => ProxyGenerator.CreateInterfaceProxyWithoutTarget<TFactory>(new GenericFactoryInterceptor<TTo>(container, this.Name)));

            Func<IComponentContext, TFactory> injectionFactory = context => ProxyGenerator.CreateInterfaceProxyWithoutTarget<TFactory>(new GenericFactoryInterceptor<TTo>(context, this.Name));
            this.Container.Register(injectionFactory);

            if (this.Name != null)
            {
                throw new NotImplementedException("passing a Name is not supported by autofac... or is it ?");
            }
        }

        #endregion
    }
}