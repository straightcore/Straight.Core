using Straight.Core.Domain;
using System;

namespace Straight.Core.Sample.RealEstateAgency.House.Domain.Command
{
    public class CreateHouseCommand : IDomainCommand
    {
        public string AdditionalAddress { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        public string CreatorFirstName { get; set; }
        public string CreatorLastName { get; set; }
        public string CreatorUsername { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
    }
}