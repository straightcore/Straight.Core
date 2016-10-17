using System;
using System.Runtime.Serialization;
using Straight.Core.Command;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account
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