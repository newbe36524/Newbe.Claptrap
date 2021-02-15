namespace Newbe.Claptrap
{
    public class ClaptrapServerOptions
    {
        public const string ConfigurationSectionName = "Claptrap";
        public string DefaultConnectionString { get; set; } = null!;

        public ClaptrapAppMetricsInfluxDbOptions MetricsInfluxDb { get; set; } = new();
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public StorageOptions EventStore { get; set; } = new();
        public StorageOptions StateStore { get; set; } = new();
        public RabbitMQOptions RabbitMQ { get; set; } = new();
        public DaprOptions Dapr { get; set; } = new();
    }
}