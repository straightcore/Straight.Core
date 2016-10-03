using Straight.Core.Domain;
using Straight.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Straight.Core.EventStore;

namespace Straight.Core.Extensions.EventStore
{
    public static class DomainCommandHandlerExtensions
    {
        public static IEnumerable<TDomainEvent> Handle<TDomainEvent>(
                this IReadOnlyDictionary<Type, MethodInfo> registerMethods,
                object model,
                object command)
            where TDomainEvent : IDomainEvent
        {
            MethodInfo handler;
            if (!registerMethods.TryGetValue(command.GetType(), out handler))
            {
                throw new UnregisteredDomainEventException(
                    $"The domain command '{command.GetType().FullName}' is not registered in '{model.GetType().FullName}'");
            }
            return ((IEnumerable)handler.Invoke(model, new[] { command }))
                            .OfType<TDomainEvent>();
        }
    }
}