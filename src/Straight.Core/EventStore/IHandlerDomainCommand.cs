using System.Collections;

namespace Straight.Core.EventStore
{
    public interface IHandlerDomainCommand<TCommand>
        where TCommand : IDomainCommand
    {
        IEnumerable Handle(TCommand command);
    }
}