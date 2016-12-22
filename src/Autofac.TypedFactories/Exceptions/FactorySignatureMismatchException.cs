namespace Autofac.TypedFactories.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    public class FactorySignatureMismatchException : Exception
    {
        public FactorySignatureMismatchException()
        {
        }

        public FactorySignatureMismatchException(string message)
            : base(message)
        {
        }

        public FactorySignatureMismatchException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected FactorySignatureMismatchException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}