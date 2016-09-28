using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.EventStore
{
    [TestFixture]
    public class AggregatorHouseTests
    {
        private AggregatorHouse _house;

        [SetUp]
        public void Setup()
        {
            _house = new AggregatorHouse();
        }

        [Test]
        public void Should_create_event_when_create_new_aggregator_house()
        {
            _house.Update(new CreateHouseCommand()
            {
                Street = "Central Park West",
                StreetNumber = "",
                AdditionalAddress = "Cross 79th Street",
                PostalCode = "10024",
                City = "New York",
                CreatorLastName = "Doe",
                CreatorFirstName = "John",
                CreatorUsername = "john.doe"
            });
            Assert.That(_house.GetChanges(), Has.Count.EqualTo(1));
            Assert.That(_house.GetChanges().First().GetType(), Is.EqualTo(typeof(HouseCreated)));
        }


        [Test]
        public void Should_change_address_when_adress_is_wrong()
        {
            _house.Update(new UpdateAddressCommand() {
                Street = "Central Park West",
                StreetNumber = "",
                AdditionalAddress = "Cross 79th Street",
                PostalCode = "10024",
                City = "New York",
                LastName = "Doe",
                FirstName = "Jane",
                Username = "jane.doe"
            });
            Assert.That(_house.GetChanges(), Has.Count.EqualTo(1));
            Assert.That(_house.GetChanges().Last().GetType(), Is.EqualTo(typeof(AddressUpdated)));
        }
    }
    
}