using NUnit.Framework;
using System;
using System.Collections.Generic;
using Straight.Core.Domain;
using Straight.Core.EventStore.Storage;
using Straight.Core.Tests.EventStore.Aggregate;

namespace Straight.Core.Tests.EventStore.Storage
{
    [TestFixture]
    public class InMemoryAggregatorRootMapTests
    {
        private InMemoryAggregatorRootMap<IDomainEvent> _rootMap;

        [SetUp]
        public void SetUp()
        {
            _rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
        }

        [Test]
        public void Should_return_null_when_model_is_not_found()
        {
            var root = _rootMap.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(root, Is.Null);
        }

        [Test]
        public void Should_get_by_id_when_model_is_added()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() {Id = Guid.NewGuid()});
            Assert.DoesNotThrow(() => _rootMap.Add(expected));
            var root = _rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(root, Is.EqualTo(expected));
        }

        [Test]
        public void Should_get_null_value_when_is_not_found_and_one_or_more_aggregator_is_saved()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() {Id = Guid.NewGuid()});
            Assert.DoesNotThrow(() => _rootMap.Add(expected));
            var root = _rootMap.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(root, Is.Null);
        }

        [Test]
        public void Should_remove_by_type_when_aggregate_is_in_memory()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() {Id = Guid.NewGuid()});
            _rootMap.Add(expected);
            expected = _rootMap.GetById<AggregatorTest>(expected.Id);
            _rootMap.Remove(expected.GetType(), expected.Id);
            var actual = _rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Should_remove_when_aggregate_is_in_memory()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() {Id = Guid.NewGuid()});
            _rootMap.Add(expected);
            expected = _rootMap.GetById<AggregatorTest>(expected.Id);
            _rootMap.Remove<AggregatorTest>(expected.Id);
            var actual = _rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }


        [Test]
        public void Should_does_throw_exception_when_remove_element_which_does_not_found()
        {
            Assert.DoesNotThrow(() => _rootMap.Remove<AggregatorTest>(Guid.NewGuid()));
        }

        [Test]
        public void Should_does_throw_exception_when_get_element_does_not_exist()
        {
            Assert.DoesNotThrow(() => _rootMap.GetById<AggregatorTest>(Guid.NewGuid()));
        }

        [Test]
        public void Should_does_not_throw_exception_when_Add_element()
        {
            Assert.DoesNotThrow(() => _rootMap.Add(new AggregatorTest()));
        }

        [Test]
        public void Should_throw_exception_when_Add_element_already_exist()
        {
            var aggregate = new AggregatorTest();
            var domainEvents = new List<IDomainEvent>() { new DomainEventTest() { Id = Guid.NewGuid(), Version = 1, AggregateId = Guid.NewGuid() } };
            aggregate.LoadFromHistory(domainEvents);
            _rootMap.Add(aggregate);
            aggregate = new AggregatorTest();
            aggregate.LoadFromHistory(domainEvents);
            Assert.Throws<ArgumentException>(() => _rootMap.Add(aggregate));
        }
    }
}