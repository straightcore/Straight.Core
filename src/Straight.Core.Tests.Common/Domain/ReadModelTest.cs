using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common.EventStore;
using System;

namespace Straight.Core.Tests.Common.Domain
{
    public class ReadModelTest : ReadModelBase<IDomainEvent>
    {
        private readonly Action _whenApplied;

        public ReadModelTest(Action whenApplied)
        {
            _whenApplied = whenApplied;
        }

        public ReadModelTest()
            : this(() => { })
        {
        }

        private void Apply(DomainEventTest @event)
        {
            _whenApplied?.Invoke();
        }
    }

    public class ReadModelTest2 : ReadModelBase<IDomainEvent>
    {
        private readonly Action _whenApplied;

        public ReadModelTest2(Action whenApplied)
        {
            _whenApplied = whenApplied;
        }

        public ReadModelTest2()
            : this(() => { })
        {
        }

        private void Apply(DomainEventTest2 @event)
        {
            _whenApplied?.Invoke();
        }
    }
}