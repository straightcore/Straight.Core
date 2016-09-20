using Straight.Core.Domain;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Domain;
using Straight.Core.Extensions.EventStore;
using Straight.Core.Extensions.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Straight.Core.EventStore.Aggregate
{
    public abstract class AggregatorBase<TDomainCommand, TDomainEvent> : IAggregator<TDomainCommand, TDomainEvent>
        where TDomainEvent : IDomainEvent
        where TDomainCommand : IDomainCommand
    {
        private const string APPLY_METHOD_NAME = "Apply";
        private const string HANDLE_METHOD_NAME = "Handle";
        private static readonly Type applyEventType = typeof(IApplyEvent<>);
        private static readonly Type handlerDomainCommandType = typeof(IHandlerDomainCommand<>);
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> registerApplyMethodsByType 
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> registerHandleMethodsByType 
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();

        private readonly IReadOnlyDictionary<Type, MethodInfo> registerMethods;
        private readonly List<TDomainEvent> appliedEvents;

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }
        public int EventVersion { get; protected set; }
        
        protected AggregatorBase()
        {
            registerMethods = GetRegisterByType(registerApplyMethodsByType, applyEventType, APPLY_METHOD_NAME)
                .Union(GetRegisterByType(registerHandleMethodsByType, handlerDomainCommandType, HANDLE_METHOD_NAME))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            appliedEvents = new List<TDomainEvent>();
        }

        public void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents)
        {
            if (!domainEvents.Any())
            {
                return;
            }
            domainEvents.ForEach(ev => registerMethods.Apply(this, ev));
            appliedEvents.AddRange(domainEvents);
            Version = domainEvents.Last().Version;
            EventVersion = Version;
        }

        public IEnumerable<TDomainEvent> GetChanges()
        {
            return appliedEvents.ToList();
        }

        public void Clear()
        {
            appliedEvents.Clear();
        }

        public void UpdateVersion(int version)
        {
            Version = version;
        }
        
        private int GetNewEventVersion()
        {
            return ++EventVersion;
        }

        public void Handle(TDomainCommand command)
        {
            registerMethods.Handle<TDomainEvent>(this, command).ForEach(ev => Apply(ev));
        }

        protected void Apply(TDomainEvent domainEvent)
        {
            domainEvent.AggregateId = Id;
            domainEvent.Version = GetNewEventVersion();
            registerMethods.Apply(this, domainEvent);
            appliedEvents.Add(domainEvent);
        }
        
        private IReadOnlyDictionary<Type, MethodInfo> GetRegisterByType(
            ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> register,
            Type typeOfInterfaceBase, 
            string methodName)
        {
            IReadOnlyDictionary<Type, MethodInfo> referentiel;
            if (!register.TryGetValue(GetType(), out referentiel))
            {
                register[GetType()] = referentiel = MappingTypeToMethodHelper.ToMappingTypeMethod(
                    GetType(),
                    typeof(TDomainEvent),
                    typeOfInterfaceBase,
                    methodName);
            }
            return referentiel;
        }
    }
}
