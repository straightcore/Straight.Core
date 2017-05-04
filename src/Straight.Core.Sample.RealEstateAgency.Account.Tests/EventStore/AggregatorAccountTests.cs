using Xunit;
using Straight.Core.Sample.RealEstateAgency.Account.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using System.Linq;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Account.Tests.EventStore
{
    
    public class AggregatorAccountTests
    {
        
        public AggregatorAccountTests()
        {
            _account = new AggregatorAccount();

            _createAccountCommand = new CreateAccountCommand
            {
                CreatorFirstName = PersonaUser.John.FirstName,
                CreatorLastName = PersonaUser.John.LastName,
                CreatorUsername = PersonaUser.John.Username,
                Customers = new[]
                {
                    PersonaCustomer.Pierre
                }
            };
            _account.Update(_createAccountCommand);
        }

        private readonly AggregatorAccount _account;
        private readonly CreateAccountCommand _createAccountCommand;

        [Fact]
        public void Should_add_customer_when_execute_attach_customer_command()
        {
            _account.Clear();
            var attachCustomersCommand = new AttachCustomersCommand
            {
                ModifierUsername = PersonaUser.Jane.Username,
                ModifierFirstName = PersonaUser.Jane.FirstName,
                ModifierLastName = PersonaUser.Jane.LastName,
                Customers = new Customer[1]
                {
                    PersonaCustomer.Virginie
                }
            };
            _account.Update(attachCustomersCommand);
            Assert.Equal(_account.GetChanges().Count(), 1);
            Assert.Equal(_account.GetChanges().First().AggregateId, _account.Id);
            var accountCreated = _account.GetChanges().OfType<CustomerAttached>().First();
            Assert.NotNull(accountCreated);
            Assert.Equal(accountCreated.Modifier, PersonaUser.Jane, PersonaUser.UserValueComparer);
            Assert.Equal(accountCreated.Customer, attachCustomersCommand.Customers.First(), PersonaCustomer.CustomerValueComparer);
        }

        [Fact]
        public void Should_have_account_with_customer_when_create_new_account()
        {
            Assert.Equal(_account.GetChanges().Count(), 1);
            Assert.Equal(_account.GetChanges().First().AggregateId, _account.Id);
            var accountCreated = _account.GetChanges().OfType<AccountCreated>().First();
            Assert.NotNull(accountCreated);
            Assert.Equal(accountCreated.Creator, PersonaUser.John, PersonaUser.UserValueComparer);
            Assert.Equal(accountCreated.Customers, _createAccountCommand.Customers, PersonaCustomer.CustomerValueComparer);
        }

        [Fact]
        public void Should_update_customer_when_execute_update_customer_command()
        {
            _account.Clear();
            var pierre = PersonaCustomer.Pierre.Clone() as Customer;
            pierre.Birthday = pierre.Birthday.AddDays(2);
            var attachCustomersCommand = new UpdateCustomersCommand
            {
                ModifierUsername = PersonaUser.Jane.Username,
                ModifierFirstName = PersonaUser.Jane.FirstName,
                ModifierLastName = PersonaUser.Jane.LastName,
                Customers = new Customer[1]
                {
                    pierre
                }
            };
            _account.Update(attachCustomersCommand);
            Assert.Equal(_account.GetChanges().Count(), 1);
            Assert.Equal(_account.GetChanges().First().AggregateId, _account.Id);
            var accountCreated = _account.GetChanges().OfType<CustomerUpdated>().First();
            Assert.NotNull(accountCreated);
            Assert.Equal(accountCreated.Modifier, PersonaUser.Jane, PersonaUser.UserValueComparer);
            Assert.Equal(accountCreated.Customer, attachCustomersCommand.Customers.First(), PersonaCustomer.CustomerValueComparer);
        }
    }
}