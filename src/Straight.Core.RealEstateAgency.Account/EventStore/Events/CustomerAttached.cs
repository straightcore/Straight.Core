using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.RealEstateAgency.Account.EventStore.Events
{
    public sealed class CustomerAttached : DomainEventBase
    {
        public User Modifier { get; private set; }
        public Customer Customer { get; private set; }

        public CustomerAttached(Customer c, User modifier)
        {
            Modifier = modifier;
            this.Customer = c;
        }
    }
}