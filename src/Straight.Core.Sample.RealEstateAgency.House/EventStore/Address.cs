namespace Straight.Core.Sample.RealEstateAgency.House.EventStore
{
    public class Address
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
    }
}