using Straight.Core.Domain;
using Straight.Core.Sample.RealEstateAgency.Model;
using System.Collections.Generic;

namespace Straight.Core.Sample.RealEstateAgency.Account.Domain.Command
{
    public class UpdateCustomersCommand : DomainCommandBase
    {
        public string ModifierFirstName { get; set; }
        public string ModifierLastName { get; set; }
        public string ModifierUsername { get; set; }

        public IEnumerable<Customer> Customers { get; set; }
    }
}