using System.Collections.Generic;
using Straight.Core.Domain;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Account.Domain.Command
{
    public sealed class CreateAccountCommand : DomainCommandBase
    {
        public IEnumerable<Customer> Customers { get; set; }

        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreatorUsername { get; set; }

        public string AccountKey { get; set; }
    }
}