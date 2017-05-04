using Xunit;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Exceptions;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.Tests.EventStore.Aggregate
{
    
    public class AggregatorBaseTests
    {
        
        public AggregatorBaseTests()
        {
            aggregate = new AggregatorTest(() => actionInAggregatorTest());
        }

        private IAggregator<IDomainEvent> aggregate;
        private Action actionInAggregatorTest = () => { };

        [Fact]
        public void Should_call_Apply_domain_event_when_event_is_raise_in_aggregate_base()
        {
            var isCalled = false;
            actionInAggregatorTest = () => isCalled = true;
            aggregate.Update(new DomainCommandTest());
            Assert.True(isCalled);
        }

        [Fact]
        public void Should_does_throw_exception_when_command_is_not_found()
        {
            Assert.Throws<UnregisteredDomainEventException>(() => aggregate.Update(new DomainCommandUnknow()));
        }

        [Fact]
        public void Should_does_throw_exception_when_domainEvent_is_not_found()
        {
            Assert.Throws<UnregisteredDomainEventException>(() => aggregate.Update(new DomainCommandTest2()));
        }

        [Fact]
        public void Should_have_change_event_when_execute_new_command()
        {
            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            aggregate.Update(domainCommandTest);
            var domainEvents = aggregate.GetChanges();
            Assert.NotNull(domainEvents);
            Assert.NotEmpty(domainEvents);
            Assert.Equal(domainEvents.Count(ev => ev.Id == domainCommandTest.Id), 1);
        }

        [Fact]
        public void Should_load_historical_event_when_load_aggregate_root_model()
        {
            var guid = Guid.NewGuid();
            var events = new List<IDomainEvent>();
            for (var version = 0; version < 5; version++)
                events.Add(new DomainEventTest {Id = Guid.NewGuid(), AggregateId = guid, Version = version});
            aggregate.LoadFromHistory(events);
            Assert.Equal(aggregate.Id, guid);
            Assert.Equal(aggregate.Version, events.Last().Version);
        }
    }
}