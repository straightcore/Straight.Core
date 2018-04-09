using NUnit.Framework;
using Straight.Core.Command;
using Straight.Core.Messaging;
using Straight.Core.Tests.Common.Command;
using System;

namespace Straight.Core.Tests.Messaging
{
    [TestFixture]
    public class CommandHandlerDispatcherTests
    {
        
        private class CommandHandlerTest : ICommandHandler<CommandTest>
        {
            private readonly Action<CommandTest> _action = cmd => { };

            public CommandHandlerTest(Action<CommandTest> action)
            {
                _action = action;
            }

            public void Handle(CommandTest command)
            {
                _action(command);
            }
        }

        private class CommandHandlerTestV2 : ICommandHandler<CommandTest>
        {
            private readonly Action<CommandTest> _action = cmd => { };

            public CommandHandlerTestV2(Action<CommandTest> action)
            {
                _action = action;
            }

            public void Handle(CommandTest command)
            {
                _action(command);
            }
        }

        [Test]
        public void Should_process_when_commandhandler_has_been_register()
        {
            var dispatcher = new CommandHandlerDispatcher();
            var isCalled = false;
            var handler = new CommandHandlerTest(test => isCalled = true);
            dispatcher.Register(handler);
            dispatcher.Process(new CommandTest());
            Assert.True(isCalled);
        }

        [Test]
        public void Should_process_when_multiple_commandhandler_has_been_register()
        {
            var dispatcher = new CommandHandlerDispatcher();
            var isCalledFirstInstance = false;
            var handler = new CommandHandlerTest(test => isCalledFirstInstance = true);
            dispatcher.Register(handler);
            var isCalledSecondInstance = false;
            handler = new CommandHandlerTest(test => isCalledSecondInstance = true);
            dispatcher.Register(handler);

            dispatcher.Process(new CommandTest());
            Assert.True(isCalledFirstInstance);
            Assert.True(isCalledSecondInstance);
        }

        [Test]
        public void Should_process_when_multiple_type_commandhandler_has_been_register()
        {
            var dispatcher = new CommandHandlerDispatcher();
            var isCalledFirstInstance = false;
            var handler = new CommandHandlerTest(test => isCalledFirstInstance = true);
            dispatcher.Register(handler);
            var isCalledSecondInstance = false;
            var handler2 = new CommandHandlerTestV2(test => isCalledSecondInstance = true);
            dispatcher.Register(handler2);

            dispatcher.Process(new CommandTest());
            Assert.True(isCalledFirstInstance);
            Assert.True(isCalledSecondInstance);
        }

        [Test]
        public void Should_throw_argument_exception_when_process_commandtest_not_register()
        {
            var dispatcher = new CommandHandlerDispatcher();
            Assert.Throws<ArgumentOutOfRangeException>(() => dispatcher.Process(new CommandTest()));
        }
    }
}