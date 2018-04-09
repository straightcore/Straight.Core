using NSubstitute;
using NSubstitute.Core;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Sample.RealEstateAgency.Contracts.Messages.House;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using Straight.Core.Sample.RealEstateAgency.House.Command;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Straight.Core.Sample.RealEstateAgency.House.Tests.Command
{
    [TestFixture]
    public class HouseCommandHandlerTests
    {
        private class Context
        {
            public Context()
            {
                Houses = new Dictionary<Guid, AggregatorHouse>();
                Repository = GetRepository();
                CommandHandler = new HouseCommandHandler(Repository);
            }

            public HouseCommandHandler CommandHandler { get; }
            public IDomainEventStore<IDomainEvent> Repository { get; }
            public Dictionary<Guid, AggregatorHouse> Houses { get; }

            private IDomainEventStore<IDomainEvent> GetRepository()
            {
                var repo = Substitute.For<IDomainEventStore<IDomainEvent>>();
                repo.GetById<AggregatorHouse>(Arg.Any<Guid>()).Returns(GetByIdHouse);
                repo.When(r => r.Add(Arg.Any<AggregatorHouse>()))
                    .Do(info => Houses[info.Arg<AggregatorHouse>().Id] = info.Arg<AggregatorHouse>());
                return repo;
            }


            private AggregatorHouse GetByIdHouse(CallInfo callInfo)
            {
                return Houses[callInfo.Arg<Guid>()];
            }
        }

        [Test]
        public void Should_create_new_house_when_emit_create_house_command()
        {
            var context = new Context();
            context.CommandHandler.Handle(new CreateHouseCommandHandler
            {
                Address = PersonaAddressDto.NationalMuseumNewYork,
                Creator = PersonaRequesterDto.John
            });
            context.Repository.Received(1).Add(Arg.Any<AggregatorHouse>());
        }

        [Test]
        public void Should_have_created_event_when_emit_create_house_command()
        {
            var context = new Context();
            var newYork = PersonaAddressDto.NationalMuseumNewYork;
            var john = PersonaRequesterDto.John;

            context.Houses.Clear();
            context.CommandHandler.Handle(new CreateHouseCommandHandler
            {
                Address = newYork,
                Creator = john
            });
            Assert.That(context.Houses.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_have_updated_event_when_emit_update_house_command()
        {
            var context = new Context();
            var newYork = PersonaAddressDto.NationalMuseumNewYork;
            var john = PersonaRequesterDto.John;

            context.Houses.Clear();
            var house = new AggregatorHouse();
            house.Update(new CreateHouseCommand
            {
                Street = newYork.Street,
                StreetNumber = newYork.StreetNumber,
                AdditionalAddress = newYork.AdditionalAddress,
                PostalCode = newYork.PostalCode,
                City = newYork.City,
                CreatorLastName = john.LastName,
                CreatorFirstName = john.FirstName,
                CreatorUsername = john.Username
            });
            house.Clear(); // Clear pending event
            context.Houses.Add(house.Id, house);
            var expectedCommand = new UpdateHouseCommandHandler
            {
                HouseId = house.Id,
                Address = new AddressDto
                {
                    Street = "Central Park West",
                    StreetNumber = "",
                    AdditionalAddress = "Cross 79th Street",
                    PostalCode = "10024",
                    City = "New York"
                },
                Modifier = PersonaRequesterDto.Jane
            };
            context.CommandHandler.Handle(expectedCommand);
            var actual = context.Houses[house.Id];
            var addressUpdated = actual.GetChanges().OfType<AddressUpdated>().First();
            Assert.That(addressUpdated.NewAddress.Street, Is.EqualTo(expectedCommand.Address.Street));
            Assert.That(addressUpdated.NewAddress.City, Is.EqualTo(expectedCommand.Address.City));
            Assert.That(addressUpdated.NewAddress.PostalCode, Is.EqualTo(expectedCommand.Address.PostalCode));
            Assert.That(addressUpdated.NewAddress.StreetNumber, Is.EqualTo(expectedCommand.Address.StreetNumber));
            Assert.That(addressUpdated.Modifier.FirstName, Is.EqualTo(expectedCommand.Modifier.FirstName));
            Assert.That(addressUpdated.Modifier.LastName, Is.EqualTo(expectedCommand.Modifier.LastName));
            Assert.That(addressUpdated.Modifier.Username, Is.EqualTo(expectedCommand.Modifier.Username));
        }

        [Test]
        public void Should_update_address_when_emit_update_house_command()
        {
            var context = new Context();
            var newYork = PersonaAddressDto.NationalMuseumNewYork;
            var john = PersonaRequesterDto.John;
            var house = new AggregatorHouse();
            house.Update(new CreateHouseCommand
            {
                Street = newYork.Street,
                StreetNumber = newYork.StreetNumber,
                AdditionalAddress = newYork.AdditionalAddress,
                PostalCode = newYork.PostalCode,
                City = newYork.City,
                CreatorLastName = john.LastName,
                CreatorFirstName = john.FirstName,
                CreatorUsername = john.Username
            });
            context.Houses.Add(house.Id, house);
            context.CommandHandler.Handle(new UpdateHouseCommandHandler
            {
                HouseId = house.Id,
                Address = newYork,
                Modifier = john
            });
            context.Repository.Received(1).GetById<AggregatorHouse>(Arg.Is(house.Id));
        }
    }
}