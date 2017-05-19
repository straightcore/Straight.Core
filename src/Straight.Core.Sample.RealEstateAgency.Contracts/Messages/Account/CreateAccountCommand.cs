using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using System.Runtime.Serialization;
using Straight.Core.Common.Command;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account
{
    
    [DataContract]
    public sealed class CreateAccountCommand : ICommand
    {
        [DataMember]
        public CustomerDto[] Customers { get; set; }

        [DataMember]
        public RequesterDto Creator { get; set; }
    }
}