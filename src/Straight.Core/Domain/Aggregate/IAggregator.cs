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

using Straight.Core.Domain;
using Straight.Core.EventStore;
using System.Collections.Generic;

namespace Straight.Core.Domain.Aggregate
{
    public interface IDomainEventChangeable<out TDomainEvent> : IVersionableUpdatable, IIdentifiable
    {
        IEnumerable<TDomainEvent> GetChanges();

        void Clear();
    }

    public interface IAggregator<TDomainEvent> : IDomainEventChangeable<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        void Reset();

        void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents);

        void Update<TDomainCommand>(TDomainCommand command) where TDomainCommand : class, IDomainCommand;
    }
}