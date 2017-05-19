using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using System;
using System.Runtime.Serialization;
using Straight.Core.Common.Command;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.House
{
    
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