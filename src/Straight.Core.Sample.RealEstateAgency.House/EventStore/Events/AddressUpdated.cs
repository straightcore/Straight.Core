using System;
using Straight.Core.EventStore;

namespace Straight.Core.Sample.RealEstateAgency.House.EventStore.Events
{
    public class AddressUpdated : IDomainEvent
    {
        public AddressUpdated(User modifier, Address address)
        {
            Id = Guid.NewGuid();
            NewAddress = address;
            Modifier = modifier;
        }

        public User Modifier { get; set; }

        public Address NewAddress { get; private set; }

        public Guid Id { get; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }



    }
}