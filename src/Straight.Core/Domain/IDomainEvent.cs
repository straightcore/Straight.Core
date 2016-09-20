using System;

namespace Straight.Core.Domain
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        Guid AggregateId { get; set; }
        int Version { get; set; }
    }
}