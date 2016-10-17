
namespace Straight.Core.Sample.RealEstateAgency.Model
{
    public interface IHouse : IIdentifiable
    {
        User Creator { get; }
        Address Address { get; }
        User LastModifier { get; }
    }
}