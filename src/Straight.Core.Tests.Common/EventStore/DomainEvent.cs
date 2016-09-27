using System;
using Straight.Core.Domain;

namespace Straight.Core.Tests.Common.EventStore
{

    public class DomainEventTest : IDomainEvent
    {
        public Guid AggregateId { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }
    }

    public class DomainEventTest2 : IDomainEvent
    {
        public Guid AggregateId { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }
    }

}