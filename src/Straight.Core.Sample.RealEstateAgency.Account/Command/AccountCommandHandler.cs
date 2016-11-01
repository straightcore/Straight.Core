using Straight.Core.Command;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Extensions.Guard;
using Straight.Core.Sample.RealEstateAgency.Account.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore;
using Straight.Core.Sample.RealEstateAgency.Contracts.Extensions;
using System;
using System.Linq;
using AttachCustomersCommandDto =
    Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account.AttachCustomersCommand;
using CreateAccountCommandDto = Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account.CreateAccountCommand;
using UpdateCustomersCommandDto =
    Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account.UpdateCustomersCommand;

namespace Straight.Core.Sample.RealEstateAgency.Account.Command
{
    public class AccountCommandHandler : ICommandHandler<CreateAccountCommandDto>
        , ICommandHandler<UpdateCustomersCommandDto>
        , ICommandHandler<AttachCustomersCommandDto>
    {
        private readonly IDomainEventStore<IDomainEvent> _repository;

        public AccountCommandHandler(IDomainEventStore<IDomainEvent> repository)
        {
            _repository = repository;
        }

        public void Handle(AttachCustomersCommandDto command)
        {
            command.CheckIfArgumentIsNull(nameof(command));
            command.Customers.CheckIfArgumentIsNullOrEmpty(nameof(command.Customers));
            command.Modifier.CheckIfArgumentIsNull(nameof(command.Modifier));
            var account = _repository.GetById<AggregatorAccount>(command.Id);
            if (account == null)
                throw new ArgumentException($"Cannot find account ('{command.Id}')");
            command.Customers.ForEach(c => c.Id = Guid.Empty);
            account.Update(new AttachCustomersCommand
            {
                ModifierFirstName = command.Modifier.FirstName,
                ModifierLastName = command.Modifier.LastName,
                ModifierUsername = command.Modifier.Username,
                Customers = command.Customers.Select(ToModelExtensions.ToModel).ToList()
            });
        }

        public void Handle(CreateAccountCommandDto command)
        {
            command.CheckIfArgumentIsNull(nameof(command));
            command.Customers.CheckIfArgumentIsNullOrEmpty(nameof(command.Customers));
            command.Creator.CheckIfArgumentIsNull(nameof(command.Creator));
            var account = new AggregatorAccount();
            account.Update(new CreateAccountCommand
            {
                CreatorFirstName = command.Creator.FirstName,
                CreatorLastName = command.Creator.LastName,
                CreatorUsername = command.Creator.Username,
                Customers = command.Customers.Select(ToModelExtensions.ToModel).ToList()
            });
            _repository.Add(account);
        }

        public void Handle(UpdateCustomersCommandDto command)
        {
            command.CheckIfArgumentIsNull(nameof(command));
            command.Customers.CheckIfArgumentIsNullOrEmpty(nameof(command.Customers));
            command.Modifier.CheckIfArgumentIsNull(nameof(command.Modifier));
            var account = _repository.GetById<AggregatorAccount>(command.Id);
            if (account == null)
                throw new ArgumentException($"Cannot find account ('{command.Id}')");
            account.Update(new UpdateCustomersCommand
            {
                ModifierFirstName = command.Modifier.FirstName,
                ModifierLastName = command.Modifier.LastName,
                ModifierUsername = command.Modifier.Username,
                Customers = command.Customers.Select(ToModelExtensions.ToModel).ToList()
            });
        }
    }
}