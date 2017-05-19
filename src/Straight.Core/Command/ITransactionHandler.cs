using Straight.Core.Common.Command;

namespace Straight.Core.Command
{
    public interface ITransactionHandler<in TCommand, in TCommandHandler>
        where TCommandHandler : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        void Execute(TCommand command, TCommandHandler commandHandler);
    }
}