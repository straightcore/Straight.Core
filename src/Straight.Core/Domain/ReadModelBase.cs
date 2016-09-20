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
        private const string APPLY_METHOD_NAME = "Apply";
        private static readonly Type handlerDomainEventType = typeof(IApplyEvent<>);
        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> registerApplyMethodsByType 
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();
        private List<TDomainEvent> appliedEvents;
        private readonly IReadOnlyDictionary<Type, MethodInfo> registerMethods;
        private readonly Type tDomainEventType = typeof(TDomainEvent);

        public ReadModelBase()
        {
            registerMethods = GetRegisterByType(handlerDomainEventType, APPLY_METHOD_NAME);
            appliedEvents = new List<TDomainEvent>();
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

        public IEnumerable<TDomainEvent> Events
        {
            get
            {
                return appliedEvents.AsReadOnly();
            }
        }

        public void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents)
        {
            domainEvents.ForEach(ev => Apply(ev));
        }

        protected void Apply(TDomainEvent domainEvent)
        {
            domainEvent.AggregateId = Id;
            Version = domainEvent.Version;
            registerMethods.Apply(this, domainEvent);
            appliedEvents.Add(domainEvent);
        }
                
        private IReadOnlyDictionary<Type, MethodInfo> GetRegisterByType(Type typeOfInterfaceBase, string methodName)
        {
            IReadOnlyDictionary<Type, MethodInfo> referentiel;
            if (!registerApplyMethodsByType.TryGetValue(GetType(), out referentiel))
            {
                registerApplyMethodsByType[GetType()] = referentiel = MappingTypeToMethodHelper.ToMappingTypeMethod(
                    GetType(), 
                    typeof(TDomainEvent), 
                    typeOfInterfaceBase, 
                    methodName);
            }
            return referentiel;
        }
    }
}
