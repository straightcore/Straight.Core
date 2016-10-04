using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Straight.Core.RealEstateAgency.Contracts.Models;
using ICommand = Straight.Core.Command.ICommand;

namespace Straight.Core.RealEstateAgency.Contracts.Messages.Account
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