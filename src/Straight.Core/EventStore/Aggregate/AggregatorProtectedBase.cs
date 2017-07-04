using Straight.Core.Domain;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Domain;
using Straight.Core.Extensions.EventStore;
using Straight.Core.Extensions.Guard;
using Straight.Core.Extensions.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Straight.Core.EventStore.Aggregate
{
    public class AggregatorProtectedBase<TDomainEvent> : IAggregator<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private const string ApplyMethodName = "Apply";
        private const string HandleMethodName = "Handle";

        private ImmutableList<TDomainEvent> _appliedEvents = ImmutableList<TDomainEvent>.Empty;
        private ImmutableList<TDomainEvent> _changedEvents = ImmutableList<TDomainEvent>.Empty;
        private readonly IReadOnlyDictionary<Type, MethodInfo> _registerMethods;

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> RegisterApplyMethodsByType
                = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> RegisterHandleMethodsByType
                = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }
        public int EventVersion { get; protected set; }

        protected AggregatorProtectedBase()
        {
            Id = Guid.NewGuid();
            _registerMethods = GetRegisterByType(RegisterApplyMethodsByType, ApplyMethodName)
                .Union(GetRegisterByType(RegisterHandleMethodsByType, typeof(IHandlerDomainCommand<>), HandleMethodName))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            
        }

        public void Clear()
        {
            _changedEvents = ImmutableList<TDomainEvent>.Empty;
        }

        public IEnumerable<TDomainEvent> GetChanges()
        {
            return _changedEvents.ToBuilder();
        }

        public void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents)
        {
            Reset();
            var events = domainEvents as IList<TDomainEvent> ?? domainEvents.ToList();
            if (!events.Any())
                return;
            events.ForEach(ev => _registerMethods.Apply(this, ev));
            _appliedEvents = _appliedEvents.AddRange(events);
            Version = events.Last().Version;
            Id = events.Last().AggregateId;
            EventVersion = Version;
        }

        public void Reset()
        {
            Id = Guid.Empty;
            Version = 0;
            EventVersion = 0;
            _appliedEvents = ImmutableList<TDomainEvent>.Empty;
            _changedEvents = ImmutableList<TDomainEvent>.Empty;
        }

        public void UpdateVersion(int version)
        {
            Version = version;
        }

        public void Update<TDomainCommand>(TDomainCommand command) where TDomainCommand : class, IDomainCommand
        {
            command.CheckIfArgumentIsNull("command");
            _changedEvents.AddRange(_registerMethods.Handle<TDomainEvent>(this, command)
                .Select(ExecuteApply));
        }

        private int GetNewEventVersion()
        {
            return ++EventVersion;
        }

        private TDomainEvent ExecuteApply(TDomainEvent @event)
        {
            @event.CheckIfArgumentIsNull("event");
            @event.AggregateId = Id;
            @event.Version = GetNewEventVersion();
            _registerMethods.Apply(this, @event);
            _appliedEvents.Add(@event);
            return @event;
        }

        private IReadOnlyDictionary<Type, MethodInfo> GetRegisterByType(IDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> register, string methodName)
        {
            if (!register.TryGetValue(GetType(), out IReadOnlyDictionary<Type, MethodInfo> referentiel))
            {
                register[GetType()] = referentiel = MappingTypeToMethodHelper.ToMappingTypeMethod(
                   GetType(),
                   typeof(TDomainEvent),
                   methodName);
            }
            return referentiel;
        }

        private IReadOnlyDictionary<Type, MethodInfo> GetRegisterByType(IDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> register,
            Type typeOfInterfaceBase,
            string methodName)
        {
            if (!register.TryGetValue(GetType(), out IReadOnlyDictionary<Type, MethodInfo> referentiel))
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
