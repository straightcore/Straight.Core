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

using Straight.Core.EventStore;
using System.Collections.Generic;

namespace Straight.Core.Domain
{
    public interface IReadModel<TDomainEvent> : IVersionable, IIdentifiable
        where TDomainEvent : IDomainEvent
    {
        IEnumerable<TDomainEvent> Events { get; }

        void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents);

        void Update(TDomainEvent domainEvent);
    }
}