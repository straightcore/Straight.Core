using System;
using System.Collections.Immutable;
using System.Linq;
using Straight.Core.Domain;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Storage;
using Straight.Core.Storage.Generic;

namespace Straight.Core.EventStore.Storage
{
    public class InMemoryDomainEventStoreUnitOfWork<TDomainEvent> :
            IDomainEventStoreUnitOfWork<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private readonly IBus<TDomainEvent> _bus;
        private readonly IAggregatorRootMap<TDomainEvent> _identityRootMap;
        private readonly IDomainEventStorage<TDomainEvent> _repository;
        private ImmutableDictionary<Guid, IAggregator<TDomainEvent>> _aggregatorsPending;

        public InMemoryDomainEventStoreUnitOfWork(
            IAggregatorRootMap<TDomainEvent> identityRootMap,
            IDomainEventStorage<TDomainEvent> repository,
            IBus<TDomainEvent> bus)
        {
            _identityRootMap = identityRootMap;
            _repository = repository;
            _bus = bus;
            _aggregatorsPending = ImmutableDictionary<Guid, IAggregator<TDomainEvent>>.Empty;
        }

        public void Commit()
        {
            var pending = _aggregatorsPending.Values;
            _aggregatorsPending = ImmutableDictionary<Guid, IAggregator<TDomainEvent>>.Empty;
            _repository.BeginTransaction();
            foreach (var eventProvider in pending)
            {
                _repository.Save(eventProvider);
                _bus.Publish(eventProvider.GetChanges());
                eventProvider.Clear();
            }
            _bus.Commit();
            _repository.Commit();
        }

        public void Rollback()
        {
            _aggregatorsPending = ImmutableDictionary<Guid, IAggregator<TDomainEvent>>.Empty;
            _identityRootMap.Clear();
        }

        public TAggregate GetById<TAggregate>(Guid id)
            where TAggregate : class, IAggregator<TDomainEvent>, new()
        {
            var aggregateRoot = _identityRootMap.GetById<TAggregate>(id) ?? LoadHistoryEvents<TAggregate>(id);
            if (aggregateRoot == null)
            {
                return aggregateRoot;
            }
            RegisterForTracking(aggregateRoot);
            return aggregateRoot;
        }

        private TAggregate LoadHistoryEvents<TAggregate>(Guid id)
            where TAggregate : class, IAggregator<TDomainEvent>, new()
        {
            var aggregate = new TAggregate();
            var domainEvents = _repository.Get(id).ToList();
            if (!domainEvents.Any())
            {
                return null;
            }
            aggregate.LoadFromHistory(domainEvents);
            return aggregate;
        }

        public void Add<TAggregate>(TAggregate aggregateRoot)
            where TAggregate : class, IAggregator<TDomainEvent>, new()
        {
            RegisterForTracking(aggregateRoot);
        }

        public void RegisterForTracking<TAggregate>(TAggregate aggregateRoot)
            where TAggregate : class, IAggregator<TDomainEvent>, new()
        {
            if (_aggregatorsPending.ContainsKey(aggregateRoot.Id))
            {
                return;
            }
            _identityRootMap.Add(aggregateRoot);
            _aggregatorsPending = _aggregatorsPending.Add(aggregateRoot.Id, aggregateRoot);
        }
    }
}