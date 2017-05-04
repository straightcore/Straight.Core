using System;
using System.Runtime.CompilerServices;

namespace Straight.Core.Common.Guards
{
    public static class StringGuard
    {
        public static void ArgumentIsNullOrEmpty(this string str, [CallerMemberName] string callMember = null)
        {
            callMember = callMember ?? "parameter";
            str.ArgumentIsNull(callMember);
            str.ArgumentIsEmpty(callMember);    
        }

        public static void ArgumentIsNull(this string str, [CallerMemberName] string callMember = null)
        {
            if (str.IsNull())
            {
                throw new ArgumentNullException(callMember);
            }            
        }

        public static void ArgumentIsEmpty(this string str, [CallerMemberName] string callMember = null)
        {
            if (str.IsEmpty())
            {
                throw new ArgumentException(callMember, $"{callMember} cannot be empty");
            }
        }


        public static bool IsNull(this string str)
        {
            return str == null;
        }

        public static bool IsEmpty(this string str)
        {
            return str == string.Empty || str == "";
        }
    }
}
