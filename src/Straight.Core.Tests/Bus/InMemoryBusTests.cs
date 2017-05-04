using Xunit;
using Straight.Core.Bus;
using Straight.Core.Storage;
using System;
using System.Collections.Generic;
using NSubstitute;
using Straight.Core.Tests.Common.Bus;

namespace Straight.Core.Tests.Bus
{

    public class InMemoryBusTests
    {
        
        public InMemoryBusTests()
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

        private Queue<Action<object>> commitQueue;
        private InMemoryBus bus;
        private IActionQueue actionQueue;
        private IRouterMessage router;

        [Fact]
        public void Should_call_route_message_when_commit_bus()
        {
            bus.Publish(new MessageTest {Transmitter = "John Doe", Message = "New Message"});
            bus.Commit();
            router.Received(1).Route(Arg.Any<object>());
            actionQueue.Received(1).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(1).Put(Arg.Any<object>());
        }

        [Fact]
        public void Should_call_route_messages_when_commit_bus()
        {
            var messageTests = new[]
            {
                new MessageTest {Transmitter = "John Doe", Message = "New Message"},
                new MessageTest {Transmitter = "Jane Doe", Message = "Woman Message"}
            };
            bus.Publish(messageTests);
            bus.Commit();
            router.Received(2).Route(Arg.Any<object>());
            actionQueue.Received(2).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(2).Put(Arg.Any<object>());
        }

        [Fact]
        public void Should_does_not_call_route_when_rollback_bus()
        {
            bus.Publish(new MessageTest {Transmitter = "John Doe", Message = "New Message"});
            bus.Rollback();
            bus.Commit();
            router.Received(0).Route(Arg.Any<object>());
            actionQueue.Received(0).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(0).Put(Arg.Any<object>());
        }

        [Fact]
        public void Should_does_not_throw_exception_when_publish_message()
        {
            bus.Publish(new MessageTest {Transmitter = "John Doe", Message = "New Message"});
            router.Received(0).Route(Arg.Any<object>());
            actionQueue.Received(0).Pop(Arg.Any<Action<object>>());
            actionQueue.Received(0).Put(Arg.Any<object>());
        }
    }
}