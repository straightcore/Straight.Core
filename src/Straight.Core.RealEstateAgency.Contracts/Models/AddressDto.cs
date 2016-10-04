using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Models
{
    [Serializable]
    [DataContract]
    public class AddressDto : ICloneable
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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

}