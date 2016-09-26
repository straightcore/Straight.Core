using System.Collections;

namespace Straight.Core.EventStore
{
    public interface IHandlerDomainCommand<in TCommand>
        where TCommand : IDomainCommand
    {
        IEnumerable Handle(TCommand command);
    }
}