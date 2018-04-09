using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.EventStore.Storage;
using Straight.Core.Exceptions;
using System;
using System.Linq;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;

namespace Straight.Core.Tests.EventStore.Storage
{
    [TestFixture]
    public class InMemoryDomainEventStoreTests
    {
        [Test]
        public void Should_add_event_in_memory_when_commit_changed()
        {
            var storage = new InMemoryDomainEventStore<IDomainEvent>();
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            aggregator.Update(domainCommandTest);
            var expectedChanged = aggregator.GetChanges();
            storage.BeginTransaction();
            storage.Save(aggregator);
            storage.Commit();

            Assert.That(storage.Get(aggregator.Id), Is.EqualTo(expectedChanged));
        }

        [Test]
        public void Should_does_not_have_event_in_memory_when_rollback_changed()
        {
            var storage = new InMemoryDomainEventStore<IDomainEvent>();
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            aggregator.Update(domainCommandTest);
            storage.BeginTransaction();
            storage.Save(aggregator);
            storage.Rollback();

            var domainEvents = storage.Get(aggregator.Id);
            Assert.That(domainEvents, Is.Not.Null.And.Empty);
        }

        [Test]
        public void Should_save_new_event_when_storage_is_empty()
        {
            var storage = new InMemoryDomainEventStore<IDomainEvent>();
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });
            aggregator.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            var expectedVersion = aggregator.GetChanges().Last().Version;
            storage.BeginTransaction();
            storage.Save(aggregator);
            Assert.That(aggregator.Version, Is.EqualTo(expectedVersion));
        }

        [Test]
        public void Should_throw_concurrancy_exception_when_aggregator_version_is_less()
        {
            var storage = new InMemoryDomainEventStore<IDomainEvent>();
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });
            IAggregator<IDomainEvent> aggregatorClone = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            aggregator.Update(domainCommandTest);
            storage.BeginTransaction();
            storage.Save(aggregator);
            storage.Commit();
            aggregator.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            aggregatorClone.LoadFromHistory(storage.Get(aggregator.Id));
            storage.BeginTransaction();
            storage.Save(aggregator);
            storage.Commit();

            storage.BeginTransaction();
            Assert.Throws<ViolationConcurrencyException>(() => storage.Save(aggregatorClone));
        }

        [Test]
        public void Should_throw_exception_when_save__without_transaction()
        {
            var storage = new InMemoryDomainEventStore<IDomainEvent>();
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });
            aggregator.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            Assert.Throws<TransactionException>(() => storage.Save(aggregator));
        }
    }
}