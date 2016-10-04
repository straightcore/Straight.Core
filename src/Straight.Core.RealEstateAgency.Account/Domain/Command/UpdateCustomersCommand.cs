using System.Collections.Generic;
using Straight.Core.Domain;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.RealEstateAgency.Account.Domain.Command
{
    public class UpdateCustomersCommand : DomainCommandBase
    {
        public string ModifierFirstName { get; set; }
        public string ModifierLastName { get; set; }
        public string ModifierUsername { get; set; }

        public IEnumerable<Customer> Customers { get; set; }
    }
}