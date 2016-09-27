// ==============================================================================================================
// Straight Compagny
// Straight Core
// ==============================================================================================================
// ©2016 Straight Compagny. All rights reserved.
// Licensed under the MIT License (MIT); you may not use this file except in compliance
// with the License. You may obtain have a last condition or last licence at https://github.com/straightcore/Straight.Core/blob/master
// Unless required by applicable law or agreed to in writing, software distributed under the License is
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

using Straight.Core.Domain;
using Straight.Core.EventStore.Aggregate;
using System;
using System.Collections.Generic;

namespace Straight.Core.EventStore.Storage
{
    public class InMemoryAggregatorRootMap<TDomainEvent> :
            IAggregatorRootMap<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private readonly Dictionary<Type, Dictionary<Guid, IAggregator<TDomainEvent>>> _mapping
            = new Dictionary<Type, Dictionary<Guid, IAggregator<TDomainEvent>>>();

        public TAggregate GetById<TAggregate>(Guid id)
            where TAggregate : class, IAggregator<TDomainEvent>, new()
        {
            Dictionary<Guid, IAggregator<TDomainEvent>> mappingAggreg;
            if (!_mapping.TryGetValue(typeof(TAggregate), out mappingAggreg))
            {
                return null;
            }
            IAggregator<TDomainEvent> localAggregator;
            return mappingAggreg.TryGetValue(id, out localAggregator) ? localAggregator as TAggregate : null;
        }

        public void Add<TAggregate>(TAggregate aggregateRoot)
            where TAggregate : class, IAggregator<TDomainEvent>, new()
        {
            Dictionary<Guid, IAggregator<TDomainEvent>> mappingAggreg;
            if (!_mapping.TryGetValue(aggregateRoot.GetType(), out mappingAggreg))
            {
                mappingAggreg = new Dictionary<Guid, IAggregator<TDomainEvent>>();
                _mapping.Add(aggregateRoot.GetType(), mappingAggreg);
            }
            mappingAggreg.Add(aggregateRoot.Id, aggregateRoot);
        }

        public void Remove<TAggregate>(Guid aggregateRootId)
            where TAggregate : class, IAggregator<TDomainEvent>, new()
        {
            Remove(typeof(TAggregate), aggregateRootId);
        }

        public void Remove(Type aggregateRootType, Guid aggregateRootId)
        {
            Dictionary<Guid, IAggregator<TDomainEvent>> mappingAggreg;
            if (!_mapping.TryGetValue(aggregateRootType, out mappingAggreg))
            {
                return;
            }
            mappingAggreg.Remove(aggregateRootId);
        }

        public void Clear()
        {
            _mapping.Clear();
        }
    }
}