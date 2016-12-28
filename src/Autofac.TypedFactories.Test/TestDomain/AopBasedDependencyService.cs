namespace Autofac.TypedFactories.Test.TestDomain
{
    [InstantiateWithDynamicFactory(typeof(IDependencyServiceFactory))]
    public class AopBasedDependencyService
        : IDependencyService
    {
    }
}