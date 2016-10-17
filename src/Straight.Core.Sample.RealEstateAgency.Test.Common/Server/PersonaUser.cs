using System.Collections.Generic;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Test.Common.Server
{
    public static class PersonaUser
    {
        public static IEqualityComparer<User> UserValueComparer { get; } = new UserComparer();

        public static User John { get; } = new User("Doe", "John", "john.doe");

        public static User Jane { get; } = new User("Doe", "Jane", "jane.doe");
    }

    internal class UserComparer : IEqualityComparer<User>
    {
        public bool Equals(User x, User y)
        {
            if ((x == null) && (y == null))
                return true;
            if ((x == null) || (y == null))
                return false;
            return x.FirstName.Equals(y.FirstName)
                   && x.LastName.Equals(y.LastName)
                   && x.Username.Equals(y.Username);
        }

        public int GetHashCode(User obj)
        {
            return -1;
        }
    }
}