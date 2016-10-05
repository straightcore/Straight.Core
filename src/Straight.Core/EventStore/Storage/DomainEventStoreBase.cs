using System;
using System.Collections.Generic;
using System.Linq;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Exceptions;

namespace Straight.Core.EventStore.Storage
{
    public abstract class DomainEventStoreBase<TDomainEvent> : IDomainEventStorage<TDomainEvent>, IDisposable
        where TDomainEvent : IDomainEvent
    {
        protected bool _disposed = false;
        private bool _isRunningWithinTransaction = false;

        public void BeginTransaction()
        {
            CheckDisposed();
            if (_isRunningWithinTransaction)
            {
                throw new TransactionException();
            }
            _isRunningWithinTransaction = true;
            BeginTransactionOverride();
        }

        protected abstract void BeginTransactionOverride();

        public void Commit()
        {
            CheckDisposed();
            CheckIsTransactionRunning();
            _isRunningWithinTransaction = false;
            CommitOverride();
        }

        protected abstract void CommitOverride();

        public void Rollback()
        {
            CheckDisposed();
            _isRunningWithinTransaction = false;
            RollbackOverride();
        }

        protected abstract void RollbackOverride();

        public IEnumerable<TDomainEvent> Get(Guid aggregateId)
        {
            CheckDisposed();
            return GetOverride(aggregateId);
        }

        protected abstract IEnumerable<TDomainEvent> GetOverride(Guid aggregateId);

        public void Save(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            CheckDisposed();
            CheckIsTransactionRunning();
            var version = GetVersionAggregator(aggregator);
            if (version != aggregator.Version)
            {
                throw new ViolationConcurrencyException();
            }
            SaveOverride(aggregator);
            aggregator.UpdateVersion(GetVersion(aggregator));
            aggregator.Clear();
        }

        protected abstract void SaveOverride(IDomainEventChangeable<TDomainEvent> aggregator);

        protected abstract int GetVersionAggregator(IDomainEventChangeable<TDomainEvent> aggregator);

        private static int GetVersion(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            return aggregator.GetChanges().Select(ev => (int?)ev.Version).LastOrDefault() ?? 0;
        }

        private void CheckIsTransactionRunning()
        {
            if (!_isRunningWithinTransaction)
            {
                throw new TransactionException("Opperation is not running within a transaction");
            }
        }

        private void CheckDisposed()
        {
            if (!_disposed)
            {
                return;
            }
            throw new ObjectDisposedException("InMemoryDomainEventStore", "it is disposed.");
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            Dispose(true);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);
    }
}