using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Models
{
    [DataContract]
    public class PhoneDto : ICloneable
    {
        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}