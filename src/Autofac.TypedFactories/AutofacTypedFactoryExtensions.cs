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

        /// <summary>
        /// Registers dynamic factories for the specified, according to their <see cref="InstantiateWithDynamicFactoryAttribute"/>.
        /// </summary>
        /// <param name="containerBuilder">The autofac <see cref="ContainerBuilder"/>.</param>
        /// <param name="types">The types to register a factory for.</param>
        /// <exception cref="ArgumentException">When the specidief <paramref name="types"/> contains types that were not decorated with the <see cref="InstantiateWithDynamicFactoryAttribute"/></exception>
        /// <returns>The specified <paramref name="containerBuilder"/>, with the factories being registered.</returns>
        public static TypedFactoryRegistrationByTypes RegisterTypedFactoriesFor(this ContainerBuilder containerBuilder, Type[] types)
        {
            return new TypedFactoryRegistrationByTypes(containerBuilder, types);

        }



        /// <param name="containerbuilder">the autofac <see cref="ContainerBuilder"/>.</param>
        /// <param name="assemblyContainingTypes">The <see cref="Assembly"/> containing the types to register.</param>
        public static TypedFactoryRegistrationByAssembly RegisterTypedFactoriesFor(this ContainerBuilder containerbuilder, params Assembly[] assemblyContainingTypes)
        {
            return new TypedFactoryRegistrationByAssembly(containerbuilder, assemblyContainingTypes);
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

    public class TypedFactoryRegistrationByAssembly : TypedFactoryRegistrationBase
    {


        public TypedFactoryRegistrationByAssembly(ContainerBuilder containerBuilder, Assembly[] assemblies)
            : base(containerBuilder, assemblies.SelectMany(assembly => assembly.GetTypes().Where(type => type.IsDefined(typeof(InstantiateWithDynamicFactoryAttribute), false))).ToArray())
        {
        }

        //private ContainerBuilder Register()
        //{
        //    foreach (var assembly in assemblies)
        //    {
        //        var typesMarkedByAttribute = assembly.GetTypes().Where(type => type.IsDefined(typeof(InstantiateWithDynamicFactoryAttribute), false)).ToArray();
        //        InternalContainerBuilder(this.containerBuilder, typesMarkedByAttribute);
        //    }
        //    return this.containerBuilder;
        //}

    }

    public class TypedFactoryRegistrationByTypes : TypedFactoryRegistrationBase
    {


        public TypedFactoryRegistrationByTypes(ContainerBuilder containerBuilder, Type[] types) 
            : base(containerBuilder, types)
        {


        }
    }

    public abstract class TypedFactoryRegistrationBase
    {
        protected ContainerBuilder ContainerBuilder { get; }
        protected Type[] Types { get; private set; }

        protected TypedFactoryRegistrationBase(ContainerBuilder containerBuilder, Type[] types)
        {
            this.Types = types;
            this.ContainerBuilder = containerBuilder;
        }



        /// <remarks>All types are expected to be decorated with the <see cref="InstantiateWithDynamicFactoryAttribute"/>.</remarks>
        protected ContainerBuilder InternalContainerBuilder(ContainerBuilder containerBuilder, Type[] types)
        {
            foreach (var type in types)
            {
                var factoryType = type.GetAttribute<InstantiateWithDynamicFactoryAttribute>().FactoryType;
                containerBuilder.RegisterTypedFactory(factoryType).ForConcreteType(type);
            }

            return containerBuilder;
        }

        public void Except(params Type[] types)
        {
            this.Types = this.Types.Except(types).ToArray();
            this.Register();
        }

        public void UsingAop()
        {
            this.Register();
        }

        protected ContainerBuilder Register()
        {
            var invalidTypes = this.Types.Where(type => !type.IsDefined(typeof(InstantiateWithDynamicFactoryAttribute), false)).ToArray();

            if (invalidTypes.Any())
            {
                var printFriendlyInvalidTypesListing = invalidTypes
                    .Select(type => type.Name)
                    .Aggregate((typeName1, typeName2) => $"{typeName1}, {typeName2}");


                // change exception
                throw new InvalidOperationException(
                    $"The specified types contained a type not decorated with the {nameof(InstantiateWithDynamicFactoryAttribute)} attribute.\nInvalid types: {printFriendlyInvalidTypesListing}");
            }

            return InternalContainerBuilder(this.ContainerBuilder, this.Types);
        }
    }
}
