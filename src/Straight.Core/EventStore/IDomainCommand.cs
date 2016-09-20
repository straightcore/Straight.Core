using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Straight.Core.EventStore
{
    public interface IDomainCommand
    {
        Guid Id { get; }
    }
}
