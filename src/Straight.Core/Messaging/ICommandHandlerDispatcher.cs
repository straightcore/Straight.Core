using Straight.Core.Command;

namespace Straight.Core.Messaging
{
    public interface ICommandHandlerDispatcher
    {
        void Register(ICommandHandler handler);

        void Process(ICommand command);
    }
}