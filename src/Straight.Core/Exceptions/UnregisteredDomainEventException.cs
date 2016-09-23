using System;
using System.Runtime.Serialization;

namespace Straight.Core.Exceptions
{
    [Serializable]
    public class UnregisteredDomainEventException : Exception
    {
        public UnregisteredDomainEventException()
        {
        }

        public UnregisteredDomainEventException(string message) : base(message)
        {
        }

        public UnregisteredDomainEventException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnregisteredDomainEventException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}