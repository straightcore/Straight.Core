using System;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.Extensions.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            enumerable.CastOrToList().ForEach(action);            
            return enumerable;
        }

        public static List<T> CastOrToList<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable as List<T> ?? enumerable.ToList());
        }
    }
}
