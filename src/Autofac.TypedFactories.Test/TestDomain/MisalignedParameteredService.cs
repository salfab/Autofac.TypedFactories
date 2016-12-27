namespace Autofac.TypedFactories.Test.TestDomain
{
    /// <summary>
    /// The constructor has parameter names not matching the ones from the factory <see cref="IParameteredServiceFactory"/>
    /// </summary>
    /// <seealso cref="IParameteredService" />
    public class MisalignedParameteredService : IParameteredService
    {
        public int Number { get; set; }

        public MisalignedParameteredService(int integer)
        {
            this.Number = integer;
        }
    }
}