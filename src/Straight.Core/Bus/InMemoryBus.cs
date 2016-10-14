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

using Straight.Core.Extensions.Collections;
using Straight.Core.Storage;
using System.Collections;
using System.Collections.Immutable;

namespace Straight.Core.Bus
{
    public class InMemoryBus : IBus
    {
        private readonly IActionQueue _actionQueue;
        private readonly IRouterMessage _router;
        private ImmutableQueue<object> _preCommitItems;

        public InMemoryBus(IRouterMessage router, IActionQueue actionQueue)
        {
            _actionQueue = actionQueue;
            _router = router;
            actionQueue.Pop(DoPublishItem);
            _preCommitItems = ImmutableQueue<object>.Empty;
        }

        public void Commit()
        {
            var preCommitItems = _preCommitItems;
            _preCommitItems = ImmutableQueue<object>.Empty;
            if (preCommitItems.IsEmpty)
                return;
            do
            {
                object item;
                preCommitItems = preCommitItems.Dequeue(out item);
                _actionQueue.Put(item);
            } while (preCommitItems != ImmutableQueue<object>.Empty);
        }

        public void Rollback()
        {
            _preCommitItems = ImmutableQueue<object>.Empty;
        }

        public void Publish(object message)
        {
            if (message == null)
                return;
            _preCommitItems = _preCommitItems.Enqueue(message);
        }

        public void Publish(IEnumerable messages)
        {
            messages.ForEach(Publish);
        }

        private void DoPublishItem(object obj)
        {
            _router.Route(obj);
            _actionQueue.Pop(DoPublishItem);
        }
    }
}