using System.Collections.Generic;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;

namespace Straight.Core.Sample.RealEstateAgency.Test.Common.Dto
{
    public static class PersonaRequesterDto
    {
        public static IEqualityComparer<RequesterDto> UserValueComparer { get; } = new RequesterDtoComparer();

        public static RequesterDto John { get; } = new RequesterDto
        {
            LastName = "Doe",
            FirstName = "John",
            Username = "john.doe"
        };

        public static RequesterDto Jane { get; } = new RequesterDto
        {
            LastName = "Doe",
            FirstName = "Jane",
            Username = "jane.doe"
        };
    }

    internal class RequesterDtoComparer : IEqualityComparer<RequesterDto>
    {
        public bool Equals(RequesterDto x, RequesterDto y)
        {
            if ((x == null) && (y == null))
                return true;
            if ((x == null) || (y == null))
                return false;
            return x.FirstName.Equals(y.FirstName)
                   && x.LastName.Equals(y.LastName)
                   && x.Username.Equals(y.Username);
        }

        public int GetHashCode(RequesterDto obj)
        {
            return -1;
        }
    }
}