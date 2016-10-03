using System;
using System.Data;

namespace Straight.Core.EventStore
{
    public abstract class DomainEventBase : IDomainEvent
    {
        private Guid _aggregateId = Guid.Empty;

        public Guid Id { get; } = Guid.NewGuid();

        public Guid AggregateId
        {
            get { return _aggregateId; }
            set
            {
                if (_aggregateId != Guid.Empty)
                {
                    throw new ReadOnlyException("AggregateId is already set.");
                }
                _aggregateId = value;
            }
        }

        public int Version { get; set; }
    }
}