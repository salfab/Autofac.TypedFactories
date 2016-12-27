namespace Autofac.TypedFactories.Test.TestDomain
{
    public class DependencyService
        : IDependencyService
    {
    }


    [InstantiateWithDynamicFactory(typeof(IDependencyServiceFactory))]
    public class AopBasedDependencyService
        : IDependencyService
    {
    }
}