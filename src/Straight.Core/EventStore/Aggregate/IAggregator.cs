using Straight.Core.Domain;
using System.Collections.Generic;

namespace Straight.Core.EventStore.Aggregate
{
    public interface IDomainEventChangeable<out TDomainEvent> : IVersionableUpdatable, IIdentifiable
    {
        IEnumerable<TDomainEvent> GetChanges();

        void Clear();
    }
    
    public interface IAggregator<in TDomainCommand, TDomainEvent> : IDomainEventChangeable<TDomainEvent>
        where TDomainEvent : IDomainEvent
        where TDomainCommand : IDomainCommand
    {
        void Reset();

        void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents);

        void Update(TDomainCommand command);
    }


}