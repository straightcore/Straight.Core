using System;
using System.Runtime.CompilerServices;

namespace Straight.Core.Common.Guards
{
    public static class ObjectGuard
    {
        public static void ArgumentNull(this object obj, [CallerMemberName] string callMember = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(callMember);
            }
        }
    }
}
