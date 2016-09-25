using System;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.EventStore.Storage;

namespace Straight.Core.Tests.Common.EventStore.Storage
{
    public class InMemoryAggregatorRootMapMock : IAggregatorRootMap<IDomainCommand, IDomainEvent>
    {
        private readonly InMemoryAggregatorRootMap<IDomainCommand, IDomainEvent> _inMemory;

        public InMemoryAggregatorRootMapMock()
        {
            this._inMemory = new InMemoryAggregatorRootMap<IDomainCommand, IDomainEvent>();
        }

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregator<IDomainCommand, IDomainEvent>, new()
        {
            return _inMemory.GetById<TAggregate>(id);
        }

        public void Add<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<IDomainCommand, IDomainEvent>, new()
        {
            if (_inMemory.GetById<TAggregate>(aggregateRoot.Id) == null)
            {
                _inMemory.Add(aggregateRoot);
            }
        }

        public void Remove<TAggregate>(Guid aggregateRootId) where TAggregate : class, IAggregator<IDomainCommand, IDomainEvent>, new()
        {
            _inMemory.Remove<TAggregate>(aggregateRootId);
        }

        public void Remove(Type aggregateRootType, Guid aggregateRootId)
        {
            _inMemory.Remove(aggregateRootType, aggregateRootId);
        }

        public void Clear()
        {
            _inMemory.Clear();
        }
    }
}