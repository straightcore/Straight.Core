// ==============================================================================================================
// Straight Compagny
// Straight Core
// ==============================================================================================================
// ©2018 Straight Compagny. All rights reserved.
// Licensed under the MIT License (MIT); you may not use this file except in compliance
// with the License. You may obtain have a last condition or last licence at https://github.com/straightcore/Straight.Core/blob/master
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

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
                throw new NotRegisteredRouteException(message.GetType());
            routes.ForEach(route => route(message));
        }
    }
}