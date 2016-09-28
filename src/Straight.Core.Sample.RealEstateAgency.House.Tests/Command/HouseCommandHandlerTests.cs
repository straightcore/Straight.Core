using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Sample.RealEstateAgency.House.Command;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.Command
{
    [TestFixture]
    public class HouseCommandHandlerTests
    {
        private HouseCommandHandler commandHandler;
        private IDomainEventStore<IDomainEvent> repository;
        private Dictionary<Guid, AggregatorHouse> _houses;

        [SetUp]
        public void Setup()
        {
            repository = GetRepository();
            commandHandler = new HouseCommandHandler(repository);
        }

        private IDomainEventStore<IDomainEvent> GetRepository()
        {
            _houses = new Dictionary<Guid, AggregatorHouse>();
            var repo = Substitute.For<IDomainEventStore<IDomainEvent>>();
            repo.GetById<AggregatorHouse>(Arg.Any<Guid>()).Returns(GetByIdHouse);
            repo.When(r => r.Add(Arg.Any<AggregatorHouse>()))
                .Do(info => _houses[info.Arg<AggregatorHouse>().Id] = info.Arg<AggregatorHouse>());
            return repo;
        }

        private AggregatorHouse GetByIdHouse(CallInfo callInfo)
        {
            return _houses[callInfo.Arg<Guid>()];
        }


        [Test]
        public void Should_create_new_house_when_emit_create_house_command()
        {
            commandHandler.Handle(new CreateHouseCommandHandler()
            {
                Address = new AddressDto()
                {
                    Street = "Central Park West",
                    StreetNumber = "",
                    AdditionalAddress = "Cross 79th Street",
                    PostalCode = "10024",
                    City = "New York",
                },
                Creator = new RequesterDto()
                {
                    LastName = "Doe",
                    FirstName = "John",
                    Username = "john.doe",
                }
            });
            repository.Received(1).Add(Arg.Any<AggregatorHouse>());
        }
        
        [Test]
        public void Should_update_address_when_emit_update_house_command()
        {
            var house = new AggregatorHouse();
            house.Update(new CreateHouseCommand()
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
            _houses.Add(house.Id, house);
            commandHandler.Handle(new UpdateHouseCommandHandler()
            {
                HouseId = house.Id,
                Address = new AddressDto()
                {
                    Street = "Central Park West",
                    StreetNumber = "",
                    AdditionalAddress = "Cross 79th Street",
                    PostalCode = "10024",
                    City = "New York",
                },
                Modifier = new RequesterDto()
                {
                    LastName = "Doe",
                    FirstName = "John",
                    Username = "john.doe",
                }
            });
            repository.Received(1).GetById<AggregatorHouse>(Arg.Is(house.Id));
        }

        [Test]
        public void Should_have_created_event_when_emit_create_house_command()
        {
            _houses.Clear();
            commandHandler.Handle(new CreateHouseCommandHandler()
            {
                Address = new AddressDto()
                {
                    Street = "Central Park West",
                    StreetNumber = "",
                    AdditionalAddress = "Cross 79th Street",
                    PostalCode = "10024",
                    City = "New York",
                },
                Creator = new RequesterDto()
                {
                    LastName = "Doe",
                    FirstName = "John",
                    Username = "john.doe",
                }
            });
            Assert.That(_houses, Has.Count.EqualTo(1));
        }

        [Test]
        public void Should_have_updated_event_when_emit_update_house_command()
        {
            _houses.Clear();
            var house = new AggregatorHouse();
            house.Update(new CreateHouseCommand()
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
            house.Clear(); // Clear pending event
            _houses.Add(house.Id, house);
            var expectedCommand = new UpdateHouseCommandHandler()
            {
                HouseId = house.Id,
                Address = new AddressDto()
                {
                    Street = "Central Park West",
                    StreetNumber = "",
                    AdditionalAddress = "Cross 79th Street",
                    PostalCode = "10024",
                    City = "New York",
                },
                Modifier = new RequesterDto()
                {
                    LastName = "Doe",
                    FirstName = "jane",
                    Username = "jane.doe",
                }
            };
            commandHandler.Handle(expectedCommand);
            var actual = _houses[house.Id];
            var addressUpdated = actual.GetChanges().OfType<AddressUpdated>().First();
            Assert.That(addressUpdated.NewAddress.Street, Is.EqualTo(expectedCommand.Address.Street));
            Assert.That(addressUpdated.NewAddress.City, Is.EqualTo(expectedCommand.Address.City));
            Assert.That(addressUpdated.NewAddress.PostalCode, Is.EqualTo(expectedCommand.Address.PostalCode));
            Assert.That(addressUpdated.NewAddress.StreetNumber, Is.EqualTo(expectedCommand.Address.StreetNumber));
            Assert.That(addressUpdated.Modifier.FirstName, Is.EqualTo(expectedCommand.Modifier.FirstName));
            Assert.That(addressUpdated.Modifier.LastName, Is.EqualTo(expectedCommand.Modifier.LastName));
            Assert.That(addressUpdated.Modifier.Username, Is.EqualTo(expectedCommand.Modifier.Username));
        }
    }
}