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

using Straight.Core.EventStore;
using System;

namespace Straight.Core.Tests.Common.EventStore
{
    public class DomainEventTest : IDomainEvent
    {
        public Guid AggregateId { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }
    }

    public class DomainEventTest2 : IDomainEvent
    {
        public Guid AggregateId { get; set; }

        public Guid Id { get; set; }

        public int Version { get; set; }
    }
}