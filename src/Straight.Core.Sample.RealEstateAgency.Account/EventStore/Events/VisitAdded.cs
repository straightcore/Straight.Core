using System;
using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events
{
    public class VisitAdded : DomainEventBase
    {
        public IHouse House { get; private set; }
        public User EstateOfficier { get; private set; }
        public DateTime MeetDate { get; private set; }

        public VisitAdded(IHouse house, User estateOfficier, DateTime meetDate)
        {
            House = house;
            EstateOfficier = estateOfficier;
            MeetDate = meetDate;
        }
    }
}