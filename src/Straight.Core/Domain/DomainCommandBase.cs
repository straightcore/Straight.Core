using System;

namespace Straight.Core.Domain
{
    public abstract class DomainCommandBase : IDomainCommand
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}