namespace Straight.Core.Command
{
    public interface ITransactionHandler<TCommand, TCommandHandler>
        where TCommandHandler : ICommandHandler<TCommand>
        where TCommand : class, ICommand
    {
        void Execute(TCommand command, TCommandHandler commandHandler);
    }
}