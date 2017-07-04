using System;

namespace Straight.Core.Sample.RealEstateAgency.Model
{
    public class ConnectionInformation
    {
        
        public ConnectionInformation(string login, string password)
        {
            Login = login;

            Password = password;
        }

        public string Login { get; }

        public string Password { get; }
        
    }
}
