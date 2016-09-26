using System;

namespace Straight.Core.Bus
{
    public interface IRegisterRouteMessage
    {
        void Register<TMessage>(Action<TMessage> route) where TMessage : class;

        void Register<TMessage>(Type typeOfMessage, Action<TMessage> route) where TMessage : class;
    }
}