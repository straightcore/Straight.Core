using NUnit.Framework;
using Straight.Core.Domain;
using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore;
using Straight.Core.Sample.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.Sample.RealEstateAgency.House.EventStore;
using Straight.Core.Sample.RealEstateAgency.Model;
using Straight.Core.Sample.RealEstateAgency.Test.Common;
using Straight.Core.Sample.RealEstateAgency.Test.Common.Server;
using System;
using System.Linq;
using HouseVisitAdded = Straight.Core.Sample.RealEstateAgency.House.EventStore.Events.VisitAdded;

namespace Straight.Core.Sample.RealEstateAgency.Handler.Tests
{
    [TestFixture]
    public class AccountEventHandlerTests
    {
        private class Context
        {
            public Context()
            {
                ReadRepository = new TestReadModelRepository();
                AggregatorRepository = new TestDomainEventStore();
                Handler = new AccountEventHandler(AggregatorRepository, ReadRepository);
                ReadRepository.Add(PersonaAccount.Pierre as Account.Domain.Account);
                ReadRepository.Add(PersonaAccount.Virginie as Account.Domain.Account);
                ReadRepository.Add(PersonaHouse.ApartmentParis);
                var virginie = new AggregatorAccount();
                virginie.LoadFromHistory((PersonaAccount.Virginie as IReadModel<IDomainEvent>)?.Events);
                AggregatorRepository.Add(virginie);

                var apartmentParis = new AggregatorHouse();
                apartmentParis.LoadFromHistory(PersonaHouse.ApartmentParis.Events);
                AggregatorRepository.Add(apartmentParis);
            }

            public AccountEventHandler Handler { get; }
            public TestReadModelRepository ReadRepository { get; }
            public TestDomainEventStore AggregatorRepository { get; }

        }

        [Test]
        public void Should_does_not_throw_exception_when_receive_event()
        {
            var context = new Context();
            context.Handler.Handle(new HouseVisitAdded
                                (new User(
                                        PersonaUser.Jane.LastName,
                                        PersonaUser.Jane.FirstName,
                                        PersonaUser.Jane.Username),
                                    PersonaAccount.Virginie,
                                    DateTime.UtcNow.Date.AddDays(2).AddHours(12).AddMinutes(30))
                                {
                                    AggregateId = PersonaHouse.ApartmentParis.Id,
                                    Version = 2
                                });
        }

        [Test]
        public void Should_raise_visit_house_at_account_when_add_visit_at_house()
        {
            var context = new Context();
            context.Handler.Handle(new HouseVisitAdded(new User(
                    PersonaUser.Jane.LastName,
                    PersonaUser.Jane.FirstName,
                    PersonaUser.Jane.Username),
                PersonaAccount.Virginie,
                DateTime.UtcNow.Date.AddDays(2).AddHours(12).AddMinutes(30))
            {
                AggregateId = PersonaHouse.ApartmentParis.Id,
                Version = 2
            });
            var virginie = context.AggregatorRepository.GetById<AggregatorAccount>(PersonaAccount.Virginie.Id);
            Assert.That(virginie.GetChanges(), Is.Not.Null.And.Not.Empty);
            Assert.That(virginie.GetChanges().Count(), Is.EqualTo(1));
            Assert.That(virginie.GetChanges().First().GetType(), Is.EqualTo(typeof(VisitAdded)));
        }

        [Test]
        public void Should_throw_exception_when_account_is_not_found()
        {
            var context = new Context();
            Assert.Throws<ArgumentException>(() => context.Handler.Handle(new HouseVisitAdded
            (new User(
                    PersonaUser.Jane.LastName,
                    PersonaUser.Jane.FirstName,
                    PersonaUser.Jane.Username),
                PersonaAccount.Pierre,
                DateTime.UtcNow.Date.AddDays(2).AddHours(12).AddMinutes(30))
            {
                AggregateId = PersonaHouse.ApartmentParis.Id,
                Version = 2
            }));
        }

        [Test]
        public void Should_throw_exception_when_house_is_not_found()
        {
            var context = new Context();
            Assert.Throws<ArgumentException>(() => context.Handler.Handle(new HouseVisitAdded
            (new User(
                    PersonaUser.Jane.LastName,
                    PersonaUser.Jane.FirstName,
                    PersonaUser.Jane.Username),
                PersonaAccount.Pierre,
                DateTime.UtcNow.Date.AddDays(2).AddHours(12).AddMinutes(30))
            {
                AggregateId = PersonaHouse.ApartmentNewYork.Id,
                Version = 2
            }));
        }
    }
}