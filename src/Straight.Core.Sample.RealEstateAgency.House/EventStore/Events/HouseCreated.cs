using System;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.House.EventStore.Events
{
    public sealed class HouseCreated : DomainEventBase
    {
        public HouseCreated(User creator, Address address)
        {
            Address = address.Clone() as Address;
            Creator = creator.Clone() as User;
        }

        public User Creator { get; private set; }
        
        public Address Address { get; private set; }
    }
}