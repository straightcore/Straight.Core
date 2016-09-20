using Straight.Core.Domain;
using System;
using System.Collections.Generic;

namespace Straight.Core.EventStore.Aggregate
{
    public interface IAggregator<TDomainCommand, TDomainEvent> : IVersionable, IIdentifiable
        where TDomainEvent : IDomainEvent
        where TDomainCommand : IDomainCommand
    {
        void Clear();
        void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents);
        IEnumerable<TDomainEvent> GetChanges();
        void UpdateVersion(int version);
        void Handle(TDomainCommand command);
    }
}
