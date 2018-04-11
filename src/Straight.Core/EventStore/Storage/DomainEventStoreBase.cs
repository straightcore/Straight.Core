// ==============================================================================================================
// Straight Compagny
// Straight Core
// ==============================================================================================================
// ©2018 Straight Compagny. All rights reserved.
// Licensed under the MIT License (MIT); you may not use this file except in compliance
// with the License. You may obtain have a last condition or last licence at https://github.com/straightcore/Straight.Core/blob/master
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

using Straight.Core.EventStore.Aggregate;
using Straight.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.EventStore.Storage
{
    public abstract class DomainEventStoreBase<TDomainEvent> : IDomainEventStorage<TDomainEvent>, IDisposable
        where TDomainEvent : IDomainEvent
    {
        protected bool _disposed;
        private bool _isRunningWithinTransaction;

        public void Dispose()
        {
            if (_disposed)
                return;
            Dispose(true);
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        public void BeginTransaction()
        {
            CheckDisposed();
            if (_isRunningWithinTransaction)
                throw new TransactionException();
            _isRunningWithinTransaction = true;
            BeginTransactionOverride();
        }

        public void Commit()
        {
            CheckDisposed();
            CheckIsTransactionRunning();
            _isRunningWithinTransaction = false;
            CommitOverride();
        }

        public void Rollback()
        {
            CheckDisposed();
            _isRunningWithinTransaction = false;
            RollbackOverride();
        }

        public IEnumerable<TDomainEvent> Get(Guid aggregateId)
        {
            CheckDisposed();
            return GetOverride(aggregateId);
        }

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

        protected abstract void BeginTransactionOverride();

        protected abstract void CommitOverride();

        protected abstract void RollbackOverride();

        protected abstract IEnumerable<TDomainEvent> GetOverride(Guid aggregateId);

        protected abstract void SaveOverride(IDomainEventChangeable<TDomainEvent> aggregator);

        protected abstract int GetVersionAggregator(IDomainEventChangeable<TDomainEvent> aggregator);

        private static int GetVersion(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            return aggregator.GetChanges().Select(ev => (int?) ev.Version).LastOrDefault() ?? 0;
        }

        private void CheckIsTransactionRunning()
        {
            if (!_isRunningWithinTransaction)
                throw new TransactionException("Opperation is not running within a transaction");
        }

        private void CheckDisposed()
        {
            if (!_disposed)
                return;
            throw new ObjectDisposedException("InMemoryDomainEventStore", "it is disposed.");
        }

        protected abstract void Dispose(bool disposing);
    }
}