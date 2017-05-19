using Straight.Core.Sample.RealEstateAgency.Account.Command;
using Straight.Core.Sample.RealEstateAgency.Account.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events;
using System;
using System.Linq;
using NSubstitute;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using Straight.Core.Sample.RealEstateAgency.Model.Contracts.Extensions;
using Straight.Core.Sample.RealEstateAgency.Test.Common;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Dto;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using Xunit;

namespace Straight.Core.Sample.RealEstateAgency.Account.Tests.Command
{
    public class AccountCommandHandlerTests
    {

        public AccountCommandHandlerTests()
        {
            _repository = GetRepository();
            _commandHandler = new AccountCommandHandler(_repository, new RealEstateAgencyModelConverter());
            GenerateAccountEvent();
        }

        private readonly AccountCommandHandler _commandHandler;
        private readonly IDomainEventStore<IDomainEvent> _repository;
        private TestDomainEventStore _testRepository;

        private void GenerateAccountEvent()
        {
            var account = new AggregatorAccount();
            account.Update(new CreateAccountCommand
            {
                Customers = new[] { PersonaCustomer.Pierre },
                CreatorUsername = PersonaUser.John.Username,
                CreatorLastName = PersonaUser.John.LastName,
                CreatorFirstName = PersonaUser.John.FirstName
            });
            _testRepository.Add(account);
        }

        private IDomainEventStore<IDomainEvent> GetRepository()
        {
            _testRepository = new TestDomainEventStore();
            var repoSubstitute = Substitute.For<IDomainEventStore<IDomainEvent>>();
            repoSubstitute.When(r => r.Add(Arg.Any<AggregatorAccount>()))
                .Do(info => _testRepository.Add(info.Arg<AggregatorAccount>()));
            repoSubstitute.GetById<AggregatorAccount>(Arg.Any<Guid>())
                .Returns(info => _testRepository.GetById<AggregatorAccount>(info.Arg<Guid>()));
            return repoSubstitute;
        }

        [Fact]
        public void Should_create_new_account_when_emit_create_account_command()
        {
            _commandHandler.Handle(new Sample.RealEstateAgency.Contracts.Messages.Account.CreateAccountCommand
            {
                Customers = new[] { PersonaCustomerDto.Virginie },
                Creator = PersonaRequesterDto.John
            });
            _repository.Received(1).Add(Arg.Any<AggregatorAccount>());
        }

        [Fact]
        public void Should_throw_arguement_null_exception_when_customers_is_null_or_empty()
        {
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand()));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand()));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand()));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
                    {
                        Customers = new CustomerDto[0]
                    }));
            Assert.Throws<ArgumentNullException>(
                () => _commandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand { Customers = new CustomerDto[0] }));
            Assert.Throws<ArgumentNullException>(
                () => _commandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand { Customers = new CustomerDto[0] }));

            Assert.Throws<ArgumentNullException>(
                () =>
                    _commandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
                    {
                        Customers = new[] { PersonaCustomerDto.Virginie }
                    }));
            Assert.Throws<ArgumentNullException>(
                () =>
                    _commandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand
                    {
                        Customers = new[] { PersonaCustomerDto.Virginie }
                    }));
            Assert.Throws<ArgumentNullException>(
                () =>
                    _commandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand
                    {
                        Customers = new[] { PersonaCustomerDto.Virginie }
                    }));
        }

        [Fact]
        public void Should_throw_argument_exception_when_cannot_find_account()
        {
            Assert.Throws<ArgumentException>(() => _commandHandler.Handle(
                new Contracts.Messages.Account.UpdateCustomersCommand
                {
                    Customers = new[] { PersonaCustomerDto.Virginie },
                    Modifier = PersonaRequesterDto.Jane,
                    Id = Guid.NewGuid()
                }));
            Assert.Throws<ArgumentException>(() => _commandHandler.Handle(
                new Contracts.Messages.Account.AttachCustomersCommand
                {
                    Customers = new[] { PersonaCustomerDto.Virginie },
                    Modifier = PersonaRequesterDto.Jane,
                    Id = Guid.NewGuid()
                }));
        }

        [Fact]
        public void Should_throw_argument_null_exception_when_command_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => _commandHandler.Handle((Contracts.Messages.Account.CreateAccountCommand)null));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle((Contracts.Messages.Account.AttachCustomersCommand)null));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle((Contracts.Messages.Account.UpdateCustomersCommand)null));
        }

        [Fact]
        public void Should_update_account_when_add_new_customer()
        {
            var accountInDb = _testRepository.Memory.First().Value;
            accountInDb.Clear();
            _commandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand
            {
                Id = accountInDb.Id,
                Modifier = PersonaRequesterDto.John,
                Customers = new[] { PersonaCustomerDto.Pierre }
            });
            Assert.Equal(accountInDb.GetChanges().Count(), 1);
        }

        [Fact]
        public void Should_update_account_when_update_customer()
        {
            var accountInDb = _testRepository.Memory.First().Value;
            var customerDto = PersonaCustomerDto.Virginie.Clone() as CustomerDto;
            customerDto.Id = accountInDb.GetChanges().OfType<AccountCreated>().First().Customers.First().Id;
            accountInDb.Clear();
            customerDto.Birthday = customerDto.Birthday.AddYears(2);

            _commandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand
            {
                Id = accountInDb.Id,
                Modifier = PersonaRequesterDto.John,
                Customers = new[] { customerDto }
            });
            var updateCustomersCommands = accountInDb.GetChanges().OfType<CustomerUpdated>().ToList();
            Assert.Equal(updateCustomersCommands.Count(), 1);
            var @event = updateCustomersCommands.First();
            Assert.Equal(@event.Customer.Birthday, customerDto.Birthday);
        }
    }
}