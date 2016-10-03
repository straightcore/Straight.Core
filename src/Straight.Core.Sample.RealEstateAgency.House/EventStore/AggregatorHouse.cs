using System;
using System.Collections;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Extensions.Guard;
using Straight.Core.RealEstateAgency.Model;
using Straight.Core.RealEstateAgency.Model.Helper;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;

namespace Straight.Core.Sample.RealEstateAgency.House.EventStore
{
    public class AggregatorHouse : AggregatorBase<IDomainEvent>
        , IHandlerDomainCommand<CreateHouseCommand>
        , IApplyEvent<HouseCreated>
        , IHandlerDomainCommand<UpdateAddressCommand>
        , IApplyEvent<AddressUpdated>
    {
        private Address _address;
        private User _creator;
        private User _lastModifier;

        public IEnumerable Handle(CreateHouseCommand command)
        {
            AddressHelper.CheckMandatory(command.Street, command.City, command.PostalCode);
            UserHelper.CheckMandatoryUser(command.CreatorFirstName, command.CreatorLastName, command.CreatorUsername);
            var address = new Address(command.Street, command.StreetNumber, command.AdditionalAddress, command.PostalCode, command.City);
            var creator = new User(command.CreatorFirstName, command.CreatorLastName, command.CreatorUsername);
            yield return new HouseCreated(creator, address);
        }

        public void Apply(HouseCreated @event)
        {
            _address = @event.Address;
            _creator = @event.Creator;
        }

        public IEnumerable Handle(UpdateAddressCommand command)
        {
            command.CheckIfArgumentIsNull("command");
            AddressHelper.CheckMandatory(command.Street, command.City, command.PostalCode);
            UserHelper.CheckMandatoryUser(command.FirstName, command.LastName, command.Username);
            var address = new Address(command.Street, command.StreetNumber, command.AdditionalAddress, command.PostalCode, command.City);
            var modifier = new User(command.LastName, command.FirstName, command.Username);
            yield return new AddressUpdated(modifier, address);
        }

        public void Apply(AddressUpdated @event)
        {
            _address = @event.NewAddress;
            _lastModifier = @event.Modifier;
        }


    }
}