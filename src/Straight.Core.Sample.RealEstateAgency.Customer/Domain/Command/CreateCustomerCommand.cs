using System;
using Straight.Core.Domain;

namespace Straight.Core.RealEstateAgency.Customer.Domain.Command
{
    public class CreateCustomerCommand : IDomainCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string FirstName { get; set; }
        public string 

        public string AdditionalAddress { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreatorUsername { get; set; }
    }
}