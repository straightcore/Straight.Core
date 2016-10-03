using System;
using System.Collections.Generic;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.RealEstateAgency.Test.Common
{
    public static class PersonaAddress
    {
        public static IEqualityComparer<Address> AddressValueComparer { get; } = new AddressComparer();

        public static Address NationalMuseumNewYork { get; } = new Address("Central Park West", "", "Cross 79th Street",
            "10024", "New York");

        public static Address NationalMuseumNewYorkMistakeInWashington { get; } = new Address("Central Park South", "",
            "Cross 54 Street", "10024", "Washington");

    }

    internal class AddressComparer : IEqualityComparer<Address>
    {
        public bool Equals(Address x, Address y)
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

        public int GetHashCode(Address obj)
        {
            return -1;
        }
    }

}