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
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Domain;
using Straight.Core.Extensions.EventStore;
using Straight.Core.Extensions.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Straight.Core.Extensions.Guard;

namespace Straight.Core.EventStore.Aggregate
{
    public abstract class AggregatorBase<TDomainEvent> : IAggregator<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private const string ApplyMethodName = "Apply";
        private const string HandleMethodName = "Handle";

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> RegisterApplyMethodsByType
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();

        private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> RegisterHandleMethodsByType
            = new ConcurrentDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>>();

        private readonly IReadOnlyDictionary<Type, MethodInfo> _registerMethods;
        private readonly List<TDomainEvent> _appliedEvents;
        private readonly List<TDomainEvent> _changedEvents;

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }
        public int EventVersion { get; protected set; }

        protected AggregatorBase()
        {
            Id = Guid.NewGuid();
            _registerMethods = GetRegisterByType(RegisterApplyMethodsByType, typeof(IApplyEvent<>), ApplyMethodName)
                .Union(GetRegisterByType(RegisterHandleMethodsByType, typeof(IHandlerDomainCommand<>), HandleMethodName))
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            _appliedEvents = new List<TDomainEvent>();
            _changedEvents = new List<TDomainEvent>();
        }

        public void LoadFromHistory(IEnumerable<TDomainEvent> domainEvents)
        {
            Reset();
            var events = domainEvents as IList<TDomainEvent> ?? domainEvents.ToList();
            if (!events.Any())
            {
                return;
            }
            events.ForEach(ev => _registerMethods.Apply(this, ev));
            _appliedEvents.AddRange(events);
            Version = events.Last().Version;
            Id = events.Last().AggregateId;
            EventVersion = Version;
        }

        public IEnumerable<TDomainEvent> GetChanges()
        {
            return _changedEvents.ToList();
        }

        public void Clear()
        {
            _changedEvents.Clear();
        }

        public void Reset()
        {
            Id = Guid.Empty;
            Version = 0;
            EventVersion = 0;
            _appliedEvents.Clear();
            _changedEvents.Clear();
        }

        public void UpdateVersion(int version)
        {
            Version = version;
        }

        private int GetNewEventVersion()
        {
            return ++EventVersion;
        }

        public void Update<TDomainCommand>(TDomainCommand command) where TDomainCommand : class, IDomainCommand
        {
            command.CheckIfArgumentIsNull("command");
            _changedEvents.AddRange(_registerMethods.Handle<TDomainEvent>(this, command)
                                                    .Select(ExecuteApply));
        }

        private TDomainEvent ExecuteApply(TDomainEvent @event)
        {
            @event.CheckIfArgumentIsNull("event");
            @event.AggregateId = Id;
            @event.Version = GetNewEventVersion();
            _registerMethods.Apply(this, @event);
            _appliedEvents.Add(@event);
            return @event;
        }

        private IReadOnlyDictionary<Type, MethodInfo> GetRegisterByType(
            IDictionary<Type, IReadOnlyDictionary<Type, MethodInfo>> register,
            Type typeOfInterfaceBase,
            string methodName)
        {
            IReadOnlyDictionary<Type, MethodInfo> referentiel;
            if (!register.TryGetValue(GetType(), out referentiel))
            {
                register[GetType()] = referentiel = MappingTypeToMethodHelper.ToMappingTypeMethod(
                    GetType(),
                    typeof(TDomainEvent),
                    typeOfInterfaceBase,
                    methodName);
            }
            return referentiel;
        }
    }
}