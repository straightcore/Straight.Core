using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.RealEstateAgency.Account.EventStore.Events
{
    public class AccountCreated : DomainEventBase
    {
        public AccountCreated(User creator, IEnumerable<Customer> customers)
        {
            Creator = creator.Clone() as User;
            Customers = customers.Select(c => c.Clone() as Customer).ToList().AsReadOnly();
        }
        
        public User Creator { get; private set; }

        public IEnumerable<Customer> Customers { get; private set; }
    }
}