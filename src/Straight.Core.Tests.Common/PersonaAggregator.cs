using System;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;

namespace Straight.Core.Tests.Common
{
    public static class PersonaAggregator
    {
        public static AggregatorTest CreateNewAggregatorTest(Action whenApplied)
        {
            var aggregator = new AggregatorTest(whenApplied);
            aggregator.Update(new DomainCommandTest());
            return aggregator;
        }
    }
}