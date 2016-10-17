using System.Collections.Generic;
using System.Linq;
using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events
{
    public class AccountCreated : DomainEventBase
    {
        public AccountCreated(string accountKey, User creator, IEnumerable<Customer> customers)
        {
            AccountKey = accountKey;
            Creator = creator.Clone() as User;
            Customers = customers.Select(c => c.Clone() as Customer).ToList().AsReadOnly();
        }

        public string AccountKey { get; private set; }

        public User Creator { get; private set; }

        public IEnumerable<Customer> Customers { get; private set; }
    }
}