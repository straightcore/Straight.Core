using Straight.Core.Domain;
using Straight.Core.RealEstateAgency.Model;
using System;

namespace Straight.Core.Sample.RealEstateAgency.House.Domain.Command
{
    public class AddVisitHouseCommand : DomainCommandBase
    {
        public User EstateOfficer { get; set; }
        public IAccount Account { get; set; }
        public DateTime MeetDateTime { get; set; }
    }
}