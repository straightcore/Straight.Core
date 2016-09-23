using System;

namespace Straight.Core.EventStore
{
    public interface IDomainCommand
    {
        Guid Id { get; }
    }
}