namespace Newbe.Claptrap.StorageTestConsole
{
    public class TestConsoleOptions
    {
        public DatabaseType DatabaseType { get; set; }
        public bool SetupLocalDatabase { get; set; }
        public int ActorCount { get; set; }
        public int TotalCount { get; set; } = 5_000_000;
        public int BatchSize { get; set; } = 10_000;
        public TestJobType Job { get; set; }
    }
}