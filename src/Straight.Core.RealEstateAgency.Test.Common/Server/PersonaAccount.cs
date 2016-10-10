using System.Collections.Generic;
using Straight.Core.EventStore;
using Straight.Core.RealEstateAgency.Account.EventStore.Events;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.RealEstateAgency.Test.Common.Server
{
    public static class PersonaAccount
    {
        private static IAccount _virginie;
        private static IAccount _pierre;

        public static IAccount Virginie => _virginie ?? (_virginie = BuildVirginie());

        private static IAccount BuildVirginie()
        {
            var virginie = new Account.Domain.Account();
            virginie.LoadFromHistory(new List<IDomainEvent>()
            {
                new AccountCreated("chevalier.virginie", PersonaUser.John, new [] {PersonaCustomer.Virginie}),
            });
            return virginie;
        }

        public static IAccount Pierre => _pierre ?? (_pierre = BuildPierre());


        private static IAccount BuildPierre()
        {
            var pierre = new Account.Domain.Account();
            pierre.LoadFromHistory(new List<IDomainEvent>()
            {
                new AccountCreated("pierre.durand", PersonaUser.Jane, new [] {PersonaCustomer.Pierre}),
            });
            return pierre;
        }
    }
}