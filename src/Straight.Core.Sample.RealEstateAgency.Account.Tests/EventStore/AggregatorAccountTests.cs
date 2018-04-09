using NUnit.Framework;
using Straight.Core.Sample.RealEstateAgency.Account.Domain.Command;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using System.Linq;
using Straight.Core.Sample.RealEstateAgency.Model;

namespace Straight.Core.Sample.RealEstateAgency.Account.Tests.EventStore
{
    [TestFixture]
    public class AggregatorAccountTests
    {

        private class AggregContext
        {
            public AggregContext()
            {
                Account = new AggregatorAccount();

                CreateAccountCommand = new CreateEmployeAccountCommand
                {
                    CreatorFirstName = PersonaUser.John.FirstName,
                    CreatorLastName = PersonaUser.John.LastName,
                    CreatorUsername = PersonaUser.John.Username,
                    Customers = new[]
                    {
                        PersonaCustomer.Pierre
                    }
                };
                Account.Update(CreateAccountCommand);
            }

            public AggregatorAccount Account { get; }
            public CreateEmployeAccountCommand CreateAccountCommand { get; }
        }

        [Test]
        public void Should_add_customer_when_execute_attach_customer_command()
        {
            var context = new AggregContext();
            context.Account.Clear();
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
            context.Account.Update(attachCustomersCommand);
            Assert.That(context.Account.GetChanges().Count(), Is.EqualTo(1));
            Assert.That(context.Account.GetChanges().First().AggregateId, Is.EqualTo(context.Account.Id));
            var accountCreated = context.Account.GetChanges().OfType<CustomerAttached>().First();
            Assert.NotNull(accountCreated);
            Assert.That(accountCreated.Modifier, Is.EqualTo(PersonaUser.Jane).Using(PersonaUser.UserValueComparer));
            Assert.That(accountCreated.Customer, Is.EqualTo(attachCustomersCommand.Customers.First()).Using(PersonaCustomer.CustomerValueComparer));
        }

        [Test]
        public void Should_have_account_with_customer_when_create_new_account()
        {
            var context = new AggregContext();
            Assert.That(context.Account.GetChanges().Count(), Is.EqualTo(1));
            Assert.That(context.Account.GetChanges().First().AggregateId, Is.EqualTo(context.Account.Id));
            var accountCreated = context.Account.GetChanges().OfType<EmployeAccountCreated>().First();
            Assert.NotNull(accountCreated);
            Assert.That(accountCreated.Creator, Is.EqualTo(PersonaUser.John).Using(PersonaUser.UserValueComparer));
            Assert.That(accountCreated.Customers, Is.EqualTo(context.CreateAccountCommand.Customers).Using(PersonaCustomer.CustomerValueComparer));
        }

        [Test]
        public void Should_update_customer_when_execute_update_customer_command()
        {
            var context = new AggregContext();
            context.Account.Clear();
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
            context.Account.Update(attachCustomersCommand);
            Assert.That(context.Account.GetChanges().Count(), Is.EqualTo(1));
            Assert.That(context.Account.GetChanges().First().AggregateId, Is.EqualTo(context.Account.Id));
            var accountCreated = context.Account.GetChanges().OfType<CustomerUpdated>().First();
            Assert.NotNull(accountCreated);
            Assert.That(accountCreated.Modifier, Is.EqualTo(PersonaUser.Jane).Using(PersonaUser.UserValueComparer));
            Assert.That(accountCreated.Customer, Is.EqualTo(attachCustomersCommand.Customers.First()).Using(PersonaCustomer.CustomerValueComparer));
        }
    }
}