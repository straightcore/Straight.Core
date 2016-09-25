using System;
using Straight.Core.Domain;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Storage;

namespace Straight.Core.EventStore.Storage
{
    public interface IDomainEventStoreUnitOfWork<out TDomainCommand, TDomainEvent> : IUnitOfWork 
        where TDomainEvent : IDomainEvent
        where TDomainCommand : IDomainCommand
    {
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregator<TDomainCommand, TDomainEvent>, new();
        void Add<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<TDomainCommand, TDomainEvent>, new();
        void RegisterForTracking<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<TDomainCommand, TDomainEvent>, new();
    }
    
}