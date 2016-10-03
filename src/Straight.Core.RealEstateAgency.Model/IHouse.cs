namespace Straight.Core.RealEstateAgency.Model
{
    public interface IHouse
    {
        User Creator { get; }
        Address Address { get; }
        User LastModifier { get; }
    }
}