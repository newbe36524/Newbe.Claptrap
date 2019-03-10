using System.IO.Pipes;

namespace Newbe.Claptrap.Autofac
{
    public static class Constants
    {
        public const string ActorLifetimeScope = nameof(ActorLifetimeScope);
        public const string EventLifetimeScope = nameof(EventLifetimeScope);

        public static class MinionEventHandlerMetadataKeys
        {
            public const string MinionKind = nameof(MinionKind);
            public const string EventType = nameof(EventType);
        }
    }
}