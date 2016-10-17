using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Sample.RealEstateAgency.Account.Command;
using Straight.Core.Sample.RealEstateAgency.Account.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Contracts.Models;
using Straight.Core.Sample.RealEstateAgency.Test.Common;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Dto;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using AttachCustomersCommand = Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account.AttachCustomersCommand;
using UpdateCustomersCommand = Straight.Core.Sample.RealEstateAgency.Contracts.Messages.Account.UpdateCustomersCommand;

namespace Straight.Core.Sample.RealEstateAgency.Account.Tests.Command
{
    [TestFixture]
    public class AccountCommandHandlerTests
    {
        [SetUp]
        public void Setup()
        {
            _repository = GetRepository();
            _commandHandler = new AccountCommandHandler(_repository);
            GenerateAccountEvent();
        }

        private AccountCommandHandler _commandHandler;
        private IDomainEventStore<IDomainEvent> _repository;
        private TestDomainEventStore _testRepository;

        private void GenerateAccountEvent()
        {
            var account = new AggregatorAccount();
            account.Update(new CreateAccountCommand
            {
                Customers = new[] {PersonaCustomer.Pierre},
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

        [Test]
        public void Should_create_new_account_when_emit_create_account_command()
        {
            _commandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
            {
                Customers = new[] {PersonaCustomerDto.Virginie},
                Creator = PersonaRequesterDto.John
            });
            _repository.Received(1).Add(Arg.Any<AggregatorAccount>());
        }

        [Test]
        public void Should_throw_arguement_null_exception_when_customers_is_null_or_empty()
        {
            Assert.Throws<ArgumentNullException>(
                () => _commandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand()));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle(new AttachCustomersCommand()));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle(new UpdateCustomersCommand()));
            Assert.Throws<ArgumentNullException>(
                () =>
                    _commandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
                    {
                        Customers = new CustomerDto[0]
                    }));
            Assert.Throws<ArgumentNullException>(
                () => _commandHandler.Handle(new AttachCustomersCommand {Customers = new CustomerDto[0]}));
            Assert.Throws<ArgumentNullException>(
                () => _commandHandler.Handle(new UpdateCustomersCommand {Customers = new CustomerDto[0]}));

            Assert.Throws<ArgumentNullException>(
                () =>
                    _commandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
                    {
                        Customers = new CustomerDto[1] {PersonaCustomerDto.Virginie}
                    }));
            Assert.Throws<ArgumentNullException>(
                () =>
                    _commandHandler.Handle(new AttachCustomersCommand
                    {
                        Customers = new CustomerDto[1] {PersonaCustomerDto.Virginie}
                    }));
            Assert.Throws<ArgumentNullException>(
                () =>
                    _commandHandler.Handle(new UpdateCustomersCommand
                    {
                        Customers = new CustomerDto[1] {PersonaCustomerDto.Virginie}
                    }));
        }

        [Test]
        public void Should_throw_argument_exception_when_cannot_find_account()
        {
            Assert.Throws<ArgumentException>(() => _commandHandler.Handle(
                new UpdateCustomersCommand
                {
                    Customers = new CustomerDto[1] {PersonaCustomerDto.Virginie},
                    Modifier = PersonaRequesterDto.Jane,
                    Id = Guid.NewGuid()
                }));
            Assert.Throws<ArgumentException>(() => _commandHandler.Handle(
                new AttachCustomersCommand
                {
                    Customers = new CustomerDto[1] {PersonaCustomerDto.Virginie},
                    Modifier = PersonaRequesterDto.Jane,
                    Id = Guid.NewGuid()
                }));
        }

        [Test]
        public void Should_throw_argument_null_exception_when_command_is_null()
        {
            Assert.Throws<ArgumentNullException>(
                () => _commandHandler.Handle((Contracts.Messages.Account.CreateAccountCommand) null));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle((AttachCustomersCommand) null));
            Assert.Throws<ArgumentNullException>(() => _commandHandler.Handle((UpdateCustomersCommand) null));
        }

        [Test]
        public void Should_update_account_when_add_new_customer()
        {
            var accountInDb = _testRepository.Memory.First().Value;
            accountInDb.Clear();
            _commandHandler.Handle(new AttachCustomersCommand
            {
                Id = accountInDb.Id,
                Modifier = PersonaRequesterDto.John,
                Customers = new CustomerDto[1] {PersonaCustomerDto.Pierre}
            });
            Assert.That(accountInDb.GetChanges(), Has.Count.EqualTo(1));
        }

        [Test]
        public void Should_update_account_when_update_customer()
        {
            var accountInDb = _testRepository.Memory.First().Value;
            var customerDto = PersonaCustomerDto.Virginie.Clone() as CustomerDto;
            customerDto.Id = accountInDb.GetChanges().OfType<AccountCreated>().First().Customers.First().Id;
            accountInDb.Clear();
            customerDto.Birthday = customerDto.Birthday.AddYears(2);

            _commandHandler.Handle(new UpdateCustomersCommand
            {
                Id = accountInDb.Id,
                Modifier = PersonaRequesterDto.John,
                Customers = new CustomerDto[1] {customerDto}
            });
            var updateCustomersCommands = accountInDb.GetChanges().OfType<CustomerUpdated>().ToList();
            Assert.That(updateCustomersCommands, Has.Count.EqualTo(1));
            var @event = updateCustomersCommands.First();
            Assert.That(@event.Customer.Birthday, Is.EqualTo(customerDto.Birthday));
        }
    }
}