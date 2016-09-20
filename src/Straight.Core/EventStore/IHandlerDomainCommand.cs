using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Straight.Core.EventStore
{
    public interface IHandlerDomainCommand<TCommand>
        where TCommand : IDomainCommand
    {
        IEnumerable Handle(TCommand command);
    } 
}
