using NUnit.Framework;
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
    [TestFixture]
    public class AggregatorBaseTests
    {
        private class AggregContext
        {
            public AggregContext()
            {
                Aggregate = new AggregatorTest(() => ActionInAggregatorTest());
            }

            public IAggregator<IDomainEvent> Aggregate { get; }
            public Action ActionInAggregatorTest { get; set; } = () => { };
        }

        [Test]
        public void Should_call_Apply_domain_event_when_event_is_raise_in_aggregate_base()
        {
            var context = new AggregContext();
            var isCalled = false;
            context.ActionInAggregatorTest = () => isCalled = true;
            context.Aggregate.Update(new DomainCommandTest());
            Assert.That(isCalled, Is.True);
        }

        [Test]
        public void Should_does_throw_exception_when_command_is_not_found()
        {
            var context = new AggregContext();
            Assert.Throws<UnregisteredDomainEventException>(() => context.Aggregate.Update(new DomainCommandUnknow()));
        }

        [Test]
        public void Should_does_throw_exception_when_domainEvent_is_not_found()
        {
            var context = new AggregContext();
            Assert.Throws<UnregisteredDomainEventException>(() => context.Aggregate.Update(new DomainCommandTest2()));
        }

        [Test]
        public void Should_have_change_event_when_execute_new_command()
        {
            var context = new AggregContext();
            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            context.Aggregate.Update(domainCommandTest);
            var domainEvents = context.Aggregate.GetChanges();
            Assert.That(domainEvents, Is.Not.Null.And.Not.Empty);
            Assert.That(domainEvents.Count(ev => ev.Id == domainCommandTest.Id), Is.EqualTo(1));
        }

        [Test]
        public void Should_load_historical_event_when_load_aggregate_root_model()
        {
            var context = new AggregContext();
            var guid = Guid.NewGuid();
            var events = new List<IDomainEvent>();
            for (var version = 0; version < 5; version++)
                events.Add(new DomainEventTest {Id = Guid.NewGuid(), AggregateId = guid, Version = version});
            context.Aggregate.LoadFromHistory(events);
            Assert.That(context.Aggregate.Id, Is.EqualTo(guid));
            Assert.That(context.Aggregate.Version, Is.EqualTo(events.Last().Version));
        }
    }
}