using System;
using System.Diagnostics.CodeAnalysis;

namespace Newbe.Claptrap
{
    [ExcludeFromCodeCoverage]
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UnitState : IState
    {
        public IClaptrapIdentity Identity { get; } = null!;
        public IStateData Data { get; } = null!;
        public long Version { get; private set; }

        public void IncreaseVersion()
        {
            Version++;
        }

        public class UnitStateData : IStateData
        {
            public string Item1 { get; set; } = null!;
            public string Item2 { get; set; } = null!;
            public string Item3 { get; set; } = null!;
            public string Item4 { get; set; } = null!;
            public string Item5 { get; set; } = null!;

            public static UnitStateData Create()
            {
                return new UnitStateData
                {
                    Item1 = Guid.NewGuid().ToString("N"),
                    Item2 = Guid.NewGuid().ToString("N"),
                    Item3 = Guid.NewGuid().ToString("N"),
                    Item4 = Guid.NewGuid().ToString("N"),
                    Item5 = Guid.NewGuid().ToString("N"),
                };
            }
        }
    }
}