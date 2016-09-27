using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Straight.Core.Bus;
using Straight.Core.Storage;
using Straight.Core.Tests.Common.Bus;

namespace Straight.Core.Tests.Bus
{
    [TestFixture]
    public class InMemoryBusTests
    {
        private Queue<Action<object>> commitQueue;
        private InMemoryBus bus;
        private IActionQueue actionQueue;
        private IRouterMessage router;
        
        [SetUp]
        public void Setup()
        {
            commitQueue = new Queue<Action<object>>();
            actionQueue = Substitute.For<IActionQueue>();
            actionQueue.When(q => q.Pop(Arg.Any<Action<object>>()))
                .Do(info => commitQueue.Enqueue(info.Arg<Action<object>>()));
            actionQueue.When(q => q.Put(Arg.Any<object>()))
                       .Do(info => commitQueue.Dequeue()(info.Arg<object>()));
            router = Substitute.For<IRouterMessage>();
            bus = new InMemoryBus(router, actionQueue);
            actionQueue.ClearReceivedCalls();
        }


        [Test]
        public void Should_does_not_throw_exception_when_publish_message()
        {
            Assert.DoesNotThrow(() => bus.Publish(new MessageTest() { Transmitter = "John Doe", Message = "New Message" }));
            router.Received(0).Route(Arg.Any<object>());
            actionQueue.Received(0).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(0).Put(Arg.Any<object>());
        }

        [Test]
        public void Should_call_route_message_when_commit_bus()
        {
            bus.Publish(new MessageTest() { Transmitter = "John Doe", Message = "New Message" });
            bus.Commit();
            router.Received(1).Route(Arg.Any<object>());
            actionQueue.Received(1).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(1).Put(Arg.Any<object>());
        }

        [Test]
        public void Should_does_not_call_route_when_rollback_bus()
        {
            bus.Publish(new MessageTest() { Transmitter = "John Doe", Message = "New Message" });
            bus.Rollback();
            bus.Commit();
            router.Received(0).Route(Arg.Any<object>());
            actionQueue.Received(0).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(0).Put(Arg.Any<object>());
        }

        [Test]
        public void Should_call_route_messages_when_commit_bus()
        {
            var messageTests = new []
            {
                new MessageTest() { Transmitter = "John Doe", Message = "New Message" },
                new MessageTest() { Transmitter = "Jane Doe", Message = "Woman Message" }
            };
            bus.Publish(messageTests);
            bus.Commit();
            router.Received(2).Route(Arg.Any<object>());
            actionQueue.Received(2).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(2).Put(Arg.Any<object>());
        }
    }
}
