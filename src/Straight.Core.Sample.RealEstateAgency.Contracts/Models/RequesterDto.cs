using System.Runtime.Serialization;

namespace Straight.Core.Sample.RealEstateAgency.Contracts.Models
{
    
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