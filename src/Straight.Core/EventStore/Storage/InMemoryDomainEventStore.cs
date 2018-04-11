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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.EventStore.Storage
{
    public class InMemoryDomainEventStore<TDomainEvent> : DomainEventStoreBase<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private readonly ConcurrentDictionary<Guid, List<TDomainEvent>> memory
            = new ConcurrentDictionary<Guid, List<TDomainEvent>>();

        private ConcurrentDictionary<Guid, List<TDomainEvent>> _changed;

        protected override void BeginTransactionOverride()
        {
            _changed = new ConcurrentDictionary<Guid, List<TDomainEvent>>();
        }

        protected override void CommitOverride()
        {
            var localChanged = new Dictionary<Guid, List<TDomainEvent>>(_changed);
            _changed = null;
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

        protected override void RollbackOverride()
        {
            _changed = null;
        }

        protected override IEnumerable<TDomainEvent> GetOverride(Guid aggregateId)
        {
            List<TDomainEvent> listEvents;
            return memory.TryGetValue(aggregateId, out listEvents)
                ? listEvents.AsReadOnly()
                : Enumerable.Empty<TDomainEvent>();
        }

        protected override void SaveOverride(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            var eventList = GetListOfEventInChanged(aggregator.Id);
            eventList.AddRange(aggregator.GetChanges());
        }

        protected override int GetVersionAggregator(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            List<TDomainEvent> listOfEvent;
            return memory.TryGetValue(aggregator.Id, out listOfEvent)
                ? listOfEvent.Select(ev => (int?) ev.Version).LastOrDefault() ?? 0
                : 0;
        }

        private List<TDomainEvent> GetListOfEventInChanged(Guid aggregatorId)
        {
            List<TDomainEvent> listOfEvent;
            if (_changed.TryGetValue(aggregatorId, out listOfEvent))
                return listOfEvent.ToList();
            listOfEvent = new List<TDomainEvent>();
            _changed.TryAdd(aggregatorId, listOfEvent);
            return listOfEvent;
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                memory.Clear();
                _changed?.Clear();
            }
            _changed = null;
        }
    }
}