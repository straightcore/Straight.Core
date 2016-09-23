using System;

namespace Straight.Core.Command
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}