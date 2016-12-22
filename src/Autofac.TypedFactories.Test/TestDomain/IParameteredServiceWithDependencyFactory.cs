namespace Autofac.TypedFactories.Test
{
    public interface IParameteredServiceWithDependencyFactory
    {
        IParameteredServiceWithDependency Create(int number);
    }
}