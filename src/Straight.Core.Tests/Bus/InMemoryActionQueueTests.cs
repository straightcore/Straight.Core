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


using System;
using Straight.Core.Bus;
using NUnit.Framework;

namespace Straight.Core.Tests.Bus
{
    [TestFixture]
    public class InMemoryActionQueueTests
    {
        
        [Test]
        public void Should_call_action_when_pop_action_after_put_item()
        {
            var queue = new InMemoryActionQueue();
            var isCalled = false;
            queue.Put(new object());
            queue.Pop(obj => isCalled = true);
            Assert.True(isCalled);
        }

        [Test]
        public void Should_does_not_throw_exception_when_pop_new_action_in_nominal_case()
        {
            var queue = new InMemoryActionQueue();
            queue.Pop(o => { });
        }

        [Test]
        public void Should_does_not_throw_exception_when_put_new_item_without_action()
        {
            var queue = new InMemoryActionQueue();
            queue.Put(new object());
        }

        [Test]
        public void Should_execute_action_when_pop_action_and_put_object()
        {
            var queue = new InMemoryActionQueue();
            var isCalled = false;
            queue.Pop(obj => isCalled = true);
            queue.Put(new object());
            Assert.True(isCalled);
        }

        [Test]
        public void Should_not_call_action_when_not_put_item()
        {
            var queue = new InMemoryActionQueue();
            var isCalled = false;
            queue.Pop(obj => isCalled = true);
            Assert.False(isCalled);
        }

        [Test]
        public void Should_not_call_action_when_put_item_without_action()
        {
            var queue = new InMemoryActionQueue();
            var isCalled = false;
            queue.Put(new object());
            Assert.False(isCalled);
        }
    }
}