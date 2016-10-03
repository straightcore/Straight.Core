using System.Collections.Generic;

namespace Straight.Core.RealEstateAgency.Model
{
    public interface IAccount
    {
        User Creator { get; }
        IEnumerable<Customer> Customers { get; }
        User LastModifier { get; }
    }
}