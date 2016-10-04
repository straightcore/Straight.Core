using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Account.Domain.Command;
using Straight.Core.RealEstateAgency.Account.EventStore;
using Straight.Core.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.RealEstateAgency.Model;
using Straight.Core.RealEstateAgency.Test.Common;
using Straight.Core.RealEstateAgency.Test.Common.Server;

namespace Straight.Core.RealEstateAgency.Account.Tests.EventStore
{
    [TestFixture]
    public class AggregatorAccountTests
    {
        private AggregatorAccount _account;
        private CreateAccountCommand _createAccountCommand;

        [SetUp]
        public void Setup()
        {
            _account = new AggregatorAccount();

            _createAccountCommand = new CreateAccountCommand()
            {
                CreatorFirstName = PersonaUser.John.FirstName,
                CreatorLastName = PersonaUser.John.LastName,
                CreatorUsername = PersonaUser.John.Username,
                Customers = new Customer[1]
                {
                    PersonaCustomer.Pierre
                }
            };
            _account.Update(_createAccountCommand);
        }

        [Test]
        public void Should_have_account_with_customer_when_create_new_account()
        {
            Assert.That(_account.GetChanges(), Has.Count.EqualTo(1));
            Assert.That(_account.GetChanges().First().AggregateId, Is.EqualTo(_account.Id));
            var accountCreated = _account.GetChanges().OfType<AccountCreated>().First();
            Assert.That(accountCreated, Is.Not.Null);
            Assert.That(accountCreated.Creator, Is.EqualTo(PersonaUser.John)
                                                  .Using(PersonaUser.UserValueComparer));
            Assert.That(accountCreated.Customers,
                Is.EquivalentTo(_createAccountCommand.Customers).Using(PersonaCustomer.CustomerValueComparer));
        }
        
        [Test]
        public void Should_add_customer_when_execute_attach_customer_command()
        {
            _account.Clear();
            var attachCustomersCommand = new AttachCustomersCommand()
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
            Assert.That(_account.GetChanges(), Has.Count.EqualTo(1));
            Assert.That(_account.GetChanges().First().AggregateId, Is.EqualTo(_account.Id));
            var accountCreated = _account.GetChanges().OfType<CustomerAttached>().First();
            Assert.That(accountCreated, Is.Not.Null);
            Assert.That(accountCreated.Modifier, Is.EqualTo(PersonaUser.Jane)
                                                  .Using(PersonaUser.UserValueComparer));
            Assert.That(accountCreated.Customer, Is.EqualTo(attachCustomersCommand.Customers.First())
                                                   .Using(PersonaCustomer.CustomerValueComparer));
        }

        [Test]
        public void Should_update_customer_when_execute_update_customer_command()
        {
            _account.Clear();
            var pierre = PersonaCustomer.Pierre.Clone() as Customer;
            pierre.Birthday = pierre.Birthday.AddDays(2);
            var attachCustomersCommand = new UpdateCustomersCommand()
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
            Assert.That(_account.GetChanges(), Has.Count.EqualTo(1));
            Assert.That(_account.GetChanges().First().AggregateId, Is.EqualTo(_account.Id));
            var accountCreated = _account.GetChanges().OfType<CustomerUpdated>().First();
            Assert.That(accountCreated, Is.Not.Null);
            Assert.That(accountCreated.Modifier, Is.EqualTo(PersonaUser.Jane)
                                                  .Using(PersonaUser.UserValueComparer));
            Assert.That(accountCreated.Customer, Is.EqualTo(attachCustomersCommand.Customers.First())
                                                   .Using(PersonaCustomer.CustomerValueComparer));
        }
    }
}