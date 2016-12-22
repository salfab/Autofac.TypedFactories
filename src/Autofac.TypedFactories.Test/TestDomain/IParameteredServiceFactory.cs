namespace Autofac.TypedFactories.Test
{
    public interface IParameteredServiceFactory
    {
        IParameteredService Create(int number);
    }
}