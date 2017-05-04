using Xunit;
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
    public class AccountEventHandlerTests
    {
        public AccountEventHandlerTests()
        {
            _readRepository = new TestReadModelRepository();
            _aggregatorRepository = new TestDomainEventStore();
            _handler = new AccountEventHandler(_aggregatorRepository, _readRepository);
            _readRepository.Add(PersonaAccount.Pierre as Account.Domain.Account);
            _readRepository.Add(PersonaAccount.Virginie as Account.Domain.Account);
            _readRepository.Add(PersonaHouse.ApartmentParis);
            var virginie = new AggregatorAccount();
            virginie.LoadFromHistory((PersonaAccount.Virginie as IReadModel<IDomainEvent>)?.Events);
            _aggregatorRepository.Add(virginie);

            var apartmentParis = new AggregatorHouse();
            apartmentParis.LoadFromHistory(PersonaHouse.ApartmentParis.Events);
            _aggregatorRepository.Add(apartmentParis);
        }

        private readonly AccountEventHandler _handler;
        private readonly TestReadModelRepository _readRepository;
        private readonly TestDomainEventStore _aggregatorRepository;

        [Fact]
        public void Should_does_not_throw_exception_when_receive_event()
        {
            _handler.Handle(new HouseVisitAdded
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

        [Fact]
        public void Should_raise_visit_house_at_account_when_add_visit_at_house()
        {
            _handler.Handle(new HouseVisitAdded(new User(
                    PersonaUser.Jane.LastName,
                    PersonaUser.Jane.FirstName,
                    PersonaUser.Jane.Username),
                PersonaAccount.Virginie,
                DateTime.UtcNow.Date.AddDays(2).AddHours(12).AddMinutes(30))
            {
                AggregateId = PersonaHouse.ApartmentParis.Id,
                Version = 2
            });
            var virginie = _aggregatorRepository.GetById<AggregatorAccount>(PersonaAccount.Virginie.Id);
            Assert.NotNull(virginie.GetChanges());
            Assert.NotEmpty(virginie.GetChanges());
            Assert.Equal(virginie.GetChanges().Count(), 1);
            Assert.Equal(virginie.GetChanges().First().GetType(), typeof(VisitAdded));
        }

        [Fact]
        public void Should_throw_exception_when_account_is_not_found()
        {
            Assert.Throws<ArgumentException>(() => _handler.Handle(new HouseVisitAdded
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

        [Fact]
        public void Should_throw_exception_when_house_is_not_found()
        {
            Assert.Throws<ArgumentException>(() => _handler.Handle(new HouseVisitAdded
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