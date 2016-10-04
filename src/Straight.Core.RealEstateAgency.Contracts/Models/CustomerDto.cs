using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Models
{
    [Serializable]
    [DataContract]
    public class CustomerDto : ICloneable
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public DateTime Birthday { get; set; }
        [DataMember]
        public GenderDto Gender { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public PhoneDto Phone { get; set; }
        [DataMember]
        public PhoneDto CellPhone { get; set; }
        [DataMember]
        public AddressDto Address { get; set; }
        [DataMember]
        public Guid Id { get; set; }

        public object Clone()
        {
            var clone = MemberwiseClone() as CustomerDto;
            clone.Id = Id;
            clone.Phone = this.Phone?.Clone() as PhoneDto;
            clone.CellPhone = CellPhone?.Clone() as PhoneDto;
            clone.Address = Address?.Clone() as AddressDto;
            return clone;
        }
    }

    [DataContract]
    public enum GenderDto
    {
        Mr,
        Mrs,
        Miss
    }
}