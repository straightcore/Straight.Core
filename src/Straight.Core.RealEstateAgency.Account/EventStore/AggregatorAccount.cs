using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.RealEstateAgency.Account.Domain.Command;
using Straight.Core.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.RealEstateAgency.Model;
using Straight.Core.RealEstateAgency.Model.Helper;

namespace Straight.Core.RealEstateAgency.Account.EventStore
{
    public class AggregatorAccount : AggregatorBase<IDomainEvent>
        , IHandlerDomainCommand<CreateAccountCommand>
        , IApplyEvent<AccountCreated>
        , IHandlerDomainCommand<UpdateCustomersCommand>
        , IApplyEvent<CustomerUpdated>
        , IHandlerDomainCommand<AttachCustomersCommand>
        , IApplyEvent<CustomerAttached>
    {
        private User _creator;
        private ImmutableDictionary<Guid, Customer> _customers = ImmutableDictionary<Guid, Customer>.Empty;
        private User _lastModifier;

        public IEnumerable Handle(CreateAccountCommand command)
        {
            command.Customers.ForEach(c => AddressHelper.CheckMandatory(c.Street, c.City, c.PostalCode));
            command.Customers.ForEach(
                c =>
                    CustomerHelper.CheckMandatoryCustomer(c.FirstName, c.LastName, c.Birthday, c.Email, c.Phone,
                        c.CellPhone));
            yield return new AccountCreated(
                command.AccountKey,
                new User(
                    command.CreatorLastName,
                    command.CreatorFirstName,
                    command.CreatorUsername)
                , command.Customers);
        }

        public void Apply(AccountCreated @event)
        {
            _creator = @event.Creator;
            _customers = _customers.AddRange(@event.Customers.ToDictionary(c => c.Id, c => c));
        }

        public IEnumerable Handle(UpdateCustomersCommand command)
        {
            if (!command.Customers.All(c => _customers.ContainsKey(c.Id)))
            {
                throw new ArgumentException("One or more customers are not found.");
            }
            command.Customers.ForEach(c => AddressHelper.CheckMandatory(c.Street, c.City, c.PostalCode));
            command.Customers.ForEach(
                c =>
                    CustomerHelper.CheckMandatoryCustomer(c.FirstName, c.LastName, c.Birthday, c.Email, c.Phone,
                        c.CellPhone));
            var modifier = new User(command.ModifierLastName, command.ModifierFirstName, command.ModifierUsername);
            return command.Customers.Select(c => new CustomerUpdated(c, modifier));
        }

        public void Apply(CustomerUpdated @event)
        {
            _lastModifier = @event.Modifier;
            _customers = _customers.SetItem(@event.Customer.Id, @event.Customer.Clone() as Customer);
        }

        public IEnumerable Handle(AttachCustomersCommand command)
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

        public void Apply(CustomerAttached @event)
        {
            _lastModifier = @event.Modifier;
            _customers = _customers.Add(@event.Customer.Id, @event.Customer);
        }
    }
}