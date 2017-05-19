using Straight.Core.Command;
using Straight.Core.Domain;
using Straight.Core.Sample.RealEstateAgency.Model;
using System;
using Straight.Core.Common.Command;

namespace Straight.Core.Sample.RealEstateAgency.Account.Domain.Command
{
    public class AddVisitCommand : DomainCommandBase, ICommand
    {
        public string EstateOfficierFirstName { get; set; }
        public string EstateOfficierLastName { get; set; }
        public string EstateOfficierUsername { get; set; }
        public DateTime MeetDate { get; set; }
        public IHouse House { get; set; }
    }
}