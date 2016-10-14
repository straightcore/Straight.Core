using Straight.Core.Command;
using Straight.Core.RealEstateAgency.Contracts.Models;
using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Messages.House
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
}