using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Straight.Core.Command;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Helper;

namespace Straight.Core.Messaging
{
    public class CommandHandlerDispatcher : ICommandHandlerDispatcher
    {
        private static readonly Type GenericCommandHandlerType = typeof(ICommandHandler<>);


        private ImmutableDictionary<Type, ImmutableList<ICommandHandler>> _handlerRegistred =
            ImmutableDictionary<Type, ImmutableList<ICommandHandler>>.Empty;
        private ImmutableDictionary<Type, MethodInfo> _methods = ImmutableDictionary<Type, MethodInfo>.Empty;

        public void Register(ICommandHandler handler)
        {
            foreach (var commandType in handler.GetType()
                .GetInterfaces()
                .Where(iface => iface.IsGenericType
                                && iface.GetGenericTypeDefinition() == GenericCommandHandlerType)
                .Select(iface => iface.GetGenericArguments()[0]))
            {
                SetHandlerRegister(handler, commandType);
                SetMethodInfo(handler, commandType);
            }
        }

        private void SetMethodInfo(ICommandHandler handler, Type commandType)
        {
            var methods = GetRegisterByType(handler.GetType(), typeof(ICommandHandler<>), commandType);
            _methods = _methods.AddRange(methods.Where(k => !_methods.ContainsKey(k.Key)));
        }

        private void SetHandlerRegister(ICommandHandler handler, Type commandType)
        {
            ImmutableList<ICommandHandler> repoHandler;
            if (!_handlerRegistred.TryGetValue(commandType, out repoHandler))
            {
                repoHandler = ImmutableList<ICommandHandler>.Empty;
                _handlerRegistred = _handlerRegistred.Add(commandType, repoHandler.Add(handler));
            }
            else
            {
                _handlerRegistred = _handlerRegistred.SetItem(commandType, repoHandler.Add(handler));
            }
        }

        public void Process(ICommand command)
        {
            ImmutableList<ICommandHandler> repoHandler;
            MethodInfo handleMethod;
            if (!_handlerRegistred.TryGetValue(command.GetType(), out repoHandler)
                || !_methods.TryGetValue(command.GetType(), out handleMethod))
            {
                throw new ArgumentOutOfRangeException($"{command.GetType().FullName} is not recognized");
            }
            repoHandler.ForEach(h => handleMethod.Invoke(h, new[] {command}));
        }

        private static Dictionary<Type, MethodInfo> GetRegisterByType(Type commandHandler, Type typeOfInterfaceBase, Type genericArguments)
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