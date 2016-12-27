namespace Autofac.TypedFactories.Test.TestDomain
{
    public interface IParameteredServiceFactory
    {
        IParameteredService Create(int number);
    }
}