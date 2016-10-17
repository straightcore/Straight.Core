using System;
using System.Runtime.Serialization;
using Straight.Core.Command;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.House
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