using Xunit;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.Tests.Domain
{
    
    public class ReadModelTests
    {
        
        public ReadModelTests()
        {
            readModel = new ReadModelTest(() => callBack());
        }

        private IReadModel<IDomainEvent> readModel;
        private readonly Action callBack = () => { };

        [Fact]
        public void Should_apply_event_when_new_event_is_raised()
        {
            var guid = Guid.NewGuid();
            var expectedEvent = new DomainEventTest {Id = guid, AggregateId = guid, Version = 2};
            readModel.Update(expectedEvent);
            Assert.Equal(readModel.Id, expectedEvent.Id);
            Assert.Equal(readModel.Version, expectedEvent.Version);
            Assert.Equal(readModel.Events.Count(ev => ev == expectedEvent), 1);
        }

        [Fact]
        public void Should_load_historical_event_when_load_read_model()
        {
            var guid = Guid.NewGuid();
            var events = new List<IDomainEvent>();
            for (var version = 0; version < 5; version++)
                events.Add(new DomainEventTest {Id = Guid.NewGuid(), AggregateId = guid, Version = version});
            readModel.LoadFromHistory(events);
        }
    }
}