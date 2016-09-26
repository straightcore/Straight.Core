using Straight.Core.Domain;
using Straight.Core.EventStore.Aggregate;
using System;

namespace Straight.Core.EventStore.Storage
{
    public interface IDomainEventRepository<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregator<TDomainEvent>, new();

        void Add<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<TDomainEvent>, new();

        void RegisterForTracking<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<TDomainEvent>, new();
    }
}