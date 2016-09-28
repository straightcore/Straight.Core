using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.Domain
{
    [TestFixture]
    public class HouseTests
    {
        private House.Domain.House _house;

        [SetUp]
        public void Setup()
        {
            _house = new House.Domain.House();
        }

        [Test]
        public void Should_house_is_initialize_when_load_from_history()
        {
            var user = new User("Doe", "John", "john.doe");
            var address = new Address("Central Park West", "", "Cross 79th Street", "10024", "New York");
            var aggregateId = Guid.NewGuid();
            _house.LoadFromHistory(new List<IDomainEvent>() { new HouseCreated(user, address) { Version = 1, AggregateId = aggregateId } });
            Assert.That(_house.Id, Is.EqualTo(aggregateId));
            Assert.That(_house.Version, Is.EqualTo(1));
            Assert.That(_house.Address, Is.Not.Null);
            Assert.That(_house.Creator, Is.Not.Null);
            Assert.That(_house.LastModifier, Is.Null);
        }


        [Test]
        public void Should_house_is_initialize_with_2_event_when_begin()
        {
            var john = new User("Doe", "John", "john.doe");
            var jane = new User("Doe", "Jane", "jane.doe");
            var washington = new Address("Central Park South", "", "Cross 54 Street", "10024", "Washington");
            var newyork = new Address("Central Park West", "", "Cross 79th Street", "10024", "New York");
            var aggregateId = Guid.NewGuid();

            _house.LoadFromHistory(new List<IDomainEvent>()
            {
                new HouseCreated(john, washington) { Version = 1, AggregateId = aggregateId },
                new AddressUpdated(jane, newyork) { Version = 2, AggregateId = aggregateId },
            });

            Assert.That(_house.Id, Is.EqualTo(aggregateId));
            Assert.That(_house.Version, Is.EqualTo(2));
            Assert.That(_house.Address, Is.EqualTo(newyork));
            Assert.That(_house.Creator, Is.EqualTo(john));
            Assert.That(_house.LastModifier, Is.EqualTo(jane));
        }
    }
}
