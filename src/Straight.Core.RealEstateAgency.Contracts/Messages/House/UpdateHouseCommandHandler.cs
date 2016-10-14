using Straight.Core.Command;
using Straight.Core.RealEstateAgency.Contracts.Models;
using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Messages.House
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