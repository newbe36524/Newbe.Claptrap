namespace Newbe.Claptrap
{
    public class ClaptrapServerOptions
    {
        public const string ConfigurationSectionName = "Claptrap";
        public string DefaultConnectionString { get; set; } = null!;

        public ClaptrapAppMetricsInfluxDbOptions MetricsInfluxDb { get; set; } =
            new ClaptrapAppMetricsInfluxDbOptions();

        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public StorageOptions EventStore { get; set; } = new StorageOptions();
        public StorageOptions StateStore { get; set; } = new StorageOptions();
        public RabbitMQOptions RabbitMQ { get; set; } = new RabbitMQOptions();
    }
}