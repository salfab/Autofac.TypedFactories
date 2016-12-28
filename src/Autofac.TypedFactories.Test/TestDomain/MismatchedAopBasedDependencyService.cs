namespace Autofac.TypedFactories.Test.TestDomain
{
    [InstantiateWithDynamicFactory(typeof(IDependencyServiceFactory))]
    public class MismatchedAopBasedParameteredService
        : IParameteredService
    {
        public int Number { get; set; }
    }
}