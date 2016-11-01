using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using System;
using System.Collections.Generic;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.Domain
{
    [TestFixture]
    public class HouseTests
    {
        [SetUp]
        public void Setup()
        {
            _house = new House.Domain.House();
        }

        private House.Domain.House _house;

        [Test]
        public void Should_house_is_initialize_when_load_from_history()
        {
            var aggregateId = Guid.NewGuid();
            _house.LoadFromHistory(new List<IDomainEvent>
            {
                new HouseCreated(PersonaUser.John, PersonaAddress.NationalMuseumNewYork)
                {
                    Version = 1,
                    AggregateId = aggregateId
                }
            });
            Assert.That(_house.Id, Is.EqualTo(aggregateId));
            Assert.That(_house.Version, Is.EqualTo(1));
            Assert.That(_house.Address, Is.Not.Null);
            Assert.That(_house.Creator, Is.Not.Null);
            Assert.That(_house.LastModifier, Is.Null);
        }

        [Test]
        public void Should_house_is_initialize_with_2_event_when_begin()
        {
            var aggregateId = Guid.NewGuid();

            _house.LoadFromHistory(new List<IDomainEvent>
            {
                new HouseCreated(PersonaUser.John, PersonaAddress.NationalMuseumNewYorkMistakeInWashington)
                {
                    Version = 1,
                    AggregateId = aggregateId
                },
                new AddressUpdated(PersonaUser.Jane, PersonaAddress.NationalMuseumNewYork)
                {
                    Version = 2,
                    AggregateId = aggregateId
                }
            });

            Assert.That(_house.Id, Is.EqualTo(aggregateId));
            Assert.That(_house.Version, Is.EqualTo(2));
            Assert.That(_house.Address,
                Is.EqualTo(PersonaAddress.NationalMuseumNewYork).Using(PersonaAddress.AddressValueComparer));
            Assert.That(_house.Creator, Is.EqualTo(PersonaUser.John).Using(PersonaUser.UserValueComparer));
            Assert.That(_house.LastModifier, Is.EqualTo(PersonaUser.Jane).Using(PersonaUser.UserValueComparer));
        }
    }
}