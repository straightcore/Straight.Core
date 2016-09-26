using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Straight.Core.Bus;
using Straight.Core.Exceptions;

namespace Straight.Core.Tests
{
    [TestFixture]
    public class RouterMessageTests
    {
        private RouterMessage router;
        [SetUp]
        public void Setup()
        {
            router = new RouterMessage();
        }

        [Test]
        public void Should_does_not_throw_exception_when_no_route_is_register()
        {
            Action<MessageTest> route =
                test => Console.WriteLine($"{test.Transmitter} raise : {test.Message}");
            Assert.DoesNotThrow(() => router.Register(route));
        }

        [Test]
        public void Should_execute_route_when_route_a_message()
        {
            var actual = string.Empty;
            Action<MessageTest> route =
                test => actual = $"{test.Transmitter} raise : {test.Message}";
            router.Register(route);
            router.Route(new MessageTest() { Transmitter = "John Doe", Message = "he says \"Hello World\"" });
            Assert.That(actual, Is.EqualTo("John Doe raise : he says \"Hello World\""));
        }


        [Test]
        public void Should_throw_exception_when_message_is_not_routing()
        {
            Assert.Throws<NotRegisteredRouteException>(() => 
                router.Route(new MessageTest() { Transmitter = "John Doe", Message = "he says \"The route is not defined\"" }));
        }
    }

    public class MessageTest
    {
        public string Message { get; set; }
        public string Transmitter { get; set; }
    }
}
