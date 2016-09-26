namespace Straight.Core.Storage
{
    public interface IRouterMessage
    {
        void Route(object message);
    }
}