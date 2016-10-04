using System;
using System.Runtime.Serialization;
using Straight.Core.RealEstateAgency.Contracts.Models;
using ICommand = Straight.Core.Command.ICommand;

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