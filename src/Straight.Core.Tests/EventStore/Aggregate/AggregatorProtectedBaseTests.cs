using Straight.Core.EventStore;
using Straight.Core.Domain.Aggregate;
using System;
using System.Collections;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using NUnit.Framework;

namespace Straight.Core.Tests.EventStore.Aggregate
{
    [TestFixture]
    public class AggregatorProtectedBaseTests
    { 
        

        [Test]
        public void Should_call_Apply_domain_event_when_event_is_raise_in_aggregate_base()
        {
            Action actionInAggregatorTest = () => { };
            var aggregate = new AggregatorProtectedTest(() => actionInAggregatorTest());
            var isCalled = false;
            actionInAggregatorTest = () => isCalled = true;
            aggregate.Update(new DomainCommandTest());
            Assert.True(isCalled);
        }
    }

    public class AggregatorProtectedTest : AggregatorProtectedBase<IDomainEvent>
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

        private IEnumerable Handle(DomainCommandTest command)
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

        private IEnumerable Handle(DomainCommandTest2 command)
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
