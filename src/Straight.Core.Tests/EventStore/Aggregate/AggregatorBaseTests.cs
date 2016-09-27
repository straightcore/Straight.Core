using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Straight.Core.Domain;
using Straight.Core.Exceptions;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;

namespace Straight.Core.Tests.EventStore.Aggregate
{
    [TestFixture]
    public class AggregatorBaseTests
    {
        IAggregator<IDomainEvent> aggregate;
        Action actionInAggregatorTest = () => { };

        [SetUp]
        public void SetUp()
        {
            aggregate = new AggregatorTest(() => actionInAggregatorTest());
        }

        [Test]
        public void Should_call_Apply_domain_event_when_event_is_raise_in_aggregate_base()
        {
            var isCalled = false;
            actionInAggregatorTest = () => isCalled = true;
            aggregate.Update(new DomainCommandTest());
            Assert.That(isCalled);
        }

        [Test]
        public void Should_load_historical_event_when_load_aggregate_root_model()
        {
            var guid = Guid.NewGuid();
            var events = new List<IDomainEvent>();
            for (var version = 0; version < 5; version++)
            {
                events.Add(new DomainEventTest() {Id = Guid.NewGuid(), AggregateId = guid, Version = version});
            }
            aggregate.LoadFromHistory(events);
            Assert.That(aggregate.Id, Is.EqualTo(guid));
            Assert.That(aggregate.Version, Is.EqualTo(events.Last().Version));
        }

        [Test]
        public void Should_have_change_event_when_execute_new_command()
        {
            var domainCommandTest = new DomainCommandTest() {Id = Guid.NewGuid()};
            aggregate.Update(domainCommandTest);
            Assert.That(aggregate.GetChanges(), Is.Not.Null.And.Not.Empty);
            Assert.That(aggregate.GetChanges().Count(ev => ev.Id == domainCommandTest.Id), Is.EqualTo(1));
        }

        [Test]
        public void Should_does_throw_exception_when_command_is_not_found()
        {
            Assert.Throws<UnregisteredDomainEventException>(() => aggregate.Update(new DomainCommandUnknow()));
        }

        [Test]
        public void Should_does_throw_exception_when_domainEvent_is_not_found()
        {
            Assert.Throws<UnregisteredDomainEventException>(() => aggregate.Update(new DomainCommandTest2()));
        }
    }

}