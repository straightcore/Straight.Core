using Xunit;
using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using System;
using System.Collections.Generic;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.Domain
{
    
    public class HouseTests
    {
        
        public HouseTests()
        {
            _house = new House.Domain.House();
        }

        private House.Domain.House _house;

        [Fact]
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
            Assert.Equal(_house.Id, aggregateId);
            Assert.Equal(_house.Version, 1);
            Assert.NotNull(_house.Address);
            Assert.NotNull(_house.Creator);
            Assert.Null(_house.LastModifier);
        }

        [Fact]
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

            Assert.Equal(_house.Id, aggregateId);
            Assert.Equal(_house.Version, 2);
            Assert.Equal(_house.Address, PersonaAddress.NationalMuseumNewYork, PersonaAddress.AddressValueComparer);
            Assert.Equal(_house.Creator, PersonaUser.John, PersonaUser.UserValueComparer);
            Assert.Equal(_house.LastModifier, PersonaUser.Jane, PersonaUser.UserValueComparer);
        }
    }
}