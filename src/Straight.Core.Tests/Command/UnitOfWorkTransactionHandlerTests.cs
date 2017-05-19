using NSubstitute;
using Xunit;
using Straight.Core.Command;
using Straight.Core.Storage;
using System;
using NSubstitute.Core;
using Straight.Core.Common.Command;

namespace Straight.Core.Tests.Command
{
    
    public class UnitOfWorkTransactionHandlerTests
    {
        private UnitOfWorkTransactionHandler<ICommand, ICommandHandler<ICommand>> _transactionHandler;
        private IUnitOfWork _unitOfWork;

        
        public UnitOfWorkTransactionHandlerTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _transactionHandler = new UnitOfWorkTransactionHandler<ICommand, ICommandHandler<ICommand>>(_unitOfWork);
        }


        [Fact]
        public void Should_call_handle_method_when_execute_transaction()
        {
            ICommandHandler<ICommand> commandHandler = Substitute.For<ICommandHandler<ICommand>>();
            _transactionHandler.Execute(Substitute.For<ICommand>(), commandHandler);
            commandHandler.Received(1).Handle(Arg.Any<ICommand>());
        }

        [Fact]
        public void Should_call_commit_method_when_execute_transaction()
        {
            _transactionHandler.Execute(Substitute.For<ICommand>(), Substitute.For<ICommandHandler<ICommand>>());
            _unitOfWork.Received(1).Commit();
        }

        [Fact]
        public void Should_call_commitrollback_method_when_exception_throw_during_execution_transaction()
        {
            var commandHandler = Substitute.For<ICommandHandler<ICommand>>();
            commandHandler.When(c => c.Handle(Arg.Any<ICommand>())).Do(ThrowException);
            Assert.Throws<Exception>(() => _transactionHandler.Execute(Substitute.For<ICommand>(), commandHandler));
            _unitOfWork.Received(1).Rollback();
        }

        private static void ThrowException(CallInfo callInfo)
        {
            throw new Exception();
        }
    }
}