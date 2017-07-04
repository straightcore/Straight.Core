using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.Model;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events
{
    public class AccountCreated : DomainEventBase
    {
        public AccountCreated(string accountKey, string login, string password, User creator)
        {
            ConnectionInfo = new ConnectionInformation(login, password);
            AccountKey = accountKey;
            Creator = creator.Clone() as User;
        }

        public string AccountKey { get; }

        public ConnectionInformation ConnectionInfo { get; }

        public User Creator { get; }
    }

    public class EmployeAccountCreated : AccountCreated
    {
        public EmployeAccountCreated(string accountKey, string login, string password, User creator, IEnumerable<Customer> customers)
            : base(accountKey, login, password, creator)
        {
            Customers = customers.Select(c => c.Clone() as Customer).ToList().AsReadOnly();
        }

        public string AccountKey { get; }

        public ConnectionInformation ConnectionInfo { get; }

        public User Creator { get; }

        public IEnumerable<Customer> Customers { get; }
    }
}