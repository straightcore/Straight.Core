using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Domain;
using Straight.Core.Extensions.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Straight.Core.Domain
{
    public abstract class ReadModelBase<TDomainEvent> : IReadModel<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private const string ApplyMethodName = "Apply";

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> RegisterApplyMethodsByType
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();

        private readonly List<TDomainEvent> _appliedEvents;
        private readonly IReadOnlyDictionary<Type, MethodInfo> _registerMethods;

        protected ReadModelBase()
        {
            _registerMethods = GetRegisterByType(typeof(IApplyEvent<>), ApplyMethodName);
            _appliedEvents = new List<TDomainEvent>();
        }

        public Guid Id
        {
            get;
            private set;
        }

        public int Version
        {
            get;
            private set;
        }

        public IEnumerable<TDomainEvent> Events => _appliedEvents.AsReadOnly();

        public void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents)
        {
            _appliedEvents.Clear();
            Id = Guid.Empty;
            Version = 0;
            domainEvents.ForEach(Update);
        }

        public void Update(TDomainEvent domainEvent)
        {
            Id = domainEvent.AggregateId;
            Version = domainEvent.Version;
            _registerMethods.Apply(this, domainEvent);
            _appliedEvents.Add(domainEvent);
        }

        private IReadOnlyDictionary<Type, MethodInfo> GetRegisterByType(Type typeOfInterfaceBase, string methodName)
        {
            IReadOnlyDictionary<Type, MethodInfo> referentiel;
            if (!RegisterApplyMethodsByType.TryGetValue(GetType(), out referentiel))
            {
                RegisterApplyMethodsByType[GetType()] = referentiel = MappingTypeToMethodHelper.ToMappingTypeMethod(
                    GetType(),
                    typeof(TDomainEvent),
                    typeOfInterfaceBase,
                    methodName);
            }
            return referentiel;
        }
    }
}