namespace Autofac.TypedFactories.Exceptions
{
    using System;

    public class TypeCannotBeCreatedByFactoryException : Exception
    {
        public TypeCannotBeCreatedByFactoryException(string message) : base(message)
        {
        }
    }
}