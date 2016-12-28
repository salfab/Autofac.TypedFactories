using System;
using Autofac.TypedFactories.Contracts;

namespace Autofac.TypedFactories
{
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Castle.Core.Internal;

    /// <summary>
    /// Defines extension methods for providing custom typed factories based on a factory interface.
    /// </summary>
    public static class AutofacTypedFactoryExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// Registers a typed factory.
        /// </summary>
        /// <param name="containerBuilder">
        /// The Unity container.
        /// </param>
        /// <param name="factoryContractType">
        /// The factory interface.
        /// </param>
        /// <returns>
        /// The holder object which facilitates the fluent interface.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="factoryContractType"/> does not represent an interface type.
        /// </exception>
        public static ITypedFactoryRegistration RegisterTypedFactory(this ContainerBuilder containerBuilder, Type factoryContractType)
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
        public static ITypedFactoryRegistration RegisterTypedFactory<TFactory>(this ContainerBuilder containerBuilder, string name)
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
        public static ITypedFactoryRegistration RegisterTypedFactory(this ContainerBuilder containerBuilder, Type factoryContractType, string name)
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
        public static ContainerBuilder RegisterTypedFactory(this ContainerBuilder containerBuilder, Type factoryContractType, Type toType)
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
        public static ContainerBuilder RegisterTypedFactory(this ContainerBuilder containerBuilder, Type factoryContractType, Type toType, string name)
        {
            containerBuilder.RegisterTypedFactory(factoryContractType, name).ForConcreteType(toType);
            return containerBuilder;
        }

        public static ContainerBuilder RegisterTypedFactoriesFor(this ContainerBuilder containerBuilder, Type[] types)
        {
            var invalidTypes = types.Where(type => !type.IsDefined(typeof(InstantiateWithDynamicFactoryAttribute), false)).ToArray();

            if (invalidTypes.Any())
            {
                var printFriendlyInvalidTypesListing = invalidTypes
                    .Select(type => type.Name)
                    .Aggregate((typeName1, typeName2) => $"{typeName1}, {typeName2}");

                throw new ArgumentException($"The specified types contained a type not decorated with the {nameof(InstantiateWithDynamicFactoryAttribute)} attribute.\nInvalid types: {printFriendlyInvalidTypesListing}", nameof(types));
            }

            foreach (var type in types)
            {
                var factoryType = type.GetAttribute<InstantiateWithDynamicFactoryAttribute>().FactoryType;
                containerBuilder.RegisterTypedFactory(factoryType).ForConcreteType(type);
            }

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
        public static ContainerBuilder RegisterTypedFactory<TFactory, TConcreteType>(this ContainerBuilder containerBuilder, string name)
            where TFactory : class
        {
            return containerBuilder.RegisterTypedFactory(typeof(TFactory), typeof(TConcreteType), name);
        }

        #endregion
    }
}