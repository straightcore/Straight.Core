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

        private class InMemoContext
        {
            public InMemoContext()
            {
                InMemoryDomainEventStore = new InMemoryDomainEventStore<IDomainEvent>();
                InMemoryAggregatorRootMap = new InMemoryAggregatorRootMapMock();
                RepositoryUnitOfWorkWithRealInput = new InMemoryDomainEventStoreUnitOfWork<IDomainEvent>(
                    InMemoryAggregatorRootMap,
                    InMemoryDomainEventStore,
                    Substitute.For<IBus<IDomainEvent>>());

                SubstituteAggregatorRootMap = Substitute.For<IAggregatorRootMap<IDomainEvent>>();
                SubstituteDomainEventStore = Substitute.For<IDomainEventStorage<IDomainEvent>>();
                RepositoryUnitOfWorkWithSubstitue = new InMemoryDomainEventStoreUnitOfWork<IDomainEvent>(
                    SubstituteAggregatorRootMap,
                    SubstituteDomainEventStore,
                    Substitute.For<IBus<IDomainEvent>>());
            }

            public InMemoryDomainEventStoreUnitOfWork<IDomainEvent> RepositoryUnitOfWorkWithSubstitue { get; }
            public IAggregatorRootMap<IDomainEvent> SubstituteAggregatorRootMap { get; }
            public IDomainEventStorage<IDomainEvent> SubstituteDomainEventStore { get; }

            public InMemoryDomainEventStoreUnitOfWork<IDomainEvent> RepositoryUnitOfWorkWithRealInput { get; }
            public InMemoryDomainEventStore<IDomainEvent> InMemoryDomainEventStore { get; }
            public InMemoryAggregatorRootMapMock InMemoryAggregatorRootMap { get; }
        }

        private void InsertInAggregateRootMap(InMemoContext context, AggregatorTest expected)
        {
            context.InMemoryAggregatorRootMap.Add(expected);
        }

        private void InsertInEventStore(InMemoContext context, AggregatorTest aggregate)
        {
            context.InMemoryDomainEventStore.BeginTransaction();
            context.InMemoryDomainEventStore.Save(aggregate);
            context.InMemoryDomainEventStore.Commit();
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
            var context = new InMemoContext();
            var expected = GenerateAggregate();
            context.RepositoryUnitOfWorkWithRealInput.Add(expected);
            var actual = context.RepositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Version, Is.EqualTo(expected.Version));
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Should_does_not_get_when_rollback_data()
        {
            var context = new InMemoContext();
            var expected = GenerateAggregate();
            context.RepositoryUnitOfWorkWithRealInput.Add(expected);
            context.RepositoryUnitOfWorkWithRealInput.Rollback();
            var actual = context.RepositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Should_get_aggregate_when_aggregate_is_in_identitymap()
        {
            var context = new InMemoContext();
            var expected = GenerateAggregate();
            InsertInAggregateRootMap(context, expected);
            var actual = context.RepositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual, Is.EqualTo(expected));
            Assert.That(ReferenceEquals(actual, expected), Is.True);
        }
        
        [Test]
        public void Should_get_aggregate_when_aggregate_is_in_storage_repository()
        {
            var context = new InMemoContext();
            var expected = GenerateAggregate();
            InsertInEventStore(context, expected);
            var actual = context.RepositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(expected.Id);
            Assert.That(actual.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.Version, Is.EqualTo(expected.Version));
        }

        [Test]
        public void Should_get_new_aggregate_when_aggregate_does_not_exist()
        {
            var context = new InMemoContext();
            var aggregate = context.RepositoryUnitOfWorkWithRealInput.GetById<AggregatorTest>(Guid.NewGuid());
            Assert.That(aggregate, Is.Null);
        }

        [Test]
        public void Should_received_call_clear_in_identity_root_map_when_rollback()
        {
            var context = new InMemoContext();
            var actual = GenerateAggregate();
            context.RepositoryUnitOfWorkWithSubstitue.Add(actual);
            context.RepositoryUnitOfWorkWithSubstitue.Rollback();
            context.SubstituteAggregatorRootMap.Received(1).Clear();
        }

        [Test]
        public void Should_received_call_in_identity_root_map_when_add_new_aggregate()
        {
            var context = new InMemoContext();
            var actual = GenerateAggregate();
            context.RepositoryUnitOfWorkWithSubstitue.Add(actual);
            context.SubstituteAggregatorRootMap.Received(1).Add(Arg.Any<AggregatorTest>());
        }

        [Test]
        public void Should_received_call_in_repository_vent_store_when_commit_new_aggregate()
        {
            var context = new InMemoContext();
            var actual = GenerateAggregate();
            context.RepositoryUnitOfWorkWithSubstitue.Add(actual);
            context.RepositoryUnitOfWorkWithSubstitue.Commit();
            context.SubstituteDomainEventStore.Received(1).BeginTransaction();
            context.SubstituteDomainEventStore.Received(1).Save(Arg.Any<AggregatorTest>());
            context.SubstituteDomainEventStore.Received(1).Commit();
        }
    }
}