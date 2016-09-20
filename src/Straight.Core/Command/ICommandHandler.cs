
namespace Straight.Core.Command
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        void Handle(TCommand theEvent);
    }
}
