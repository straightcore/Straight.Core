using Straight.Core.Storage;
using System;
using Straight.Core.Common.Command;

namespace Straight.Core.Command
{
    public class UnitOfWorkTransactionHandler<TCommand, TCommandHandler> : ITransactionHandler<TCommand, TCommandHandler>
        where TCommandHandler : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkTransactionHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(TCommand command, TCommandHandler commandHandler)
        {
            try
            {
                commandHandler.Handle(command);
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}