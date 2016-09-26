using System;
using System.Runtime.Serialization;

namespace Straight.Core.Exceptions
{
    [Serializable]
    public class NotRegisteredRouteException : Exception
    {
        public NotRegisteredRouteException(Type typeOfMessage)
            : base($"Router does not have route for {typeOfMessage.FullName}")
        {
        }

        protected NotRegisteredRouteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}