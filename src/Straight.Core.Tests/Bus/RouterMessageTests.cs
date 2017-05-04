using Xunit;
using Straight.Core.Bus;
using Straight.Core.Exceptions;
using Straight.Core.Tests.Common.Bus;
using System;

namespace Straight.Core.Tests.Bus
{
    
    public class RouterMessageTests
    {
        
        public RouterMessageTests()
        {
            router = new RouterMessage();
        }

        private RouterMessage router;

        [Fact]
        public void Should_does_not_throw_exception_when_no_route_is_register()
        {
            void Route(MessageTest test) => Console.WriteLine($"{test.Transmitter} raise : {test.Message}");
            router.Register((Action<MessageTest>) Route);
        }

        [Fact]
        public void Should_execute_route_when_route_a_message()
        {
            var actual = string.Empty;
            void Route(MessageTest test) => actual = $"{test.Transmitter} raise : {test.Message}";
            router.Register((Action<MessageTest>) Route);
            router.Route(new MessageTest {Transmitter = "John Doe", Message = "he says \"Hello World\""});
            Assert.Equal(actual, "John Doe raise : he says \"Hello World\"");
        }

        [Fact]
        public void Should_throw_exception_when_message_is_not_routing()
        {
            Assert.Throws<NotRegisteredRouteException>(() =>
                router.Route(new MessageTest
                {
                    Transmitter = "John Doe",
                    Message = "he says \"The route is not defined\""
                }));
        }
    }
}