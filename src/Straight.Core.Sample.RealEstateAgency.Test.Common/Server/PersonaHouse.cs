using Straight.Core.EventStore;
using Straight.Core.Sample.RealEstateAgency.House.EventStore.Events;
using System.Collections.Generic;
using ReadOnlyHouse = Straight.Core.Sample.RealEstateAgency.House.Domain.House;

namespace Straight.Core.Sample.RealEstateAgency.Test.Common.Server
{
    public static class PersonaHouse
    {
        private static ReadOnlyHouse _apartmentParis;
        private static ReadOnlyHouse _apartmentNewYork;

        public static ReadOnlyHouse ApartmentParis
        {
            get
            {
                if (_apartmentParis != null)
                    return _apartmentParis;
                _apartmentParis = new ReadOnlyHouse();
                _apartmentParis.LoadFromHistory(new List<IDomainEvent>
                {
                    new HouseCreated(PersonaUser.John, PersonaAddress.RueJoubertParis)
                });
                return _apartmentParis;
            }
        }

        public static ReadOnlyHouse ApartmentNewYork
        {
            get
            {
                if (_apartmentNewYork != null)
                    return _apartmentNewYork;
                _apartmentNewYork = new ReadOnlyHouse();
                _apartmentNewYork.LoadFromHistory(new List<IDomainEvent>
                {
                    new HouseCreated(PersonaUser.John, PersonaAddress.LexingtonAvenue)
                });
                return _apartmentNewYork;
            }
        }
    }
}