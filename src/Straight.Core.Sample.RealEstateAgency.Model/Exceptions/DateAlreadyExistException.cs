using System;

namespace Straight.Core.Sample.RealEstateAgency.Model.Exceptions
{
    public class DateAlreadyExistException : ArgumentException
    {
        public DateAlreadyExistException(DateTime meet)
            : base($"'{meet:ddd MMMM yyyy HH:mm}' is already used for other visit")
        {
        }
        
    }
}