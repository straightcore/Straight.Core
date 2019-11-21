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
using Straight.Core.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Straight.Core.Extensions.Domain
{
    public static class DomainCommandHandlerExtensions
    {
        public static IEnumerable<TDomainEvent> Handle<TDomainEvent>(
            this IReadOnlyDictionary<Type, MethodInfo> registerMethods,
            object model,
            object command)
            where TDomainEvent : IDomainEvent
        {
            MethodInfo handler;
            if (!registerMethods.TryGetValue(command.GetType(), out handler))
                throw new UnregisteredDomainEventException(
                    $"The domain command '{command.GetType().FullName}' is not registered in '{model.GetType().FullName}'");
            return ((IEnumerable) handler.Invoke(model, new[] {command}))
                .OfType<TDomainEvent>();
        }
    }
}