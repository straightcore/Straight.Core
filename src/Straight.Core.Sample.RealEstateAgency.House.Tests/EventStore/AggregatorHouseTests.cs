using NUnit.Framework;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Model.Exceptions;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using System;
using System.Linq;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.EventStore
{
    [TestFixture]
    public class AggregatorHouseTests
    {
        
        public AggregatorHouse BuildAggregatorHouseTests()
        {
            var house = new AggregatorHouse();
            house.Update(new CreateHouseCommand
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
            return house;
        }
        
        [Test]
        public void Should_add_visit_when_add_visit_command()
        {
            var house = BuildAggregatorHouseTests();
            house.Clear();
            house.Update(new AddVisitHouseCommand
            {
                EstateOfficer = PersonaUser.John,
                Account = PersonaAccount.Virginie,
                MeetDate = DateTime.UtcNow.Date.AddDays(2).AddHours(12)
            });
            Assert.That(house.GetChanges().Count(), Is.EqualTo(1));
            Assert.That(house.GetChanges().Last().GetType(), Is.EqualTo(typeof(VisitAdded)));
        }

        [Test]
        public void Should_change_address_when_adress_is_wrong()
        {
            var house = BuildAggregatorHouseTests();
            house.Clear();
            house.Update(new UpdateAddressCommand
            {
                Street = "Central Park West",
                StreetNumber = "",
                AdditionalAddress = "Cross 79th Street",
                PostalCode = "10024",
                City = "New York",
                LastName = "Doe",
                FirstName = "Jane",
                Username = "jane.doe"
            });
            Assert.That(house.GetChanges().Count(), Is.EqualTo(1));
            Assert.That(house.GetChanges().Last().GetType(), Is.EqualTo(typeof(AddressUpdated)));
        }

        [Test]
        public void Should_create_event_when_create_new_aggregator_house()
        {
            var house = BuildAggregatorHouseTests();
            house = new AggregatorHouse();
            house.Update(new CreateHouseCommand
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
            Assert.That(house.GetChanges().Count(), Is.EqualTo(1));
            Assert.That(house.GetChanges().First().GetType(), Is.EqualTo(typeof(HouseCreated)));
        }

        [Test]
        public void Should_throw_exception_when_add_visit_at_date_already_used()
        {
            var house = BuildAggregatorHouseTests();
            house.Clear();
            house.Update(new AddVisitHouseCommand
            {
                EstateOfficer = PersonaUser.John,
                Account = PersonaAccount.Virginie,
                MeetDate = DateTime.UtcNow.Date.AddDays(2).AddHours(12)
            });
            Assert.Throws<DateAlreadyExistException>(() =>
                house.Update(new AddVisitHouseCommand
                {
                    EstateOfficer = PersonaUser.Jane,
                    Account = PersonaAccount.Pierre,
                    MeetDate = DateTime.UtcNow.Date.AddDays(2).AddHours(12)
                }));
        }
    }
}