using System.Collections.Generic;

namespace Straight.Core.Domain
{
    public interface IReadModel<TDomainEvent>: IVersionable, IIdentifiable
        where TDomainEvent : IDomainEvent
    {
        IEnumerable<TDomainEvent> Events { get; }

        void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents);
    }
}
