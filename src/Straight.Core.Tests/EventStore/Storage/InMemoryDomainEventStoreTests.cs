using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.EventStore.Storage;
using Straight.Core.Exceptions;
using Straight.Core.Tests.EventStore.Aggregate;

namespace Straight.Core.Tests.EventStore.Storage
{
    [TestFixture]
    public class InMemoryDomainEventStoreTests
    {
        private IDomainEventStorage<IDomainEvent> _storage;

        [SetUp]
        public void SetUp()
        {
            _storage = new InMemoryDomainEventStore<IDomainEvent>();
        }

        [TearDown]
        public void TearDown()
        {
            (_storage as IDisposable)?.Dispose();
        }

        [Test]
        public void Should_save_new_event_when_storage_is_empty()
        {
            IAggregator<IDomainCommand, IDomainEvent> aggregator = new AggregatorTest(() => { });
            aggregator.Handle(new DomainCommandTest() {Id = Guid.NewGuid()});
            var expectedVersion = aggregator.GetChanges().Last().Version;
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            Assert.That(aggregator.Version, Is.EqualTo(expectedVersion));
        }

        [Test]
        public void Should_throw_exception_when_save__without_transaction()
        {
            IAggregator<IDomainCommand, IDomainEvent> aggregator = new AggregatorTest(() => { });
            aggregator.Handle(new DomainCommandTest() { Id = Guid.NewGuid() });
            Assert.Throws<TransactionException>(() => _storage.Save(aggregator));
        }


        [Test]
        public void Should_add_event_in_memory_when_commit_changed()
        {
            IAggregator<IDomainCommand, IDomainEvent> aggregator = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest() { Id = Guid.NewGuid() };
            aggregator.Handle(domainCommandTest);
            var expectedChanged = aggregator.GetChanges();
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Commit();

            Assert.That(_storage.Get(aggregator.Id), Is.EquivalentTo(expectedChanged));
        }

        [Test]
        public void Should_does_have_event_in_memory_when_rollback_changed()
        {
            IAggregator<IDomainCommand, IDomainEvent> aggregator = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest() { Id = Guid.NewGuid() };
            aggregator.Handle(domainCommandTest);
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Rollback();

            Assert.That(_storage.Get(aggregator.Id), Is.Empty.Or.Null);
        }

        [Test]
        public void Should_throw_concurrancy_exception_when_aggregator_version_is_less()
        {
            IAggregator<IDomainCommand, IDomainEvent> aggregator = new AggregatorTest(() => { });
            IAggregator<IDomainCommand, IDomainEvent> aggregatorClone = new AggregatorTest(() => { });
            
            var domainCommandTest = new DomainCommandTest() { Id = Guid.NewGuid() };
            aggregator.Handle(domainCommandTest);
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Commit();
            aggregator.Handle(new DomainCommandTest() { Id = Guid.NewGuid() });
            aggregatorClone.LoadFromHistory(_storage.Get(aggregator.Id));
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Commit();

            _storage.BeginTransaction();
            Assert.Throws<ConcurrencyViolationException>(() => _storage.Save(aggregatorClone));
        }
    }
}