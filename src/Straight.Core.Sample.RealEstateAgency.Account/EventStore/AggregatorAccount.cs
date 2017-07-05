using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Guard;
using Straight.Core.Sample.RealEstateAgency.Account.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Model;
using Straight.Core.Sample.RealEstateAgency.Model.Exceptions;
using Straight.Core.Sample.RealEstateAgency.Model.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Straight.Core.Sample.RealEstateAgency.Account.EventStore
{
    public class AggregatorAccount : AggregatorBase<IDomainEvent>
        //, IHandlerDomainCommand<CreateAccountCommand>
        //, IHandlerDomainCommand<CreateEmployeAccountCommand>
        //, IHandlerDomainCommand<UpdateCustomersCommand>
        //, IHandlerDomainCommand<AttachCustomersCommand>
        //, IHandlerDomainCommand<AddVisitCommand>
        //, IApplyEvent<EmployeAccountCreated>
        //, IApplyEvent<AccountCreated>
        //, IApplyEvent<CustomerUpdated>
        //, IApplyEvent<CustomerAttached>
        //, IApplyEvent<VisitAdded>
    {
        private readonly SortedSet<DateTime> _allMeetDates = new SortedSet<DateTime>();
        private User _creator;
        private ImmutableDictionary<Guid, Customer> _customers = ImmutableDictionary<Guid, Customer>.Empty;
        private User _lastModifier;
        private ConnectionInformation _connectionInformation;

        private void Apply(AccountCreated @event)
        {
            _creator = @event.Creator;
            _connectionInformation = @event.ConnectionInfo;
        }

        private void Apply(EmployeAccountCreated @event)
        {
            Apply(@event as AccountCreated);
            _customers = _customers.AddRange(@event.Customers.ToDictionary(c => c.Id, c => c));
        }

        private void Apply(CustomerAttached @event)
        {
            _lastModifier = @event.Modifier;
            _customers = _customers.Add(@event.Customer.Id, @event.Customer);
        }

        private void Apply(CustomerUpdated @event)
        {
            _lastModifier = @event.Modifier;
            _customers = _customers.SetItem(@event.Customer.Id, @event.Customer.Clone() as Customer);
        }

        private void Apply(VisitAdded @event)
        {
            _lastModifier = @event.EstateOfficier;
            _allMeetDates.Add(@event.MeetDate);
        }

        private IEnumerable Handle(AddVisitCommand command)
        {
            command.CheckIfArgumentIsNull("command");
            command.House.CheckIfArgumentIsNull("Address");
            AddressHelper.CheckMandatory(
                command.House.Address.Street,
                command.House.Address.City,
                command.House.Address.PostalCode);
            var estateOfficier = new User(command.EstateOfficierLastName,
                command.EstateOfficierFirstName,
                command.EstateOfficierUsername);
            if (IsInCurrentMeet(command.MeetDate))
                throw new DateAlreadyExistException(command.MeetDate);
            yield return new VisitAdded(command.House, estateOfficier, command.MeetDate);
        }

        private IEnumerable Handle(AttachCustomersCommand command)
        {
            command.Customers.ForEach(c => AddressHelper.CheckMandatory(c.Street, c.City, c.PostalCode));
            command.Customers.ForEach(c => CustomerHelper.CheckMandatoryCustomer(
                c.FirstName,
                c.LastName,
                c.Birthday,
                c.Email,
                c.Phone,
                c.CellPhone));
            var modifier = new User(command.ModifierLastName, command.ModifierFirstName, command.ModifierUsername);
            return command.Customers.Select(c => new CustomerAttached(c, modifier));
        }

        private IEnumerable Handle(CreateEmployeAccountCommand command)
        {
            command.Customers.ForEach(c => AddressHelper.CheckMandatory(c.Street, c.City, c.PostalCode));
            command.Customers.ForEach(
                                      c =>
                                          CustomerHelper.CheckMandatoryCustomer(c.FirstName, c.LastName, c.Birthday, c.Email, c.Phone,
                                                                                c.CellPhone));
            yield return new EmployeAccountCreated(command.AccountKey,
                                                   command.Login,
                                                   command.Password,
                                                   new User(command.CreatorLastName,
                                                               command.CreatorFirstName,
                                                               command.CreatorUsername)
                                                   , command.Customers);
        }

        private IEnumerable Handle(CreateAccountCommand command)
        {
            yield return new AccountCreated(
                command.AccountKey,
                command.Login,
                command.Password,
                new User(
                    command.CreatorLastName,
                    command.CreatorFirstName,
                    command.CreatorUsername));
        }

        private IEnumerable Handle(UpdateCustomersCommand command)
        {
            if (!command.Customers.All(c => _customers.ContainsKey(c.Id)))
                throw new ArgumentException("One or more customers are not found.");
            command.Customers.ForEach(c => AddressHelper.CheckMandatory(c.Street, c.City, c.PostalCode));
            command.Customers.ForEach(c => CustomerHelper.CheckMandatoryCustomer(
                c.FirstName,
                c.LastName,
                c.Birthday,
                c.Email,
                c.Phone,
                c.CellPhone));
            var modifier = new User(command.ModifierLastName, command.ModifierFirstName, command.ModifierUsername);
            return command.Customers.Select(c => new CustomerUpdated(c, modifier));
        }

        private bool IsInCurrentMeet(DateTime meetDt)
        {
            if (!_allMeetDates.Any())
                return false;
            if (_allMeetDates.Contains(meetDt))
                return true;
            if ((_allMeetDates.Count == 1)
                && IsInTimSpan(meetDt, _allMeetDates.First(), TimeSpan.FromMinutes(30)))
                return true;
            var before = _allMeetDates.LastOrDefault(dt => dt < meetDt);
            var after = _allMeetDates.FirstOrDefault(dt => dt > meetDt);
            if ((before == default(DateTime))
                && IsInTimSpan(meetDt, before, TimeSpan.FromMinutes(30)))
                return true;
            return (after == default(DateTime))
                   && IsInTimSpan(meetDt, after, TimeSpan.FromMinutes(30));
        }

        private static bool IsInTimSpan(DateTime actual, DateTime expected, TimeSpan timeOfVisit)
        {
            return (actual > expected ? actual - expected : expected - actual) < timeOfVisit;
        }

    }
}