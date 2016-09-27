using System;
using System.Runtime.Serialization;

namespace Straight.Core.Exceptions
{
    public class DomainModelAlreadyExistException : SystemException
    {
        private static readonly string FullName = typeof(DomainModelAlreadyExistException).FullName;

        public DomainModelAlreadyExistException() : this($"A system exception ({FullName}) occurred")
        {
        }


        public DomainModelAlreadyExistException(string message) : base (message)
        {
            
        }

        public DomainModelAlreadyExistException(Guid id) : base($"{id} already exists")
        {
        }
        
        protected DomainModelAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}