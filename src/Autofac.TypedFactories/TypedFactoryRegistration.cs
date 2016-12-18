namespace Autofac.TypedFactories
{
    using System;

    using Autofac.TypedFactories.Contracts;

    using Castle.DynamicProxy;

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
        /// <param name="container">
        ///     The target Unity container on which to perform the registrations.
        /// </param>
        /// <param name="factoryContractType">
        ///     The factory interface.
        /// </param>
        /// <param name="name">
        ///     Name that will be used to request the type.
        /// </param>
        public TypedFactoryRegistration(
            ContainerBuilder container,
            Type factoryContractType,
            string name = null)
        {
            this.factoryContractType = factoryContractType;
            this.Container = container;
            this.Name = name;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the target Unity container on which to perform the registrations.
        /// </summary>
        public ContainerBuilder Container { get; private set; }

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
            //var injectionFactory = new InjectionFactory(container => ProxyGenerator.CreateInterfaceProxyWithoutTarget(this.factoryContractType, new FactoryInterceptor(container, toType, this.Name)));

            if (this.Name != null)
            {
                this.Container.Register((context, parameters) => ProxyGenerator.CreateInterfaceProxyWithoutTarget(this.factoryContractType, new FactoryInterceptor(context, toType, this.Name)));
                //this.Container.RegisterType(null, this.factoryContractType, this.Name, injectionFactory);
            }
            else
            {
                throw new NotImplementedException("ForCOncreteType does not support not providing a this.Name");
                // this.Container.RegisterType(null, this.factoryContractType, injectionFactory);
            }
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

        #endregion
    }
}