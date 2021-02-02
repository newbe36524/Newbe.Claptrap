using System.Diagnostics;

namespace Newbe.Claptrap
{
    public static class ClaptrapActivitySource
    {
        public static readonly ActivitySource Instance = new("claptrap");

        public static class Tags
        {
            public const string IsMinion = "claptrap.is_minion";
            public const string TypeCode = "claptrap.type_code";
            public const string Id = "claptrap.id";
        }

        public static class ActivityNames
        {
            public const string Activate = "Activate";
            public const string Deactivate = "Deactivate";
            public const string HandleEvent = "HandleEvent";
            public const string SaveEvent = "SaveEvent";
            public const string LoadEvent = "LoadEvent";
            public const string LoadState = "LoadState";
            public const string SaveState = "SaveState";
        }
    }
}