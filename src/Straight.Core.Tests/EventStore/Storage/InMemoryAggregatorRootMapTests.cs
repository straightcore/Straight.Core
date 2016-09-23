using NUnit.Framework;
using System;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Tests.EventStore.Aggregate;

namespace Straight.Core.Tests.EventStore.Storage
{
    [TestFixture]
    public class InMemoryAggregatorRootMapTests
    {
        private InMemoryAggregatorRootMap<IDomainCommand, IDomainEvent> rootMap;

        [SetUp]
        public void SetUp()
        {
            rootMap = new InMemoryAggregatorRootMap<IDomainCommand, IDomainEvent>();
        }

        [Test]
        public void Should_return_null_when_model_is_not_found()
        {
            var root = rootMap.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(root, Is.Null);
        }

        [Test]
        public void Should_get_by_id_when_model_is_added()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() { Id = Guid.NewGuid() });
            Assert.DoesNotThrow(() => rootMap.Add(expected));
            var root = rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(root, Is.EqualTo(expected));
        }
        
        [Test]
        public void Should_get_null_value_when_is_not_found_and_one_or_more_aggregator_is_saved()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() { Id = Guid.NewGuid() });
            Assert.DoesNotThrow(() => rootMap.Add(expected));
            var root = rootMap.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(root, Is.Null);
        }

        [Test]
        public void Should_remove_by_type_when_aggregate_is_in_memory()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() { Id = Guid.NewGuid() });
            rootMap.Add(expected);
            expected = rootMap.GetById<AggregatorTest>(expected.Id);
            rootMap.Remove(expected.GetType(), expected.Id);
            var actual = rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Should_remove_when_aggregate_is_in_memory()
        {
            var expected = new AggregatorTest();
            expected.Update(new DomainCommandTest() { Id = Guid.NewGuid() });
            rootMap.Add(expected);
            expected = rootMap.GetById<AggregatorTest>(expected.Id);
            rootMap.Remove<AggregatorTest>(expected.Id);
            var actual = rootMap.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }


    }
}