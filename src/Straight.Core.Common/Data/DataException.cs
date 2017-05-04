using System;

namespace Straight.Core.Common.Data
{
    
    public class DataException : StraightCoreException
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Data.DataException" /> class. This is the default constructor.</summary>
        public DataException()
            : base("Exception Data")
        {
            this.HResult = -2146232032;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.DataException" /> class with the specified string.</summary>
        /// <param name="s">The string to display when the exception is thrown. </param>
        public DataException(string s)
            : base(s)
        {
            this.HResult = -2146232032;
        }

        /// <summary>Initializes a new instance of the <see cref="T:System.Data.DataException" /> class with the specified string and inner exception.</summary>
        /// <param name="s">The string to display when the exception is thrown. </param>
        /// <param name="innerException">A reference to an inner exception. </param>
        public DataException(string s, Exception innerException)
            : base(s, innerException)
        {
        }
    }
}
