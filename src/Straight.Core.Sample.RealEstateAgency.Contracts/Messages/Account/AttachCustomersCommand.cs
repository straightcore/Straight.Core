using Straight.Core.Command;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using System;
using System.Runtime.Serialization;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account
{
    
    [DataContract]
    public class AttachCustomersCommand : ICommand
    {
        [DataMember]
        public RequesterDto Modifier { get; set; }

        [DataMember]
        public CustomerDto[] Customers { get; set; }

        [DataMember]
        public Guid Id { get; set; }
    }
}