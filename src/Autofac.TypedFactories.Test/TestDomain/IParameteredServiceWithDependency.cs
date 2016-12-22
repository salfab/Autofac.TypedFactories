namespace Autofac.TypedFactories.Test
{
    public interface IParameteredServiceWithDependency
    {
        int Number { get; set; }

        IDependencyService Dependency { get; set; }
    }
}