using System;
using Straight.Core.Command;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Exceptions;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;

namespace Straight.Core.Sample.RealEstateAgency.House.Command
{
    public class HouseCommandHandler : ICommandHandler<CreateHouseCommandHandler>
        , ICommandHandler<UpdateHouseCommandHandler>
    {
        private readonly IDomainEventStore<IDomainEvent> _repository;

        public HouseCommandHandler(IDomainEventStore<IDomainEvent> repository)
        {
            _repository = repository;
        }

        public void Handle(CreateHouseCommandHandler commandHandler)
        {
            var house = new AggregatorHouse();
            house.Update(new CreateHouseCommand()
            {
                CreatorFirstName = commandHandler.Creator.FirstName,
                CreatorLastName = commandHandler.Creator.LastName,
                CreatorUsername = commandHandler.Creator.Username,
                PostalCode = commandHandler.Address.PostalCode,
                AdditionalAddress = commandHandler.Address.AdditionalAddress,
                City = commandHandler.Address.City,
                Street = commandHandler.Address.Street,
                StreetNumber = commandHandler.Address.StreetNumber,
            });
            _repository.Add(house);

        }

        public void Handle(UpdateHouseCommandHandler commandHandler)
        {
            var house = _repository.GetById<AggregatorHouse>(commandHandler.HouseId);
            if (house.Id != commandHandler.HouseId)
            {
                throw new ModelNotFoundException(
                    $"House not found, please check if this id is correct: {commandHandler.HouseId}");
            }
            house.Update(new UpdateAddressCommand()
            {
                FirstName = commandHandler.Modifier.FirstName,
                LastName = commandHandler.Modifier.LastName,
                Username = commandHandler.Modifier.Username,
                PostalCode = commandHandler.Address.PostalCode,
                AdditionalAddress = commandHandler.Address.AdditionalAddress,
                City = commandHandler.Address.City,
                Street = commandHandler.Address.Street,
                StreetNumber = commandHandler.Address.StreetNumber,
            });
        }
    }
}