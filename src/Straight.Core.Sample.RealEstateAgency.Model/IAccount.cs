using System.Collections.Generic;

namespace Straight.Core.Sample.RealEstateAgency.Model
{
    public interface IAccount : IIdentifiable
    {
        User Creator { get; }
        IEnumerable<Customer> Customers { get; }
        User LastModifier { get; }
    }
}