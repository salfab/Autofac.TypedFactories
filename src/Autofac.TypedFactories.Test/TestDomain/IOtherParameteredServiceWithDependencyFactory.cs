namespace Autofac.TypedFactories.Test
{
    public interface IOtherParameteredServiceWithDependencyFactory
    {
        IParameteredServiceWithDependency Create(int number, IDependencyService dependency);
    }
}