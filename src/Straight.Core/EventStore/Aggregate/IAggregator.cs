using Straight.Core.Domain;
using System.Collections.Generic;

namespace Straight.Core.EventStore.Aggregate
{
    public interface IDomainEventChangeable<out TDomainEvent> : IVersionableUpdatable, IIdentifiable
    {
        IEnumerable<TDomainEvent> GetChanges();

        void Clear();
    }

    public interface IAggregator<TDomainEvent> : IDomainEventChangeable<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        void Reset();

        void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents);

        void Update<TDomainCommand>(TDomainCommand command) where TDomainCommand : class, IDomainCommand;
    }
}