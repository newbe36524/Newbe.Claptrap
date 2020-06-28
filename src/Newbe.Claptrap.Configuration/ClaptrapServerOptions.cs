namespace Newbe.Claptrap
{
    public class ClaptrapServerOptions
    {
        public const string ConfigurationSectionName = "Claptrap";
        public string DefaultConnectionString { get; set; } = null!;

        public ClaptrapOrleansOptions Orleans { get; set; }
            = new ClaptrapOrleansOptions();

        public ClaptrapAppMetricsInfluxDbOptions MetricsInfluxDb { get; set; } =
            new ClaptrapAppMetricsInfluxDbOptions();

        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public StorageOptions EventStore { get; set; } = new StorageOptions();
        public StorageOptions StateStore { get; set; } = new StorageOptions();
        public StorageOptions Storage { get; set; } = new StorageOptions();
    }
}