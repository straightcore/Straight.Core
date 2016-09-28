using System;
using Straight.Core.Domain;

namespace Straight.Core.Sample.RealEstateAgency.House.Domain.Command
{
    public class UpdateAddressCommand : IDomainCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string AdditionalAddress { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
    }
}