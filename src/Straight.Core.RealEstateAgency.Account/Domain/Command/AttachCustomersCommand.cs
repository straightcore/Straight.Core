using Straight.Core.Domain;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.RealEstateAgency.Account.Domain.Command
{
    public class AttachCustomersCommand : DomainCommandBase
    {
        public string ModifierFirstName { get; set; }
        public string ModifierLastName { get; set; }
        public string ModifierUsername { get; set; }

        public Customer[] Customers { get; set; }
    }
}