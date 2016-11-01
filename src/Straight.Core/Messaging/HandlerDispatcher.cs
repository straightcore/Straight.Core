// ==============================================================================================================
// Straight Compagny
// Straight Core
// ==============================================================================================================
// ©2016 Straight Compagny. All rights reserved.
// Licensed under the MIT License (MIT); you may not use this file except in compliance
// with the License. You may obtain have a last condition or last licence at https://github.com/straightcore/Straight.Core/blob/master
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

using Straight.Core.Extensions.Guard;
using Straight.Core.Extensions.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Straight.Core.Messaging
{
    public class HandlerDispatcher<THandled, TProcessed> : IHandlerDispatcher<THandled, TProcessed>
        where THandled : class
        where TProcessed : class
    {
        private readonly Type _genericHandlerType;

        private ImmutableDictionary<Type, ImmutableList<THandled>> _handlerRegistred =
            ImmutableDictionary<Type, ImmutableList<THandled>>.Empty;

        private ImmutableDictionary<Type, MethodInfo> _methods = ImmutableDictionary<Type, MethodInfo>.Empty;

        public HandlerDispatcher(Type genericHandlerType)
        {
            genericHandlerType.CheckIfArgumentIsNull("genericHandlerType");
            _genericHandlerType = genericHandlerType;
        }

        public void Register(THandled handler)
        {
            foreach (var commandType in handler.GetType()
                .GetInterfaces()
                .Where(iface => iface.IsGenericType
                                && (iface.GetGenericTypeDefinition() == _genericHandlerType))
                .Select(iface => iface.GetGenericArguments()[0]))
            {
                SetHandlerRegister(handler, commandType);
                SetMethodInfo(handler, commandType);
            }
        }

        public void Process(TProcessed @event)
        {
            ImmutableList<THandled> repoHandler;
            MethodInfo handleMethod;
            if (!_handlerRegistred.TryGetValue(@event.GetType(), out repoHandler)
                || !_methods.TryGetValue(@event.GetType(), out handleMethod))
                throw new ArgumentOutOfRangeException($"{@event.GetType().FullName} is not recognized");
            repoHandler.ForEach(h => handleMethod.Invoke(h, new object[] {@event}));
        }

        private void SetMethodInfo(THandled handler, Type commandType)
        {
            var methods = GetRegisterByType(handler.GetType(), _genericHandlerType, commandType);
            _methods = _methods.AddRange(methods.Where(k => !_methods.ContainsKey(k.Key)));
        }

        private void SetHandlerRegister(THandled handler, Type commandType)
        {
            ImmutableList<THandled> repoHandler;
            if (!_handlerRegistred.TryGetValue(commandType, out repoHandler))
            {
                repoHandler = ImmutableList<THandled>.Empty;
                _handlerRegistred = _handlerRegistred.Add(commandType, repoHandler.Add(handler));
            }
            else
            {
                _handlerRegistred = _handlerRegistred.SetItem(commandType, repoHandler.Add(handler));
            }
        }

        private static Dictionary<Type, MethodInfo> GetRegisterByType(Type commandHandler, Type typeOfInterfaceBase,
            Type genericArguments)
        {
            return typeOfInterfaceBase.GetMethods()
                .Select(m => m.Name)
                .Select(m => MappingTypeToMethodHelper.ToMappingTypeMethod(
                    commandHandler
                    , genericArguments
                    , typeOfInterfaceBase
                    , m))
                .SelectMany(k => k)
                .GroupBy(k => k.Key)
                .ToDictionary(k => k.Key, k => k.First().Value);
        }
    }
}