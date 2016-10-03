using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Straight.Core.RealEstateAgency.Model;

namespace Straight.Core.RealEstateAgency.Test.Common
{

    public static class PersonaCustomer
    {
        public static IEqualityComparer<Customer> CustomerValueComparer { get; } = new CustomerEqualityComparer();

        public static Customer Pierre { get; } = new Customer()
        {
            FirstName = "Pierre",
            LastName = "Durand",
            PostalCode = "75009",
            Street = "rue des Martyres",
            StreetNumber = "123 bis",
            City = "Paris",
            CellPhone = new Phone() { Number = "0701020304", CountryCode = "33" },
            Birthday = new DateTime(1979, 08, 12),
            Gender = Gender.Mr,
            Email = "pierre.durand@fake.com",
        };

        public static Customer Virginie { get; } = new Customer()
        {
            FirstName = "Virginie",
            LastName = "Eclin",
            PostalCode = "06200",
            Street = "rue des Antibes",
            StreetNumber = "23",
            City = "Saint Laurent du Var",
            CellPhone = new Phone() { Number = "0612345678", CountryCode = "33" },
            Birthday = new DateTime(1983, 03, 07),
            Gender = Gender.Miss,
            Email = "e.nini@fake.com",
        };
    }


    internal class CustomerEqualityComparer : IEqualityComparer<Customer>
    {
        private static List<PropertyInfo> properties = new List<PropertyInfo>();

        static CustomerEqualityComparer()
        {
            var type = typeof(CustomerEqualityComparer);
            properties = type.GetProperties(BindingFlags.Public).ToList();
        }

        public bool Equals(Customer left, Customer right)
        {
            return
                properties.All(
                    info => info.GetGetMethod().Invoke(left, null) == info.GetGetMethod().Invoke(right, null));
        }

        public int GetHashCode(Customer obj)
        {
            return -1;
        }
    }

}