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
using Straight.Core.Storage;
using System;

namespace Straight.Core.EventStore.Storage
{
    public interface IDomainEventStore<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        TAggregate GetById<TAggregate>(Guid id) where TAggregate : class, IAggregator<TDomainEvent>, new();

        void Add<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<TDomainEvent>, new();

    }

    public interface IDomainEventStoreUnitOfWork<TDomainEvent> : IDomainEventStore<TDomainEvent>, IUnitOfWork
        where TDomainEvent : IDomainEvent
    {

        void RegisterForTracking<TAggregate>(TAggregate aggregateRoot) where TAggregate : class, IAggregator<TDomainEvent>, new();
    }
}