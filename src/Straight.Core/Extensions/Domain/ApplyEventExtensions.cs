using Straight.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Straight.Core.Extensions.Domain
{
    public static class ApplyEventExtensions
    {
        public static void Apply(this IReadOnlyDictionary<Type, MethodInfo> registerMethods, object model, object @event)
        {
            MethodInfo handler;
            if (!registerMethods.TryGetValue(@event.GetType(), out handler))
            {
                throw new UnregisteredDomainEventException(
                    string.Format(
                        "The domain event '{0}' is not registered in '{1}'",
                        @event.GetType().FullName,
                        model.GetType().FullName));
            }
            handler.Invoke(model, new object[] { @event });
        }
    }
}