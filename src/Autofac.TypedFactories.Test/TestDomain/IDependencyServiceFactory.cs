namespace Autofac.TypedFactories.Test
{
    public interface IDependencyServiceFactory
    {
        IDependencyService Create();
    }
    public interface IDependencyOtherServiceFactory
    {
        IDependencyService Create();
    }
}