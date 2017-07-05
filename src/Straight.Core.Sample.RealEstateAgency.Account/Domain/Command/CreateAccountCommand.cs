using Straight.Core.Domain;
using Straight.Core.Sample.RealEstateAgency.Model;
using System.Collections.Generic;

namespace Straight.Core.Sample.RealEstateAgency.Account.Domain.Command
{
    public class CreateAccountCommand : DomainCommandBase
    {
        
        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreatorUsername { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string AccountKey { get; set; }
    }

    public sealed class CreateEmployeAccountCommand : CreateAccountCommand
    {
        public IEnumerable<Customer> Customers { get; set; }
    }
}