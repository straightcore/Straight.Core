using NSubstitute;
using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Storage.Generic;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using Straight.Core.Tests.Common.EventStore.Storage;
using System;

namespace Straight.Core.Tests.EventStore.Storage
{
    [TestFixture]
    public class InMemoryDomainEventStoreUnitOfWorkTests
    {
        [SetUp]
        public void Setup()
        {
            _inMemoryDomainEventStore = new InMemoryDomainEventStore<IDomainEvent>();
            _inMemoryAggregatorRootMap = new InMemoryAggregatorRootMapMock();
            _repositoryUnitOfWorkWithRealInput = new InMemoryDomainEventStoreUnitOfWork<IDomainEvent>(
                _inMemoryAggregatorRootMap,
                _inMemoryDomainEventStore,
                Substitute.For<IBus<IDomainEvent>>());

            _substituteAggregatorRootMap = Substitute.For<IAggregatorRootMap<IDomainEvent>>();
            _substituteDomainEventStore = Substitute.For<IDomainEventStorage<IDomainEvent>>();
            _repositoryUnitOfWorkWithSubstitue = new InMemoryDomainEventStoreUnitOfWork<IDomainEvent>(
                _substituteAggregatorRootMap,
                _substituteDomainEventStore,
                Substitute.For<IBus<IDomainEvent>>());
        }

        private InMemoryDomainEventStoreUnitOfWork<IDomainEvent> _repositoryUnitOfWorkWithSubstitue;
        private IAggregatorRootMap<IDomainEvent> _substituteAggregatorRootMap;
        private IDomainEventStorage<IDomainEvent> _substituteDomainEventStore;

        private InMemoryDomainEventStoreUnitOfWork<IDomainEvent> _repositoryUnitOfWorkWithRealInput;
        private InMemoryDomainEventStore<IDomainEvent> _inMemoryDomainEventStore;
        private InMemoryAggregatorRootMapMock _inMemoryAggregatorRootMap;

        private void InsertInAggregateRootMap(AggregatorTest expected)
        {
            _inMemoryAggregatorRootMap.Add(expected);
        }

        private void InsertInEventStore(AggregatorTest aggregate)
        {
            _inMemoryDomainEventStore.BeginTransaction();
            _inMemoryDomainEventStore.Save(aggregate);
            _inMemoryDomainEventStore.Commit();
        }

        private static AggregatorTest GenerateAggregate()
        {
            var aggregate = new AggregatorTest();
            aggregate.Update(new DomainCommandTest());
            return aggregate;
        }

        [Test]
        public void Should_can_get_aggregate_when_add_new_aggregate()
        {
            var expected = GenerateAggregate();
            _repositoryUnitOfWorkWithRealInput.Add(expected);
            var actual = _repositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Version, Is.EqualTo(expected.Version));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Should_does_not_get_when_rollback_data()
        {
            var expected = GenerateAggregate();
            _repositoryUnitOfWorkWithRealInput.Add(expected);
            _repositoryUnitOfWorkWithRealInput.Rollback();
            var actual = _repositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Should_get_aggregate_when_aggregate_is_in_identitymap()
        {
            var expected = GenerateAggregate();
            InsertInAggregateRootMap(expected);
            var actual = _repositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(ReferenceEquals(actual, expected));
        }

        [Test]
        public void Should_get_aggregate_when_aggregate_is_in_storage_repository()
        {
            var expected = GenerateAggregate();
            InsertInEventStore(expected);
            var actual = _repositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Version, Is.EqualTo(expected.Version));
        }

        [Test]
        public void Should_get_new_aggregate_when_aggregate_does_not_exist()
        {
            var aggregate = _repositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(aggregate, Is.Null);
        }

        [Test]
        public void Should_received_call_clear_in_identity_root_map_when_rollback()
        {
            var actual = GenerateAggregate();
            _repositoryUnitOfWorkWithSubstitue.Add(actual);
            _repositoryUnitOfWorkWithSubstitue.Rollback();
            _substituteAggregatorRootMap.Received(1).Clear();
        }

        [Test]
        public void Should_received_call_in_identity_root_map_when_add_new_aggregate()
        {
            var actual = GenerateAggregate();
            _repositoryUnitOfWorkWithSubstitue.Add(actual);
            _substituteAggregatorRootMap.Received(1).Add(Arg.Any<AggregatorTest>());
        }

        [Test]
        public void Should_received_call_in_repository_vent_store_when_commit_new_aggregate()
        {
            var actual = GenerateAggregate();
            _repositoryUnitOfWorkWithSubstitue.Add(actual);
            _repositoryUnitOfWorkWithSubstitue.Commit();
            _substituteDomainEventStore.Received(1).BeginTransaction();
            _substituteDomainEventStore.Received(1).Save(Arg.Any<AggregatorTest>());
            _substituteDomainEventStore.Received(1).Commit();
        }
    }
}