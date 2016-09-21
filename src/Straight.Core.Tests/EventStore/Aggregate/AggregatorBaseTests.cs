using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Straight.Core.Domain;
using Straight.Core.Exceptions;

namespace Straight.Core.Tests.EventStore.Aggregate
{
    [TestFixture]
    public class AggregatorBaseTests
    {
        IAggregator<IDomainCommand, IDomainEvent> aggregate;
        Action actionInAggregatorTest = () => { };

        [SetUp]
        public void SetUp()
        {
            aggregate = new AggregatorTest(() => actionInAggregatorTest());
        }

        [Test]
        public void Should_call_Apply_domain_event_when_event_is_raise_in_aggregate_base()
        {
            bool isCalled = false;
            actionInAggregatorTest = () => isCalled = true;
            aggregate.Handle(new DomainCommandTest());
            Assert.That(isCalled);
        }

        [Test]
        public void Should_load_historical_event_when_load_aggregate_root_model()
        {
            var guid = Guid.NewGuid();
            var events = new List<IDomainEvent>();
            for (var version = 0; version < 5; version++)
            {
                events.Add(new DomainEventTest() { Id = Guid.NewGuid(), AggregateId = guid, Version = version });
            }
            aggregate.LoadFromHistory(events);
            Assert.That(aggregate.Id, Is.EqualTo(guid));
            Assert.That(aggregate.Version, Is.EqualTo(events.Last().Version));
        }

        [Test]
        public void Should_have_change_event_when_execute_new_command()
        {
            var domainCommandTest = new DomainCommandTest() { Id = Guid.NewGuid() };
            aggregate.Handle(domainCommandTest);
            Assert.That(aggregate.GetChanges(), Is.Not.Null.And.Not.Empty);
            Assert.That(aggregate.GetChanges().Count(ev => ev.Id == domainCommandTest.Id), Is.EqualTo(1));
        }

        [Test]
        public void Should_does_throw_exception_when_command_is_not_found()
        {
            Assert.Throws<UnregisteredDomainEventException>(() => aggregate.Handle(new DomainCommandUnknow()));
        }

        [Test]
        public void Should_does_throw_exception_when_domainEvent_is_not_found()
        {
            Assert.Throws<UnregisteredDomainEventException>(() => aggregate.Handle(new DomainCommandTest2()));
        }

    }

    internal class DomainCommandTest2 : IDomainCommand
    {
        public Guid Id
        {
            get;
            set;
        }
    }

    internal class DomainCommandUnknow : IDomainCommand
    {
        public Guid Id
        {
            get;
            set;
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

    internal class DomainEventTest2 : IDomainEvent
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
        , IHandlerDomainCommand<DomainCommandTest2>
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

            whenApplied?.Invoke();
        }

        public IEnumerable Handle(DomainCommandTest2 command)
        {
            yield return new DomainEventTest2()
            {
                Id = Guid.NewGuid(),
                AggregateId = command.Id,
                Version = 1
            };
        }

        public IEnumerable Handle(DomainCommandTest command)
        {
            yield return new DomainEventTest()
            {
                Id = command.Id,
                AggregateId = command.Id,
                Version = 1
            };
        }
    }
}
