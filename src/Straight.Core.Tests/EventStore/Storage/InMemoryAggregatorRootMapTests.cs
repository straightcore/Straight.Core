using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using System;
using System.Collections.Generic;

namespace Straight.Core.Tests.EventStore.Storage
{
    [TestFixture]
    public class InMemoryAggregatorRootMapTests
    {
        [Test]
        public void Should_does_not_throw_exception_when_Add_element()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            Assert.DoesNotThrow(() => rootMap.Add(new AggregatorTest()));
        }

        [Test]
        public void Should_does_throw_exception_when_get_element_does_not_exist()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            Assert.DoesNotThrow(() => rootMap.GetById<AggregatorTest>(Guid.NewGuid()));
        }

        [Test]
        public void Should_does_throw_exception_when_remove_element_which_does_not_found()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            Assert.DoesNotThrow(() => rootMap.Remove<AggregatorTest>(Guid.NewGuid()));
        }

        [Test]
        public void Should_get_by_id_when_model_is_added()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            rootMap.Add(expected);
            var root = rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(root, Is.EqualTo(expected));
        }

        [Test]
        public void Should_get_null_value_when_is_not_found_and_one_or_more_aggregator_is_saved()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            rootMap.Add(expected);
            var root = rootMap.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(root, Is.Null);
        }

        [Test]
        public void Should_remove_by_type_when_aggregate_is_in_memory()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            rootMap.Add(expected);
            expected = rootMap.GetById<AggregatorTest>(expected.Id);
            rootMap.Remove(expected.GetType(), expected.Id);
            var actual = rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Should_remove_when_aggregate_is_in_memory()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            rootMap.Add(expected);
            expected = rootMap.GetById<AggregatorTest>(expected.Id);
            rootMap.Remove<AggregatorTest>(expected.Id);
            var actual = rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Should_return_null_when_model_is_not_found()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            var root = rootMap.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(root, Is.Null);
        }

        [Test]
        public void Should_throw_exception_when_Add_element_already_exist()
        {
            var rootMap = new InMemoryAggregatorRootMap<IDomainEvent>();
            var aggregate = new AggregatorTest();
            var domainEvents = new List<IDomainEvent>
            {
                new DomainEventTest {Id = Guid.NewGuid(), Version = 1, AggregateId = Guid.NewGuid()}
            };
            aggregate.LoadFromHistory(domainEvents);
            rootMap.Add(aggregate);
            aggregate = new AggregatorTest();
            aggregate.LoadFromHistory(domainEvents);
            Assert.Throws<ArgumentException>(() => rootMap.Add(aggregate));
        }
    }
}