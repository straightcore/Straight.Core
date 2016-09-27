using System;
using System.Collections;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Tests.Common.Domain;

namespace Straight.Core.Tests.Common.EventStore
{

    public class AggregatorTest : AggregatorBase<IDomainEvent>
        , IApplyEvent<DomainEventTest>
        , IHandlerDomainCommand<DomainCommandTest>
        , IHandlerDomainCommand<DomainCommandTest2>
    {
        private readonly Action _whenApplied;

        public AggregatorTest()
            : this(() => { })
        {

        }

        public AggregatorTest(Action whenApplied)
            : base()
        {
            this._whenApplied = whenApplied;
        }

        public void Apply(DomainEventTest @event)
        {
            _whenApplied?.Invoke();
        }

        public IEnumerable Handle(DomainCommandTest2 command)
        {
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
            }
            yield return new DomainEventTest2()
            {
                Id = Guid.NewGuid(),
                AggregateId = command.Id,
                Version = 1
            };
        }

        public IEnumerable Handle(DomainCommandTest command)
        {
            if (Id == Guid.Empty)
            {
                Id = Guid.NewGuid();
            }
            yield return new DomainEventTest()
            {
                Id = command.Id,
                AggregateId = Id,
                Version = 1
            };
        }
    }
}