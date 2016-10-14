using System;
using System.Runtime.Serialization;

namespace Straight.Core.RealEstateAgency.Contracts.Models
{
    [Serializable]
    [DataContract]
    public class RequesterDto
    {
        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Username { get; set; }
    }
}