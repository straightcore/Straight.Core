using System;
using Straight.Core.Domain;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.House.Domain.Command
{
    public class AddVisitHouseCommand : DomainCommandBase
    {


        public User EstateOfficer { get; set; }
        public IAccount Account { get; set; }
        public DateTime MeetDateTime { get; set; }
    }
}