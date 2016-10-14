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

using System;
using System.Collections.Immutable;

namespace Straight.Core.Bus
{
    public class InMemoryActionQueue : IActionQueue
    {
        private ImmutableQueue<object> _itemQueue;
        private ImmutableQueue<Action<object>> _listenerQueue;

        public InMemoryActionQueue()
        {
            _itemQueue = ImmutableQueue<object>.Empty;
            _listenerQueue = ImmutableQueue<Action<object>>.Empty;
        }

        public void Put(object item)
        {
            if (_listenerQueue.IsEmpty)
            {
                _itemQueue = _itemQueue.Enqueue(item);
                return;
            }
            Action<object> action;
            _listenerQueue = _listenerQueue.Dequeue(out action);
            action?.Invoke(item);
        }

        public void Pop(Action<object> popAction)
        {
            if (_itemQueue.IsEmpty)
            {
                _listenerQueue = _listenerQueue.Enqueue(popAction);
                return;
            }
            object item;
            _itemQueue = _itemQueue.Dequeue(out item);
            popAction(item);
        }
    }
}