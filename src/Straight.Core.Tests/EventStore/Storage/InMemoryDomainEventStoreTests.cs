using Xunit;
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
    
    public class InMemoryDomainEventStoreTests
    {
        
        public InMemoryDomainEventStoreTests()
        {
            _storage = new InMemoryDomainEventStore<IDomainEvent>();
        }
        
        public void TearDown()
        {
            (_storage as IDisposable)?.Dispose();
        }

        private IDomainEventStorage<IDomainEvent> _storage;

        [Fact]
        public void Should_add_event_in_memory_when_commit_changed()
        {
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            aggregator.Update(domainCommandTest);
            var expectedChanged = aggregator.GetChanges();
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Commit();

            Assert.Equal(_storage.Get(aggregator.Id), expectedChanged);
        }

        [Fact]
        public void Should_does_not_have_event_in_memory_when_rollback_changed()
        {
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            aggregator.Update(domainCommandTest);
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Rollback();

            var domainEvents = _storage.Get(aggregator.Id);
            Assert.Empty(domainEvents);
        }

        [Fact]
        public void Should_save_new_event_when_storage_is_empty()
        {
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });
            aggregator.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            var expectedVersion = aggregator.GetChanges().Last().Version;
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            Assert.Equal(aggregator.Version, expectedVersion);
        }

        [Fact]
        public void Should_throw_concurrancy_exception_when_aggregator_version_is_less()
        {
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });
            IAggregator<IDomainEvent> aggregatorClone = new AggregatorTest(() => { });

            var domainCommandTest = new DomainCommandTest {Id = Guid.NewGuid()};
            aggregator.Update(domainCommandTest);
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Commit();
            aggregator.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            aggregatorClone.LoadFromHistory(_storage.Get(aggregator.Id));
            _storage.BeginTransaction();
            _storage.Save(aggregator);
            _storage.Commit();

            _storage.BeginTransaction();
            Assert.Throws<ViolationConcurrencyException>(() => _storage.Save(aggregatorClone));
        }

        [Fact]
        public void Should_throw_exception_when_save__without_transaction()
        {
            IAggregator<IDomainEvent> aggregator = new AggregatorTest(() => { });
            aggregator.Update(new DomainCommandTest {Id = Guid.NewGuid()});
            Assert.Throws<TransactionException>(() => _storage.Save(aggregator));
        }
    }
}