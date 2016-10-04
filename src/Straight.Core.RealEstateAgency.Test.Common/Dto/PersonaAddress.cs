using System.Collections.Generic;
using Straight.Core.RealEstateAgency.Contracts.Models;

namespace Straight.Core.RealEstateAgency.Test.Common.Dto
{
    public static class PersonaAddressDto
    {
        public static IEqualityComparer<AddressDto> AddressValueComparer { get; } = new AddressDtoComparer();

        public static AddressDto NationalMuseumNewYork { get; } = new AddressDto
        {
            Street = "Central Park West",
            AdditionalAddress = "",
            StreetNumber = "Cross 79th Street",
            PostalCode = "10024",
            City = "New York"
        };

        public static AddressDto NationalMuseumNewYorkMistakeInWashington { get; } = new AddressDto
        {

            Street = "Central Park South",
            AdditionalAddress = "",
            StreetNumber = "Cross 54 Street",
            PostalCode = "10024",
            City = "Washington"
        };

    }

    internal class AddressDtoComparer : IEqualityComparer<AddressDto>
    {
        public bool Equals(AddressDto x, AddressDto y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return x.AdditionalAddress.Equals(y.AdditionalAddress)
                   && x.City.Equals(y.City)
                   && x.PostalCode.Equals(y.PostalCode)
                   && x.StreetNumber.Equals(y.StreetNumber)
                   && x.Street.Equals(y.Street);
        }

        public int GetHashCode(AddressDto obj)
        {
            return -1;
        }
    }

}