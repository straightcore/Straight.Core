using NUnit.Framework;
using Straight.Core.Messaging;
using Straight.Core.Tests.Common.EventStore;
using System;

namespace Straight.Core.Tests.Messaging
{
    [TestFixture]
    public class EventHandlerDispatcherTests
    {
        [SetUp]
        public void Setup()
        {
            _dispatcher = new EventHandlerDispatcher();
        }

        private EventHandlerDispatcher _dispatcher;

        [Test]
        public void Should_call_all_methods_when_register_and_process_events()
        {
            var isCalledEvent1 = false;
            var isCalledEvent2 = false;
            _dispatcher.Register(new EventHandlerTest(() => isCalledEvent1 = true, () => isCalledEvent2 = true));
            _dispatcher.Process(new DomainEventTest());
            _dispatcher.Process(new DomainEventTest2());

            Assert.That(isCalledEvent1);
            Assert.That(isCalledEvent2);
        }

        [Test]
        public void Should_call_method_when_register_and_process_event()
        {
            var isCalledEvent1 = false;
            _dispatcher.Register(new EventHandlerTest(() => isCalledEvent1 = true));
            _dispatcher.Process(new DomainEventTest());
            Assert.That(isCalledEvent1);
        }

        [Test]
        public void Should_handle_event_when_multiple_type_handler_for_an_event()
        {
            var isCalledEvent1 = false;
            _dispatcher.Register(new EventHandlerTest(() => isCalledEvent1 = true));
            var isCalledHandler2 = false;
            _dispatcher.Register(new EventHandlerTest2 {ActionDomainEventTest = () => isCalledHandler2 = true});
            _dispatcher.Process(new DomainEventTest());
            Assert.That(isCalledEvent1);
            Assert.That(isCalledHandler2);
        }

        [Test]
        public void Should_throw_argument_exception_when_process_event_unknow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _dispatcher.Process(new DomainEventTest()));
        }
    }

    public class EventHandlerTest2 :
        IEventHandler<DomainEventTest>
    {
        public Action ActionDomainEventTest { get; set; } = () => { };

        public void Handle(DomainEventTest @event)
        {
            ActionDomainEventTest();
        }
    }

    public class EventHandlerTest :
        IEventHandler<DomainEventTest>
        , IEventHandler<DomainEventTest2>
    {
        private readonly Action _eventHandlerTest;
        private readonly Action _eventHandlerTest2;

        public EventHandlerTest(Action eventHandlerTest = null, Action eventHandlerTest2 = null)
        {
            _eventHandlerTest = eventHandlerTest ?? (() => { });
            _eventHandlerTest2 = eventHandlerTest2 ?? (() => { });
        }

        public void Handle(DomainEventTest @event)
        {
            _eventHandlerTest();
        }

        public void Handle(DomainEventTest2 @event)
        {
            _eventHandlerTest2();
        }
    }
}