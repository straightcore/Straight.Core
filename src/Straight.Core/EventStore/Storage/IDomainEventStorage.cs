using Straight.Core.Domain;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Storage;
using System;
using System.Collections.Generic;

namespace Straight.Core.EventStore.Storage
{
    public interface IDomainEventStorage<TDomainEvent> : Transactional
        where TDomainEvent : IDomainEvent
    {
        IEnumerable<TDomainEvent> Get(Guid aggregateId);

        void Save(IDomainEventChangeable<TDomainEvent> aggregator);
    }
}