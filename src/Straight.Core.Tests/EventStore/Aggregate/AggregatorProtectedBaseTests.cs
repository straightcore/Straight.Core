using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using Xunit;

namespace Straight.Core.Tests.EventStore.Aggregate
{
    public class AggregatorProtectedBaseTests
    { 
        private IAggregator<IDomainEvent> aggregate;
        private Action actionInAggregatorTest = () => { };

        public AggregatorProtectedBaseTests()
        {
            aggregate = new AggregatorProtectedTest(() => actionInAggregatorTest());
        }

        [Fact]
        public void Should_call_Apply_domain_event_when_event_is_raise_in_aggregate_base()
        {
            var isCalled = false;
            actionInAggregatorTest = () => isCalled = true;
            aggregate.Update(new DomainCommandTest());
            Assert.True(isCalled);
        }
    }

    public class AggregatorProtectedTest : AggregatorProtectedBase<IDomainEvent>
        //, IApplyEvent<DomainEventTest>
        , IHandlerDomainCommand<DomainCommandTest>
        , IHandlerDomainCommand<DomainCommandTest2>
    {
        private Action _whenApplied;

        public AggregatorProtectedTest(Action injectionMethod)
        {
            _whenApplied = injectionMethod;
        }

        private void Apply(DomainEventTest @event)
        {
            _whenApplied?.Invoke();
        }

        public IEnumerable Handle(DomainCommandTest command)
        {
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();
            yield return new DomainEventTest
            {
                Id = command.Id,
                AggregateId = Id,
                Version = 1
            };
        }

        public IEnumerable Handle(DomainCommandTest2 command)
        {
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();
            yield return new DomainEventTest2
            {
                Id = Guid.NewGuid(),
                AggregateId = command.Id,
                Version = 1
            };
        }
    }
}
