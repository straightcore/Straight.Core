using System;
using System.Runtime.Serialization;
using Straight.Core.Command;

namespace Straight.Core.Sample.RealEstateAgency.House.Command
{
    [Serializable]
    [DataContract]
    public class UpdateHouseCommandHandler : ICommand
    {
        [DataMember]
        public RequesterDto Modifier { get; set; }
        [DataMember]
        public Guid HouseId { get; set; }
        [DataMember]
        public AddressDto Address { get; set; }

    }
}