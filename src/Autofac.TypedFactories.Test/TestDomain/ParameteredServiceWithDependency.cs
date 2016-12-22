namespace Autofac.TypedFactories.Test
{
    public class ParameteredServiceWithDependency : IParameteredServiceWithDependency
    {
        public int Number { get; set; }

        public IDependencyService Dependency { get; set; }

        public ParameteredServiceWithDependency(int number, IDependencyService dependency)
        {
            this.Number = number;
            this.Dependency = dependency;
        }
    }
}