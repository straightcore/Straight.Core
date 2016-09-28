using System;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common.EventStore;

namespace Straight.Core.Tests.Common.Domain
{

    public class ReadModelTest : ReadModelBase<IDomainEvent>
    , IApplyEvent<DomainEventTest>
    {

        private readonly Action _whenApplied;

        public ReadModelTest(Action whenApplied)
            : base()
        {
            this._whenApplied = whenApplied;
        }

        public ReadModelTest()
            : this(() => { })
        {
            
        }

        void IApplyEvent<DomainEventTest>.Apply(DomainEventTest @event)
        {
            _whenApplied?.Invoke();
        }
    }

    public class ReadModelTest2 : ReadModelBase<IDomainEvent>
    , IApplyEvent<DomainEventTest2>
    {

        private readonly Action _whenApplied;

        public ReadModelTest2(Action whenApplied)
            : base()
        {
            this._whenApplied = whenApplied;
        }

        public ReadModelTest2()
            : this(() => { })
        {

        }

        void IApplyEvent<DomainEventTest2>.Apply(DomainEventTest2 @event)
        {
            _whenApplied?.Invoke();
        }
    }
}