using System;

namespace Straight.Core.Common
{
    public abstract class StraightCoreException : Exception
    {
        protected StraightCoreException(string message) : base(message)
        {
        }
        
        protected StraightCoreException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
