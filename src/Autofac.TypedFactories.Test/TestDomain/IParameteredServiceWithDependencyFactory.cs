namespace Autofac.TypedFactories.Test.TestDomain
{
    public interface IParameteredServiceWithDependencyFactory
    {
        IParameteredServiceWithDependency Create(int number);
    }

    public class ParameteredServiceWithDependencyFactory : IParameteredServiceWithDependencyFactory
    {
        private readonly IDependencyService dependencyService;

        public ParameteredServiceWithDependencyFactory(IDependencyService dependencyService)
        {
            this.dependencyService = dependencyService;
        }
        public IParameteredServiceWithDependency Create(int number)
        {
            return new ParameteredServiceWithDependency(number, dependencyService);
        }
    }
}