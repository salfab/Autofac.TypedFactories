namespace Autofac.TypedFactories.Test.TestDomain
{
    using System;

    public class InstantiateWithDynamicFactoryAttribute : Attribute
    {
        public Type FactoryType { get; set; }

        public InstantiateWithDynamicFactoryAttribute(Type factoryType)
        {
            this.FactoryType = factoryType;
        }
    }
}