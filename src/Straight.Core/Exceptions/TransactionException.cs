using System;
using System.Runtime.Serialization;

namespace Straight.Core.Exceptions
{
    public class TransactionException : Exception
    {
        public TransactionException()
        {
        }

        public TransactionException(string message) : base(message)
        {
        }

        public TransactionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}