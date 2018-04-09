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
using NUnit.Framework;

namespace Straight.Core.Sample.RealEstateAgency.Account.Tests.Command
{
    [TestFixture]
    public class AccountCommandHandlerTests
    {
        private class ContextTest
        {
            public ContextTest()
            {
                TestRepository = new TestDomainEventStore();
                Repository = GetRepository();
                CommandHandler = new AccountCommandHandler(Repository, new RealEstateAgencyModelConverter());
                GenerateAccountEvent(this);
            }

            public AccountCommandHandler CommandHandler { get; } 
            public IDomainEventStore<IDomainEvent> Repository { get; }
            public TestDomainEventStore TestRepository { get; }

            private IDomainEventStore<IDomainEvent> GetRepository()
            {                
                var repoSubstitute = Substitute.For<IDomainEventStore<IDomainEvent>>();
                repoSubstitute.When(r => r.Add(Arg.Any<AggregatorAccount>()))
                    .Do(info => TestRepository.Add(info.Arg<AggregatorAccount>()));
                repoSubstitute.GetById<AggregatorAccount>(Arg.Any<Guid>())
                    .Returns(info => TestRepository.GetById<AggregatorAccount>(info.Arg<Guid>()));
                return repoSubstitute;
            }

            private void GenerateAccountEvent(ContextTest context)
            {
                var account = new AggregatorAccount();
                account.Update(new CreateEmployeAccountCommand
                {
                    Customers = new[] { PersonaCustomer.Pierre },
                    CreatorUsername = PersonaUser.John.Username,
                    CreatorLastName = PersonaUser.John.LastName,
                    CreatorFirstName = PersonaUser.John.FirstName
                });
                context.TestRepository.Add(account);
            }
        }


        [Test]
        public void Should_create_new_account_when_emit_create_account_command()
        {
            var context = new ContextTest();
            context.CommandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
            {
                Customers = new[] { PersonaCustomerDto.Virginie },
                Creator = PersonaRequesterDto.John
            });
            context.Repository.Received(1).Add(Arg.Any<AggregatorAccount>());
        }

        [Test]
        public void Should_throw_arguement_null_exception_when_customers_is_null_or_empty()
        {
            var context = new ContextTest();
            Assert.Throws<ArgumentNullException>(() => context.CommandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand()));
            Assert.Throws<ArgumentNullException>(() => context.CommandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand()));
            Assert.Throws<ArgumentNullException>(() => context.CommandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand()));
            Assert.Throws<ArgumentNullException>(() => context.CommandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
                    {
                        Customers = new CustomerDto[0]
                    }));
            Assert.Throws<ArgumentNullException>(
                () => context.CommandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand { Customers = new CustomerDto[0] }));
            Assert.Throws<ArgumentNullException>(
                () => context.CommandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand { Customers = new CustomerDto[0] }));

            Assert.Throws<ArgumentNullException>(
                () =>
                    context.CommandHandler.Handle(new Contracts.Messages.Account.CreateAccountCommand
                    {
                        Customers = new[] { PersonaCustomerDto.Virginie }
                    }));
            Assert.Throws<ArgumentNullException>(
                () =>
                    context.CommandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand
                    {
                        Customers = new[] { PersonaCustomerDto.Virginie }
                    }));
            Assert.Throws<ArgumentNullException>(
                () =>
                    context.CommandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand
                    {
                        Customers = new[] { PersonaCustomerDto.Virginie }
                    }));
        }

        [Test]
        public void Should_throw_argument_exception_when_cannot_find_account()
        {
            var context = new ContextTest();
            Assert.Throws<ArgumentException>(() => context.CommandHandler.Handle(
                new Contracts.Messages.Account.UpdateCustomersCommand
                {
                    Customers = new[] { PersonaCustomerDto.Virginie },
                    Modifier = PersonaRequesterDto.Jane,
                    Id = Guid.NewGuid()
                }));
            Assert.Throws<ArgumentException>(() => context.CommandHandler.Handle(
                new Contracts.Messages.Account.AttachCustomersCommand
                {
                    Customers = new[] { PersonaCustomerDto.Virginie },
                    Modifier = PersonaRequesterDto.Jane,
                    Id = Guid.NewGuid()
                }));
        }

        [Test]
        public void Should_throw_argument_null_exception_when_command_is_null()
        {
            var context = new ContextTest();
            Assert.Throws<ArgumentNullException>(
                () => context.CommandHandler.Handle((Contracts.Messages.Account.CreateAccountCommand)null));
            Assert.Throws<ArgumentNullException>(() => context.CommandHandler.Handle((Contracts.Messages.Account.AttachCustomersCommand)null));
            Assert.Throws<ArgumentNullException>(() => context.CommandHandler.Handle((Contracts.Messages.Account.UpdateCustomersCommand)null));
        }

        [Test]
        public void Should_update_account_when_add_new_customer()
        {
            var context = new ContextTest();
            var accountInDb = context.TestRepository.Memory.First().Value;
            accountInDb.Clear();
            context.CommandHandler.Handle(new Contracts.Messages.Account.AttachCustomersCommand
            {
                Id = accountInDb.Id,
                Modifier = PersonaRequesterDto.John,
                Customers = new[] { PersonaCustomerDto.Pierre }
            });
            Assert.That(accountInDb.GetChanges().Count(), Is.EqualTo(1));
        }

        [Test]
        public void Should_update_account_when_update_customer()
        {
            var context = new ContextTest();
            var accountInDb = context.TestRepository.Memory.First().Value;
            var customerDto = PersonaCustomerDto.Virginie.Clone() as CustomerDto;
            customerDto.Id = accountInDb.GetChanges().OfType<EmployeAccountCreated>().First().Customers.First().Id;
            accountInDb.Clear();
            customerDto.Birthday = customerDto.Birthday.AddYears(2);

            context.CommandHandler.Handle(new Contracts.Messages.Account.UpdateCustomersCommand
            {
                Id = accountInDb.Id,
                Modifier = PersonaRequesterDto.John,
                Customers = new[] { customerDto }
            });
            var updateCustomersCommands = accountInDb.GetChanges().OfType<CustomerUpdated>().ToList();
            Assert.That(updateCustomersCommands.Count(), Is.EqualTo(1));
            var @event = updateCustomersCommands.First();
            Assert.That(@event.Customer.Birthday, Is.EqualTo(customerDto.Birthday));
        }
    }
}