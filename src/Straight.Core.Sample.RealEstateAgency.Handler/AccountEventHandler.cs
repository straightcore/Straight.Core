using System;
using Straight.Core.Domain.Storage;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Extensions.Guard;
using Straight.Core.Messaging;
using Straight.Core.Sample.RealEstateAgency.Account.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Model;
using HouseVisitAdded = Straight.Core.Sample.RealEstateAgency.House.EventStore.Events.VisitAdded;

namespace Straight.Core.Sample.RealEstateAgency.Handler
{
    public class AccountEventHandler : IEventHandler<HouseVisitAdded>
    {
        private readonly IDomainEventStore<IDomainEvent> _aggregatorRepository;
        private readonly IReadModelRepository<IDomainEvent> _readRepository;

        public AccountEventHandler(IDomainEventStore<IDomainEvent> aggregatorRepository,
            IReadModelRepository<IDomainEvent> readRepository)
        {
            _aggregatorRepository = aggregatorRepository;
            _readRepository = readRepository;
        }
        
        public void Handle(HouseVisitAdded @event)
        {
            @event.CheckIfArgumentIsNull("event");
            var aggregate = _aggregatorRepository.GetById<AggregatorAccount>(@event.Account.Id);
            if (aggregate == null)
            {
                throw new ArgumentException($"Cannot find account ('{aggregate.Id}')");
            }
            IHouse house = _readRepository.GetById<House.Domain.House>(@event.AggregateId);
            aggregate.Update(new AddVisitCommand()
            {
                EstateOfficierFirstName = @event.EstateOfficer.FirstName,
                EstateOfficierLastName = @event.EstateOfficer.LastName,
                EstateOfficierUsername = @event.EstateOfficer.Username,
                MeetDate = @event.MeetDateTime,
                House = house,
            });
        }
    }
}