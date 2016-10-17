using NUnit.Framework;
using Straight.Core.RealEstateAgency.Model.Exceptions;
using Straight.Core.RealEstateAgency.Test.Common.Server;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using System;
using System.Linq;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.EventStore
{
    [TestFixture]
    public class AggregatorHouseTests
    {
        [SetUp]
        public void Setup()
        {
            _house = new AggregatorHouse();
            _house.Update(new CreateHouseCommand
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
        }

        private AggregatorHouse _house;

        [Test]
        public void Should_add_visit_when_add_visit_command()
        {
            _house.Clear();
            _house.Update(new AddVisitHouseCommand
            {
                EstateOfficer = PersonaUser.John,
                Account = PersonaAccount.Virginie,
                MeetDateTime = DateTime.UtcNow.Date.AddDays(2).AddHours(12)
            });
            Assert.That(_house.GetChanges(), Has.Count.EqualTo(1));
            Assert.That(_house.GetChanges().Last().GetType(), Is.EqualTo(typeof(VisitAdded)));
        }

        [Test]
        public void Should_change_address_when_adress_is_wrong()
        {
            _house.Clear();
            _house.Update(new UpdateAddressCommand
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
            Assert.That(_house.GetChanges(), Has.Count.EqualTo(1));
            Assert.That(_house.GetChanges().Last().GetType(), Is.EqualTo(typeof(AddressUpdated)));
        }

        [Test]
        public void Should_create_event_when_create_new_aggregator_house()
        {
            _house = new AggregatorHouse();
            _house.Update(new CreateHouseCommand
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
        public void Should_throw_exception_when_add_visit_at_date_already_used()
        {
            _house.Clear();
            _house.Update(new AddVisitHouseCommand
            {
                EstateOfficer = PersonaUser.John,
                Account = PersonaAccount.Virginie,
                MeetDateTime = DateTime.UtcNow.Date.AddDays(2).AddHours(12)
            });
            Assert.Throws<DateAlreadyExistException>(() =>
                _house.Update(new AddVisitHouseCommand
                {
                    EstateOfficer = PersonaUser.Jane,
                    Account = PersonaAccount.Pierre,
                    MeetDateTime = DateTime.UtcNow.Date.AddDays(2).AddHours(12)
                }));
        }
    }
}