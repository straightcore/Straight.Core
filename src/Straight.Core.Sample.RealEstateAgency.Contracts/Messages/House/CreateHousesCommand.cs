using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using System.Runtime.Serialization;
using Straight.Core.Common.Command;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.House
{
    
    [DataContract]
    public class CreateHouseCommandHandler : ICommand
    {
        [DataMember]
        public RequesterDto Creator { get; set; }

        [DataMember]
        public AddressDto Address { get; set; }
    }
}