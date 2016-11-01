using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Straight.Core.Sample.RealEstateAgency.Test.Common.Dto
{
    public static class PersonaCustomerDto
    {
        public static IEqualityComparer<CustomerDto> CustomerValueComparer { get; } = new CustomerDtoEqualityComparer();

        public static CustomerDto Pierre { get; } = new CustomerDto
        {
            FirstName = "Pierre",
            LastName = "Durand",
            Address = new AddressDto
            {
                PostalCode = "75009",
                Street = "rue des Martyres",
                StreetNumber = "123 bis",
                City = "Paris"
            },
            CellPhone = new PhoneDto {Number = "0701020304", CountryCode = "33"},
            Birthday = new DateTime(1979, 08, 12),
            Gender = GenderDto.Mr,
            Email = "pierre.durand@fake.com"
        };

        public static CustomerDto Virginie { get; } = new CustomerDto
        {
            FirstName = "Virginie",
            LastName = "Eclin",
            Address = new AddressDto
            {
                PostalCode = "06200",
                Street = "rue des Antibes",
                StreetNumber = "23",
                City = "Saint Laurent du Var"
            },
            CellPhone = new PhoneDto {Number = "0612345678", CountryCode = "33"},
            Birthday = new DateTime(1983, 03, 07),
            Gender = GenderDto.Miss,
            Email = "e.nini@fake.com"
        };
    }

    internal class CustomerDtoEqualityComparer : IEqualityComparer<CustomerDto>
    {
        private static readonly List<PropertyInfo> Properties;

        static CustomerDtoEqualityComparer()
        {
            var type = typeof(CustomerEqualityComparer);
            Properties = type.GetProperties(BindingFlags.Public).ToList();
        }

        public bool Equals(CustomerDto left, CustomerDto right)
        {
            return
                Properties.All(
                    info => info.GetGetMethod().Invoke(left, null) == info.GetGetMethod().Invoke(right, null));
        }

        public int GetHashCode(CustomerDto obj)
        {
            return -1;
        }
    }
}