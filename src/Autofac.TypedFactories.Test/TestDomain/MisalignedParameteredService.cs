namespace Autofac.TypedFactories.Test
{
    /// <summary>
    /// The constructor has parameter names not matching the ones from the factory <see cref="IParameteredServiceFactory"/>
    /// </summary>
    /// <seealso cref="Autofac.TypedFactories.Test.IParameteredService" />
    public class MisalignedParameteredService : IParameteredService
    {
        public int Number { get; set; }

        public MisalignedParameteredService(int integer)
        {
            this.Number = integer;
        }
    }
}