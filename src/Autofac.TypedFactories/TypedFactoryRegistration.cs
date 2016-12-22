using System;

using Autofac.TypedFactories.Contracts;

using Castle.DynamicProxy;

namespace Autofac.TypedFactories
{
    using System.Linq;
    using System.Reflection;

    internal class TypedFactoryRegistration<TFactory> : TypedFactoryRegistration
        where TFactory : class
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedFactoryRegistration"/> class.
        /// </summary>
        /// <param name="containerBuilder">
        ///     The target Unity container on which to perform the registrations.
        /// </param>
        /// <param name="name">
        ///     Name that will be used to request the type.
        /// </param>
        public TypedFactoryRegistration(ContainerBuilder containerBuilder,
                                        string name = null)
            : base(containerBuilder, typeof(TFactory), name)
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
            //TODO: Add a "ForRegisteredType" to avoid registering the same type twice ?
            this.ContainerBuilder.RegisterType<TTo>();

            Func<IComponentContext, TFactory> injectionFactory = context =>
                {
                    var lifetimeScope = context.Resolve<ILifetimeScope>();
                    return ProxyGenerator.CreateInterfaceProxyWithoutTarget<TFactory>(new GenericFactoryInterceptor<TTo>(context,lifetimeScope, this.Name));
                };

            this.ContainerBuilder.Register(injectionFactory);

            if (this.Name != null)
            {
                throw new NotImplementedException("passing a Name is not supported by autofac... or is it ?");
            }
        }

                /// <summary>
        /// Defines the factory already returns concrete types.
        /// </summary>
        public override void ReturningConcreteType()
        {
            var factoryContractType = typeof(TFactory);
            var returnedTypes = factoryContractType.GetMethods(BindingFlags.Public | BindingFlags.Instance).Select(info => info.ReturnType).Distinct().ToArray();
            var factoryTypeName = factoryContractType.Name;
            if (returnedTypes.Length > 1)
            {
                // This factory Creates more than one type.
                throw new NotSupportedException($"The {factoryTypeName} factory returns {returnedTypes.Length} different types. Only factories returning one single type are supported.");
            }

            var toType = returnedTypes.SingleOrDefault();

            if (toType == null)
            {
                throw new InvalidOperationException($"No suitable factory method was found in interface {factoryTypeName}");
            }

            if (toType.IsInterface)
            {
                throw new InvalidOperationException($"The factory does not return concrete types. Please use the {nameof(ForConcreteType)} method instead for factories that have interfaces as return types.");
            }

            // TODO: Add a "ForRegisteredType" to avoid registering the same type twice ?
            this.ContainerBuilder.RegisterType(toType);

            this.ContainerBuilder.Register(
                (context, parameters) =>
                {
                    var lifetimeScope = context.Resolve<ILifetimeScope>();
                    return ProxyGenerator.CreateInterfaceProxyWithoutTarget<TFactory>(new FactoryInterceptor(lifetimeScope, toType, this.Name));
                });
        }

        #endregion
    }

    /// <summary>
    /// Implements the fluent interface for registering typed factories.
    /// </summary>
    internal class TypedFactoryRegistration : ITypedFactoryRegistration
    {
        #region Static Fields

        /// <summary>
        /// The Castle proxy generator.
        /// </summary>
        private static readonly Lazy<ProxyGenerator> LazyProxyGenerator = new Lazy<ProxyGenerator>();

        #endregion

        #region Fields

        /// <summary>
        ///     The factory interface.
        /// </summary>
        private readonly Type factoryContractType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedFactoryRegistration"/> class.
        /// </summary>
        /// <param name="containerBuilder">
        ///     The target Unity container on which to perform the registrations.
        /// </param>
        /// <param name="factoryContractType">
        ///     The factory interface.
        /// </param>
        /// <param name="name">
        ///     Name that will be used to request the type.
        /// </param>
        public TypedFactoryRegistration(
            ContainerBuilder containerBuilder,
            Type factoryContractType,
            string name = null)
        {
            this.factoryContractType = factoryContractType;
            this.ContainerBuilder = containerBuilder;
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the target Unity container on which to perform the registrations.
        /// </summary>
        public ContainerBuilder ContainerBuilder { get; private set; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Castle proxy generator. A new instance will be created upon the first access, and reused afterwards.
        /// </summary>
        protected static ProxyGenerator ProxyGenerator
        {
            get
            {
                return LazyProxyGenerator.Value;
            }
        }

        //TODO: can we get rid of this ? does Autofac support named  registrations ?
        /// <summary>
        /// Gets the name that will be used to request the type.
        /// </summary>
        protected string Name { get; private set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <param name="toType">
        /// The concrete type which the factory will instantiate.
        /// </param>
        public void ForConcreteType(Type toType)
        {
            //TODO: Add a "ForRegisteredType" to avoid registering the same type twice ?
            this.ContainerBuilder.RegisterType(toType);

            // throw new NotImplementedException("Registering factories by providing the type as a parameter is not yet supported. Please provide the type of the factory as a TypeArgument");
            this.ContainerBuilder.Register(
                (context, parameters) =>
                    {
                        var lifetimeScope = context.Resolve<ILifetimeScope>();
                        return ProxyGenerator.CreateInterfaceProxyWithoutTarget(this.factoryContractType, new FactoryInterceptor(lifetimeScope, toType, this.Name));
                    })
                    .As(this.factoryContractType);

        }

        /// <summary>
        /// Defines the concrete type which the factory will create.
        /// </summary>
        /// <typeparam name="TTo">
        /// The concrete type which the factory will instantiate.
        /// </typeparam>
        public virtual void ForConcreteType<TTo>()
        {
            this.ForConcreteType(typeof(TTo));
        }

        public virtual void ReturningConcreteType()
        {
            var factoryContractType = this.factoryContractType;
            this.ReturningConcreteType(factoryContractType);
        }

        protected void ReturningConcreteType(Type factoryContractType)
        {
            // throw new NotImplementedException("Registering factories by providing the type as a parameter is not yet supported. Please provide the type of the factory as a TypeArgument");
            var returnedTypes =
                factoryContractType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Select(info => info.ReturnType)
                    .Distinct()
                    .ToArray();
            var factoryTypeName = factoryContractType.Name;
            if (returnedTypes.Length > 1)
            {
                // This factory Creates more than one type.
                throw new NotSupportedException(
                          $"The {factoryTypeName} factory returns {returnedTypes.Length} different types. Only factories returning one single type are supported.");
            }

            var toType = returnedTypes.SingleOrDefault();

            if (toType == null)
            {
                throw new InvalidOperationException($"No suitable factory method was found in interface {factoryTypeName}");
            }

            if (toType.IsInterface)
            {
                throw new InvalidOperationException(
                          $"The factory does not return concrete types. Please use the {nameof(ForConcreteType)} method instead for factories that have interfaces as return types.");
            }

            // TODO: Add a "ForRegisteredType" to avoid registering the same type twice ?
            this.ContainerBuilder.RegisterType(toType);

            this.ContainerBuilder
                .Register(
                (context, parameters) =>
                    {
                        var lifetimeScope = context.Resolve<ILifetimeScope>();
                        return ProxyGenerator.CreateInterfaceProxyWithoutTarget(
                            factoryContractType,
                            new FactoryInterceptor(lifetimeScope, toType, this.Name));
                    })
                .As(factoryContractType);
        }

        #endregion
    }
}