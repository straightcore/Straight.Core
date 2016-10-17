using System;

namespace Straight.Core.Sample.RealEstateAgency.Model
{
    public class User : ICloneable
    {
        public User(string lastname, string firstname, string username)
        {
            LastName = lastname;
            FirstName = firstname;
            Username = username;
        }

        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Username { get; private set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}