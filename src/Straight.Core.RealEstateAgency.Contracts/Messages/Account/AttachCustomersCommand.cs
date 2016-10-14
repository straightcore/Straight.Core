using Straight.Core.Command;
using Straight.Core.RealEstateAgency.Contracts.Models;
using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Messages.Account
{
    [Serializable]
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