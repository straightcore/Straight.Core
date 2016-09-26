using Straight.Core.Domain;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.EventStore.Storage
{
    public class InMemoryDomainEventStore<TDomainEvent> : IDomainEventStorage<TDomainEvent>, IDisposable
        where TDomainEvent : IDomainEvent
    {
        private readonly ConcurrentDictionary<Guid, List<TDomainEvent>> memory
            = new ConcurrentDictionary<Guid, List<TDomainEvent>>();

        private bool _disposed = false;
        private ConcurrentDictionary<Guid, List<TDomainEvent>> _changed;
        private bool _isRunningWithinTransaction = false;

        public void BeginTransaction()
        {
            CheckDisposed();
            if (_isRunningWithinTransaction)
            {
                throw new TransactionException();
            }
            _changed = new ConcurrentDictionary<Guid, List<TDomainEvent>>();
            _isRunningWithinTransaction = true;
        }

        public void Commit()
        {
            CheckDisposed();
            CheckIsTransactionRunning();
            var localChanged = new Dictionary<Guid, List<TDomainEvent>>(_changed);
            _changed = null;
            _isRunningWithinTransaction = false;
            foreach (var mappingIdChanged in localChanged)
            {
                List<TDomainEvent> listEvent;
                if (!memory.TryGetValue(mappingIdChanged.Key, out listEvent))
                {
                    memory.TryAdd(mappingIdChanged.Key, mappingIdChanged.Value);
                    return;
                }
                listEvent.AddRange(mappingIdChanged.Value);
            }
        }

        public IEnumerable<TDomainEvent> Get(Guid aggregateId)
        {
            CheckDisposed();
            List<TDomainEvent> listEvents;
            return memory.TryGetValue(aggregateId, out listEvents)
                ? listEvents.AsReadOnly()
                : Enumerable.Empty<TDomainEvent>();
        }

        public void Rollback()
        {
            CheckDisposed();
            _changed = null;
            _isRunningWithinTransaction = false;
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
            var eventList = GetListOfEventInChanged(aggregator.Id);
            eventList.AddRange(aggregator.GetChanges());
            aggregator.UpdateVersion(GetVersion(aggregator));
            aggregator.Clear();
        }

        private void CheckIsTransactionRunning()
        {
            if (!_isRunningWithinTransaction)
            {
                throw new TransactionException("Opperation is not running within a transaction");
            }
        }

        private static int GetVersion(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            return aggregator.GetChanges().Select(ev => (int?)ev.Version).LastOrDefault() ?? 0;
        }

        private List<TDomainEvent> GetListOfEventInChanged(Guid aggregatorId)
        {
            List<TDomainEvent> listOfEvent;
            if (_changed.TryGetValue(aggregatorId, out listOfEvent))
            {
                return listOfEvent.ToList();
            }
            listOfEvent = new List<TDomainEvent>();
            _changed.TryAdd(aggregatorId, listOfEvent);
            return listOfEvent;
        }

        private int GetVersionAggregator(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            List<TDomainEvent> listOfEvent;
            return memory.TryGetValue(aggregator.Id, out listOfEvent)
                ? listOfEvent.Select(ev => (int?)ev.Version).LastOrDefault() ?? 0
                : 0;
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
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            if (disposing)
            {
                memory.Clear();
                _changed?.Clear();
            }
            _changed = null;
            _isRunningWithinTransaction = false;
        }
    }
}