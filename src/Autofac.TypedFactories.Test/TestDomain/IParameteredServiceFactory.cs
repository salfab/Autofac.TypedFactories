namespace Autofac.TypedFactories.Test
{
    public interface IParameteredServiceFactory
    {
        IParameteredService Create(int number);
    }

    public interface IParameteredService
    {
        int Number { get; set; }
    }
}