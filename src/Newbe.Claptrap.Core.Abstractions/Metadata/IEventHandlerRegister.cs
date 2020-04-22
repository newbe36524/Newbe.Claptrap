using System;

namespace Newbe.Claptrap.Metadata
{
    public interface IEventHandlerRegister
    {
        void RegisterHandler(string actorTypeCode, string eventTypeCode, Type handlerType);
        Type? FindHandlerType(string actorTypeCode, string eventTypeCode);
    }
}