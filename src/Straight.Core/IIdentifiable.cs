using System;

namespace Straight.Core
{
    public interface IIdentifiable
    {
        Guid Id { get; }
    }
}