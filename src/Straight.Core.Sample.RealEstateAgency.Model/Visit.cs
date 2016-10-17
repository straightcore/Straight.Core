using System;

namespace Straight.Core.Sample.RealEstateAgency.Model
{
    public class Visit
    {
        public Visit(User estateOfficer, IAccount account, DateTime meet)
        {
            EstateOfficier = estateOfficer;
            Account = account;
            Meet = meet;
        }

        public User EstateOfficier { get; private set; }

        public IAccount Account { get; private set; }

        public DateTime Meet { get; private set; }
    }
}