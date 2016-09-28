using System;
using System.Runtime.Serialization;
using Straight.Core.Command;

namespace Straight.Core.Sample.RealEstateAgency.House.Command
{
    [Serializable]
    [DataContract]
    public class CreateHouseCommandHandler : ICommand
    {
        [DataMember]
        public RequesterDto Creator { get; set; }
        [DataMember]
        public AddressDto Address { get; set; }
    }

    [Serializable]
    [DataContract]
    public class AddressDto
    {
        [DataMember]
        public string AdditionalAddress { get; set; }
        [DataMember]
        public string Street { get; set; }
        [DataMember]
        public string StreetNumber { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string City { get; set; }
    }

    [Serializable]
    [DataContract]
    public class RequesterDto
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Username { get; set; }
    }
}