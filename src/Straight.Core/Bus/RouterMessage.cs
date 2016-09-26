using Straight.Core.Exceptions;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Storage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Straight.Core.Bus
{
    public class RouterMessage : IRouterMessage, IRegisterRouteMessage
    {
        private ImmutableDictionary<Type, ICollection<Action<object>>> _routes;

        public RouterMessage()
        {
            _routes = ImmutableDictionary<Type, ICollection<Action<object>>>.Empty;
        }

        public void Register<TMessage>(Action<TMessage> route) where TMessage : class
        {
            Register(typeof(TMessage), route);
        }

        public void Register<TMessage>(Type typeOfMessage, Action<TMessage> route) where TMessage : class
        {
            ICollection<Action<object>> routes;
            if (!_routes.TryGetValue(typeOfMessage, out routes))
            {
                routes = new LinkedList<Action<object>>();
                _routes = _routes.Add(typeOfMessage, routes);
            }
            routes.Add(message => route(message as TMessage));
        }

        public void Route(object message)
        {
            ICollection<Action<object>> routes;
            if (!_routes.TryGetValue(message.GetType(), out routes))
            {
                throw new NotRegisteredRouteException(message.GetType());
            }
            routes.ForEach(route => route(message));
        }
    }
}