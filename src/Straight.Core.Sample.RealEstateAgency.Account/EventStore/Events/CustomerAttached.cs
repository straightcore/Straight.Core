using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events
{
    public sealed class CustomerAttached : DomainEventBase
    {
        public CustomerAttached(Customer c, User modifier)
        {
            Modifier = modifier;
            Customer = c;
        }

        public User Modifier { get; private set; }
        public Customer Customer { get; private set; }
    }
}