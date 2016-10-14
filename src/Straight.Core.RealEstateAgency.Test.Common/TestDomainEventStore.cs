using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.EventStore.Storage;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Straight.Core.RealEstateAgency.Test.Common
{
    public class TestDomainEventStore : IDomainEventStore<IDomainEvent>
    {
        private readonly Dictionary<Guid, IAggregator<IDomainEvent>> _memory =
            new Dictionary<Guid, IAggregator<IDomainEvent>>();

        public IReadOnlyDictionary<Guid, IAggregator<IDomainEvent>> Memory
            => ImmutableDictionary<Guid, IAggregator<IDomainEvent>>.Empty.AddRange(_memory);

        public TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregator<IDomainEvent>, new()
        {
            IAggregator<IDomainEvent> value;
            if (_memory.TryGetValue(id, out value))
                return (TAggregate) value;
            return null;
        }

        public void Add<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<IDomainEvent>, new()
        {
            _memory[aggregateRoot.Id] = aggregateRoot;
        }
    }
}