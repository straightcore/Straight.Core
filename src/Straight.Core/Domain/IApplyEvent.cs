namespace Straight.Core.Domain
{
    public interface IApplyEvent<in TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        void Apply(TDomainEvent theEvent);
    }
}