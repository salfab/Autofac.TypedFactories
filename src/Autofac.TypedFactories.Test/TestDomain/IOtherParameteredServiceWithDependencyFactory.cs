namespace Autofac.TypedFactories.Test.TestDomain
{
    public interface IOtherParameteredServiceWithDependencyFactory
    {
        IParameteredServiceWithDependency Create(int number, IDependencyService dependency);
    }
}