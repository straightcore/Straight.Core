using Straight.Core.Bus;
using Straight.Core.Storage;
using System;
using System.Collections.Generic;
using NSubstitute;
using Straight.Core.Tests.Common.Bus;
using NUnit.Framework;

namespace Straight.Core.Tests.Bus
{
    [TestFixture]
    public class InMemoryBusTests
    {

        private class BusContext
        {
            public BusContext()
            {
                CommitQueue = new Queue<Action<object>>();
                ActionQueue = Substitute.For<IActionQueue>();
                ActionQueue.When(q => q.Pop(Arg.Any<Action<object>>()))
                    .Do(info => CommitQueue.Enqueue(info.Arg<Action<object>>()));
                ActionQueue.When(q => q.Put(Arg.Any<object>()))
                    .Do(info => CommitQueue.Dequeue()(info.Arg<object>()));
                Router = Substitute.For<IRouterMessage>();
                Bus = new InMemoryBus(Router, ActionQueue);
                ActionQueue.ClearReceivedCalls();
            }

            public Queue<Action<object>> CommitQueue { get; }
            public InMemoryBus Bus { get; }
            public IActionQueue ActionQueue { get; }
            public IRouterMessage Router { get; }
        }

        [Test]
        public void Should_call_route_message_when_commit_bus()
        {
            var context = new BusContext();
            context.Bus.Publish(new MessageTest {Transmitter = "John Doe", Message = "New Message"});
            context.Bus.Commit();
            context.Router.Received(1).Route(Arg.Any<object>());
            context.ActionQueue.Received(1).Pop(Arg.Any<Action<object>>());
            context.ActionQueue.Received(1).Put(Arg.Any<object>());
        }

        [Test]
        public void Should_call_route_messages_when_commit_bus()
        {
            var context = new BusContext();
            var messageTests = new[]
            {
                new MessageTest {Transmitter = "John Doe", Message = "New Message"},
                new MessageTest {Transmitter = "Jane Doe", Message = "Woman Message"}
            };
            context.Bus.Publish(messageTests);
            context.Bus.Commit();
            context.Router.Received(2).Route(Arg.Any<object>());
            context.ActionQueue.Received(2).Pop(Arg.Any<Action<object>>());
            context.ActionQueue.Received(2).Put(Arg.Any<object>());
        }

        [Test]
        public void Should_does_not_call_route_when_rollback_bus()
        {
            var context = new BusContext();
            context.Bus.Publish(new MessageTest {Transmitter = "John Doe", Message = "New Message"});
            context.Bus.Rollback();
            context.Bus.Commit();
            context.Router.Received(0).Route(Arg.Any<object>());
            context.ActionQueue.Received(0).Pop(Arg.Any<Action<object>>());
            context.ActionQueue.Received(0).Put(Arg.Any<object>());
        }

        [Test]
        public void Should_does_not_throw_exception_when_publish_message()
        {
            var context = new BusContext();
            context.Bus.Publish(new MessageTest {Transmitter = "John Doe", Message = "New Message"});
            context.Router.Received(0).Route(Arg.Any<object>());
            context.ActionQueue.Received(0).Pop(Arg.Any<Action<object>>());
            context.ActionQueue.Received(0).Put(Arg.Any<object>());
        }
    }
}