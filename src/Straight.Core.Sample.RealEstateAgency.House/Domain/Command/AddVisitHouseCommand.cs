using Straight.Core.Domain;
using System;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.House.Domain.Command
{
    public class AddVisitHouseCommand : DomainCommandBase
    {
        public User EstateOfficer { get; set; }
        public IAccount Account { get; set; }
        public DateTime MeetDate { get; set; }
    }
}