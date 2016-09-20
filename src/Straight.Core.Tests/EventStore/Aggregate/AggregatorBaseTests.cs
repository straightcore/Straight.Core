using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using System;
using System.Collections;
using Straight.Core.Domain;

namespace Straight.Core.Tests.EventStore.Aggregate
{
    [TestFixture]
    public class AggregatorBaseTests
    {
        [Test]
        public void Should_call_Apply_domain_event_when_event_is_raise_in_aggregate_base()
        {
            bool isCalled = false;
            IAggregator<IDomainCommand, IDomainEvent> aggregate = new AggregatorTest(() => isCalled = true);
            aggregate.Handle(new DomainCommandTest());
            Assert.That(isCalled);
        }
    }

    internal class DomainCommandTest : IDomainCommand
    {
        public Guid Id
        {
            get;
            set;
        }
    }

    internal class DomainEventTest : IDomainEvent
    {
        public Guid AggregateId
        {
            get;

            set;
        }

        public Guid Id
        {
            get;

            set;
        }

        public int Version
        {
            get;
            set;
        }
    }

    internal class AggregatorTest : AggregatorBase<IDomainCommand, IDomainEvent>
        , IApplyEvent<DomainEventTest>
        , IHandlerDomainCommand<DomainCommandTest>
    {
        private Action whenApplied;

        public AggregatorTest(Action whenApplied)
            : base()
        {
            this.whenApplied = whenApplied;
        }

        public void Apply(DomainEventTest @event)
        {
            Id = @event.Id;            
            Version = @event.Version;
            if(whenApplied != null)
            {
                whenApplied();
            }
        }

        public IEnumerable Handle(DomainCommandTest command)
        {
            yield return new DomainEventTest()
            {
                Id = Guid.NewGuid(),
                AggregateId = command.Id,
                Version = 1
            };
        }
    }
}
