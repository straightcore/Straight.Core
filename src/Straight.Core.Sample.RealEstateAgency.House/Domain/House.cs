using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Model;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using System.Collections.Generic;

namespace Straight.Core.Sample.RealEstateAgency.House.Domain
{
    public class House : ReadModelBase<IDomainEvent>, IHouse
        , IApplyEvent<HouseCreated>
        , IApplyEvent<AddressUpdated>
        , IApplyEvent<VisitAdded>
    {
        private readonly List<Visit> _planningMeet = new List<Visit>();
        public IEnumerable<Visit> PlanningMeet => _planningMeet.AsReadOnly();

        public void Apply(AddressUpdated @event)
        {
            LastModifier = @event.Modifier;
            Address = @event.NewAddress;
        }

        public void Apply(HouseCreated @event)
        {
            Creator = @event.Creator;
            Address = @event.Address;
        }

        public void Apply(VisitAdded @event)
        {
            _planningMeet.Add(new Visit(@event.EstateOfficer, @event.Account, @event.MeetDateTime));
            LastModifier = @event.EstateOfficer;
        }

        public User Creator { get; private set; }
        public Address Address { get; private set; }
        public User LastModifier { get; private set; }
    }
}