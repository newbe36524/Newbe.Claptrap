using System;

namespace Newbe.Claptrap.Autofac
{
    public interface IEventHandlerRegister
    {
        void RegisterHandler(string actorTypeCode, string eventType, Type handlerType);
        Type? FindHandlerType(string actorTypeCode, string eventType);
    }
}