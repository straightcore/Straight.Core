using System;
using System.Text.RegularExpressions;

namespace Straight.Core.Extensions.Guard
{
    public static class Guard
    {
        public static void CheckIfArgumentIsNull<T>(this T source, string name)
        {
            if (source == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void CheckIfArgumentIsNullOrEmpty(this string source, string name)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException("source");
            }
        }

        public static void CheckRegexValidity(this Regex regex, string value, string name)
        {
            if (!regex.IsMatch(value))
            {
                throw new ArgumentException($"{name} format is not valid");
            }
        }
    }
}