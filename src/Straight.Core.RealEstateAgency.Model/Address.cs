using System;

namespace Straight.Core.RealEstateAgency.Model
{
    public class Address : ICloneable
    {
        public string AdditionalAddress { get; private set; }
        public string Street { get; private set; }
        public string StreetNumber { get; private set; }
        public string PostalCode { get; private set; }
        public string City { get; private set; }

        public Address(string street, string streetNumber, string additionalAddress, string postalCode, string city)
        {
            Street = street;
            StreetNumber = streetNumber;
            PostalCode = postalCode;
            City = city;
            AdditionalAddress = additionalAddress;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}