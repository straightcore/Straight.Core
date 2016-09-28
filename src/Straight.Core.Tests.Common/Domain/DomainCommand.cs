using System;
using Straight.Core.Domain;
using Straight.Core.EventStore;

namespace Straight.Core.Tests.Common.Domain
{

    public class DomainCommandTest2 : IDomainCommand
    {
        public Guid Id { get; set; }
    }

    public class DomainCommandUnknow : IDomainCommand
    {
        public Guid Id { get; set; }
    }

    public class DomainCommandTest : IDomainCommand
    {
        public Guid Id { get; set; }
    }

}