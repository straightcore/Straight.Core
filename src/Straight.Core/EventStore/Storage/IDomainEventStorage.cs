using System;
using System.Collections.Generic;
using Straight.Core.Domain;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Storage;

namespace Straight.Core.EventStore.Storage
{
    public interface IDomainEventStorage<TDomainEvent> : Transactional
        where TDomainEvent : IDomainEvent
        //ISnapShotStorage<TDomainEvent>,
    {
        IEnumerable<TDomainEvent> Get(Guid aggregateId);
        void Save(IDomainEventChangeable<TDomainEvent> aggregator);
    }
}