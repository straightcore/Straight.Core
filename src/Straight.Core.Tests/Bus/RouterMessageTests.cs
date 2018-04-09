using NUnit.Framework;
using Straight.Core.Bus;
using Straight.Core.Exceptions;
using Straight.Core.Tests.Common.Bus;
using System;

namespace Straight.Core.Tests.Bus
{
    [TestFixture]
    public class RouterMessageTests
    {
        [Test]
        public void Should_does_not_throw_exception_when_no_route_is_register()
        {
            var router = new RouterMessage();
            void Route(MessageTest test) => Console.WriteLine($"{test.Transmitter} raise : {test.Message}");
            router.Register((Action<MessageTest>) Route);
        }

        [Test]
        public void Should_execute_route_when_route_a_message()
        {
            var router = new RouterMessage();
            var actual = string.Empty;
            void Route(MessageTest test) => actual = $"{test.Transmitter} raise : {test.Message}";
            router.Register((Action<MessageTest>) Route);
            router.Route(new MessageTest {Transmitter = "John Doe", Message = "he says \"Hello World\""});
            Assert.That(actual, Is.EqualTo("John Doe raise : he says \"Hello World\""));
        }

        [Test]
        public void Should_throw_exception_when_message_is_not_routing()
        {
            var router = new RouterMessage();
            Assert.Throws<NotRegisteredRouteException>(() =>
                router.Route(new MessageTest
                {
                    Transmitter = "John Doe",
                    Message = "he says \"The route is not defined\""
                }));
        }
    }
}