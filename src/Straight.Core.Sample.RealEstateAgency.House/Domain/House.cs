using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Model;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;

namespace Straight.Core.Sample.RealEstateAgency.House.Domain
{
    public class House : ReadModelBase<IDomainEvent>, IHouse
        , IApplyEvent<HouseCreated>
        , IApplyEvent<AddressUpdated>
    {
        public User Creator { get; private set; }
        public Address Address { get; private set; }
        public User LastModifier { get; private set; }

        public void Apply(HouseCreated @event)
        {
            Creator = @event.Creator;
            Address = @event.Address;
        }

        public void Apply(AddressUpdated @event)
        {
            LastModifier = @event.Modifier;
            Address = @event.NewAddress;
        }
    }
}