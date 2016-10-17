using System;
using System.Runtime.Serialization;
using Straight.Core.Command;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account
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