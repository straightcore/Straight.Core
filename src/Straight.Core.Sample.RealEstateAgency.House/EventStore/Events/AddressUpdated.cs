using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.House.EventStore.Events
{
    public class AddressUpdated : DomainEventBase
    {
        public AddressUpdated(User modifier, Address address)
        {
            NewAddress = address.Clone() as Address;
            Modifier = modifier.Clone() as User;
        }

        public User Modifier { get; private set; }

        public Address NewAddress { get; private set; }
    }
}