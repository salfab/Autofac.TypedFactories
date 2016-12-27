namespace Autofac.TypedFactories.Test.TestDomain
{
    public interface IParameteredServiceWithDependencyFactory
    {
        IParameteredServiceWithDependency Create(int number);
    }
}