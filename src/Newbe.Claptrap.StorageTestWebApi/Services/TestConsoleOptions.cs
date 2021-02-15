namespace Newbe.Claptrap.StorageTestWebApi.Services
{
    public class TestConsoleOptions
    {
        public DatabaseType DatabaseType { get; set; }
        public bool SetupLocalDatabase { get; set; }
        public int ActorCount { get; set; }
    }
}