using System;

namespace Straight.Core.Common.Data
{
    /// <summary>Represents the exception that is thrown when you try to change the value of a read-only column.</summary>
    /// <filterpriority>1</filterpriority>
    
    public class ReadOnlyException : DataException
    {
        
        /// <summary>Initializes a new instance of the <see cref="T:System.Data.ReadOnlyException" /> class.</summary>
        public ReadOnlyException()
            : base("ReadOnly Exception Data")
        {
            this.HResult = -2146232025;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.ReadOnlyException" /> class with the specified string.</summary>
        /// <param name="s">The string to display when the exception is thrown. </param>
        public ReadOnlyException(string s)
            : base(s)
        {
            this.HResult = -2146232025;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.ReadOnlyException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public ReadOnlyException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.HResult = -2146232025;
        }
    }
}
