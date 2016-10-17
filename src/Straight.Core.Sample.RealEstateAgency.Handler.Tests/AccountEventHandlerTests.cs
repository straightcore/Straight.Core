using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Straight.Core.Domain.Storage;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Storage;
using Straight.Core.Sample.RealEstateAgency.Account.Domain;
using Straight.Core.Sample.RealEstateAgency.Handler;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.Model;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;

namespace Straight.Core.RealEstateAgency.Handler.Tests
{
    [TestFixture]
    public class AccountEventHandlerTests
    {
        private AccountEventHandler _handler;

        [SetUp]
        public void Setup()
        {
            var readRepository = Substitute.For<IReadModelRepository<IDomainEvent>>();
            var aggregatorRepository = Substitute.For<IDomainEventStore<IDomainEvent>>();
            _handler = new AccountEventHandler(aggregatorRepository, readRepository);
        }


        [Test]
        public void Should_handle_event_when_receive_event()
        {
            _handler.Handle(new VisitAdded(new User(
                PersonaUser.Jane.LastName,
                PersonaUser.Jane.FirstName,
                PersonaUser.Jane.Username),
                new Account(), 
                DateTime.UtcNow.Date.AddDays(2).AddHours(12).AddMinutes(30) 
                ));
        }
    }
}
