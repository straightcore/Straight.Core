using System;
using System.Runtime.Serialization;
using Straight.Core.RealEstateAgency.Contracts.Models;
using ICommand = Straight.Core.Command.ICommand;

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