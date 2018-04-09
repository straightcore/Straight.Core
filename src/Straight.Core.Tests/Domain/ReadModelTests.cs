﻿using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common.Domain;
using Straight.Core.Tests.Common.EventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Straight.Core.Tests.Domain
{
    [TestFixture]
    public class ReadModelTests
    {
        
        public ReadModelTests()
        {
            readModel = new ReadModelTest(() => callBack());
        }

        private IReadModel<IDomainEvent> readModel;
        private readonly Action callBack = () => { };

        [Test]
        public void Should_apply_event_when_new_event_is_raised()
        {
            var guid = Guid.NewGuid();
            var expectedEvent = new DomainEventTest {Id = guid, AggregateId = guid, Version = 2};
            readModel.Update(expectedEvent);
            Assert.That(readModel.Id, Is.EqualTo(expectedEvent.Id));
            Assert.That(readModel.Version, Is.EqualTo(expectedEvent.Version));
            Assert.That(readModel.Events.Count(ev => ev == expectedEvent), Is.EqualTo(1));
        }

        [Test]
        public void Should_load_historical_event_when_load_read_model()
        {
            var guid = Guid.NewGuid();
            var events = new List<IDomainEvent>();
            for (var version = 0; version < 5; version++)
            {
                events.Add(new DomainEventTest { Id = Guid.NewGuid(), AggregateId = guid, Version = version });
            }
            readModel.LoadFromHistory(events);
        }
    }
}