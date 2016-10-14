using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Model.Exceptions
{
    public class DateAlreadyExistException : ArgumentException
    {
        public DateAlreadyExistException(DateTime meet)
            : base($"'{meet:ddd MMMM yyyy HH:mm}' is already used for other visit")
        {
        }

        protected DateAlreadyExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}