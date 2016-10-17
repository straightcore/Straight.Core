using System;
using System.Runtime.Serialization;
using Straight.Core.Command;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account
{
    public class UpdateCustomersCommand : ICommand
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public RequesterDto Modifier { get; set; }

        [DataMember]
        public CustomerDto[] Customers { get; set; }
    }
}