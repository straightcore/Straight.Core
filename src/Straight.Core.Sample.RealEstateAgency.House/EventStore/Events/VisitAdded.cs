using Straight.Core.EventStore;
using System;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.House.EventStore.Events
{
    public class VisitAdded : DomainEventBase
    {
        public VisitAdded(
            User estateOfficier, 
            IAccount account, 
            DateTime meetDateTime)
        {
            EstateOfficer = estateOfficier;
            Account = account;
            MeetDateTime = meetDateTime;
        }

        public User EstateOfficer { get; private set; }
        public IAccount Account { get; private set; }
        public DateTime MeetDateTime { get; private set; }
        
    }
}