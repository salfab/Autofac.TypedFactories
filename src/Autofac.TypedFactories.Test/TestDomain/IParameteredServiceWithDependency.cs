namespace Autofac.TypedFactories.Test.TestDomain
{
    public interface IParameteredServiceWithDependency
    {
        int Number { get; set; }

        IDependencyService Dependency { get; set; }
    }
}