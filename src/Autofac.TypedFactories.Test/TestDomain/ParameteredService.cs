namespace Autofac.TypedFactories.Test.TestDomain
{
    public class ParameteredService : IParameteredService
    {
        public int Number { get; set; }

        public ParameteredService(int number)
        {
            this.Number = number;
        }
    }
}