using Straight.Core.Command;
using Straight.Core.RealEstateAgency.Contracts.Models;
using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Messages.Account
{
    [Serializable]
    [DataContract]
    public sealed class CreateAccountCommand : ICommand
    {
        [DataMember]
        public CustomerDto[] Customers { get; set; }

        [DataMember]
        public RequesterDto Creator { get; set; }
    }
}