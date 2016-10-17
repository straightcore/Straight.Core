using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Extensions.Guard;
using Straight.Core.Sample.RealEstateAgency.Model;
using Straight.Core.Sample.RealEstateAgency.Model.Exceptions;
using Straight.Core.Sample.RealEstateAgency.Model.Helper;
using Straight.Core.Sample.RealEstateAgency.House.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Straight.Core.Sample.RealEstateAgency.House.EventStore
{
    public class AggregatorHouse : AggregatorBase<IDomainEvent>
        , IHandlerDomainCommand<CreateHouseCommand>
        , IApplyEvent<HouseCreated>
        , IHandlerDomainCommand<UpdateAddressCommand>
        , IApplyEvent<AddressUpdated>
        , IHandlerDomainCommand<AddVisitHouseCommand>
        , IApplyEvent<VisitAdded>
    {
        private readonly SortedSet<DateTime> _allMeetDateTimes = new SortedSet<DateTime>();
        private Address _address;
        private User _creator;
        private User _lastModifier;

        public void Apply(AddressUpdated @event)
        {
            _address = @event.NewAddress;
            _lastModifier = @event.Modifier;
        }

        public void Apply(HouseCreated @event)
        {
            _address = @event.Address;
            _creator = @event.Creator;
        }

        public void Apply(VisitAdded @event)
        {
            _lastModifier = @event.EstateOfficer;
            _allMeetDateTimes.Add(@event.MeetDateTime);
        }

        public IEnumerable Handle(AddVisitHouseCommand command)
        {
            command.CheckIfArgumentIsNull("houseCommand");
            command.Account.CheckIfArgumentIsNull("Account");
            command.EstateOfficer.CheckIfArgumentIsNull("EstateOfficer");
            if (IsInCurrentMeet(command.MeetDate))
            {
                throw new DateAlreadyExistException(command.MeetDate);
            }
            yield return new VisitAdded(
                command.EstateOfficer.Clone() as User,
                command.Account,
                command.MeetDate);
        }

        public IEnumerable Handle(CreateHouseCommand command)
        {
            AddressHelper.CheckMandatory(command.Street, command.City, command.PostalCode);
            UserHelper.CheckMandatoryUser(command.CreatorFirstName, command.CreatorLastName, command.CreatorUsername);
            var address = new Address(command.Street, command.StreetNumber, command.AdditionalAddress,
                command.PostalCode, command.City);
            var creator = new User(command.CreatorFirstName, command.CreatorLastName, command.CreatorUsername);
            yield return new HouseCreated(creator, address);
        }

        public IEnumerable Handle(UpdateAddressCommand command)
        {
            command.CheckIfArgumentIsNull("houseCommand");
            AddressHelper.CheckMandatory(command.Street, command.City, command.PostalCode);
            UserHelper.CheckMandatoryUser(command.FirstName, command.LastName, command.Username);
            var address = new Address(command.Street, command.StreetNumber, command.AdditionalAddress,
                command.PostalCode, command.City);
            var modifier = new User(command.LastName, command.FirstName, command.Username);
            yield return new AddressUpdated(modifier, address);
        }

        private bool IsInCurrentMeet(DateTime meetDt)
        {
            if (!_allMeetDateTimes.Any())
                return false;
            if (_allMeetDateTimes.Contains(meetDt))
                return true;
            if ((_allMeetDateTimes.Count == 1)
                && IsInTimSpan(meetDt, _allMeetDateTimes.First(), TimeSpan.FromMinutes(30)))
                return true;
            var before = _allMeetDateTimes.LastOrDefault(dt => dt < meetDt);
            var after = _allMeetDateTimes.FirstOrDefault(dt => dt > meetDt);
            if ((before == default(DateTime))
                && IsInTimSpan(meetDt, before, TimeSpan.FromMinutes(30)))
                return true;
            if ((after == default(DateTime))
                && IsInTimSpan(meetDt, after, TimeSpan.FromMinutes(30)))
                return true;
            return false;
        }

        private static bool IsInTimSpan(DateTime actual, DateTime expected, TimeSpan timeOfVisit)
        {
            return (actual > expected ? actual - expected : expected - actual) < timeOfVisit;
        }
    }
}