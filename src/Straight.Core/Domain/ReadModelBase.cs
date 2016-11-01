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
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Domain;
using Straight.Core.Extensions.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Straight.Core.Domain
{
    public abstract class ReadModelBase<TDomainEvent> : IReadModel<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private const string ApplyMethodName = "Apply";

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>
            RegisterApplyMethodsByType
                = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();

        private readonly List<TDomainEvent> _appliedEvents;
        private readonly IReadOnlyDictionary<Type, MethodInfo> _registerMethods;

        protected ReadModelBase()
        {
            _registerMethods = GetRegisterByType(typeof(IApplyEvent<>), ApplyMethodName);
            _appliedEvents = new List<TDomainEvent>();
        }

        public Guid Id { get; private set; }

        public int Version { get; private set; }

        public IEnumerable<TDomainEvent> Events => _appliedEvents.AsReadOnly();

        public void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents)
        {
            _appliedEvents.Clear();
            Id = Guid.Empty;
            Version = 0;
            domainEvents.ForEach(Update);
        }

        public void Update(TDomainEvent domainEvent)
        {
            Id = domainEvent.AggregateId;
            Version = domainEvent.Version;
            _registerMethods.Apply(this, domainEvent);
            _appliedEvents.Add(domainEvent);
        }

        private IReadOnlyDictionary<Type, MethodInfo> GetRegisterByType(Type typeOfInterfaceBase, string methodName)
        {
            IReadOnlyDictionary<Type, MethodInfo> referentiel;
            if (!RegisterApplyMethodsByType.TryGetValue(GetType(), out referentiel))
                RegisterApplyMethodsByType[GetType()] = referentiel = MappingTypeToMethodHelper.ToMappingTypeMethod(
                    GetType(),
                    typeof(TDomainEvent),
                    typeOfInterfaceBase,
                    methodName);
            return referentiel;
        }
    }
}