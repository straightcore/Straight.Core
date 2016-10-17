using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Account.Domain
{
    public class Account : ReadModelBase<IDomainEvent>, IAccount
        , IApplyEvent<AccountCreated>
        , IApplyEvent<CustomerUpdated>
    {
        private ImmutableList<Customer> _customers;

        public User Creator { get; private set; }
        public IEnumerable<Customer> Customers => _customers;
        public User LastModifier { get; private set; }

        public void Apply(AccountCreated @event)
        {
            Creator = @event.Creator;
            _customers = ImmutableList<Customer>.Empty.AddRange(@event.Customers);
        }

        public void Apply(CustomerUpdated @event)
        {
            LastModifier = @event.Modifier;
            var oldCustomers = _customers.Single(c => c.Id == @event.Id);
            _customers = _customers.Replace(oldCustomers, @event.Customer);
        }
    }
}