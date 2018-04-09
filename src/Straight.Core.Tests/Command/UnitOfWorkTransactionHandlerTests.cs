using NSubstitute;
using Straight.Core.Command;
using Straight.Core.Storage;
using System;
using NSubstitute.Core;
using Straight.Core.Common.Command;
using NUnit.Framework;

namespace Straight.Core.Tests.Command
{
    [TestFixture]
    public class UnitOfWorkTransactionHandlerTests
    {
        private class UOWContext
        {

            public UnitOfWorkTransactionHandler<ICommand, ICommandHandler<ICommand>> TransactionHandler { get; }
            public IUnitOfWork UnitOfWork { get; }


            public UOWContext()
            {
                UnitOfWork = Substitute.For<IUnitOfWork>();
                TransactionHandler = new UnitOfWorkTransactionHandler<ICommand, ICommandHandler<ICommand>>(UnitOfWork);
            }
        }

        [Test]
        public void Should_call_handle_method_when_execute_transaction()
        {
            var context = new UOWContext();
            ICommandHandler<ICommand> commandHandler = Substitute.For<ICommandHandler<ICommand>>();
            context.TransactionHandler.Execute(Substitute.For<ICommand>(), commandHandler);
            commandHandler.Received(1).Handle(Arg.Any<ICommand>());
        }

        [Test]
        public void Should_call_commit_method_when_execute_transaction()
        {
            var context = new UOWContext();
            context.TransactionHandler.Execute(Substitute.For<ICommand>(), Substitute.For<ICommandHandler<ICommand>>());
            context.UnitOfWork.Received(1).Commit();
        }

        [Test]
        public void Should_call_commitrollback_method_when_exception_throw_during_execution_transaction()
        {
            var context = new UOWContext();
            var commandHandler = Substitute.For<ICommandHandler<ICommand>>();
            commandHandler.When(c => c.Handle(Arg.Any<ICommand>())).Do(ThrowException);
            Assert.Throws<Exception>(() => context.TransactionHandler.Execute(Substitute.For<ICommand>(), commandHandler));
            context.UnitOfWork.Received(1).Rollback();
        }

        private static void ThrowException(CallInfo callInfo)
        {
            throw new Exception();
        }
    }
}