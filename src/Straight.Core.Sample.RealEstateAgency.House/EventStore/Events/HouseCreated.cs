using System;
using Straight.Core.Domain;
using Straight.Core.EventStore;

namespace Straight.Core.Sample.RealEstateAgency.House.EventStore.Events
{
    public class HouseCreated : IDomainEvent
    {

        public HouseCreated(User creator, Address address)
        {
            Id = Guid.NewGuid();
            Address = address;
            Creator = creator;
        }

        public User Creator { get; set; }

        public Guid Id { get; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }

        public Address Address { get; }
    }
}