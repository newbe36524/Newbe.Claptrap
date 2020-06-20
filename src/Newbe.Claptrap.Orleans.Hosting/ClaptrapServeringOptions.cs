namespace Newbe.Claptrap.Orleans
{
    public class ClaptrapServeringOptions
    {
        public const string ConfigurationSectionName = "Claptrap";
        public string DefaultConnectionString { get; set; } = null!;

        public ClaptrapOrleansOptions Orleans { get; set; }
            = new ClaptrapOrleansOptions();

        public ClaptrapAppMetricsInfluxDbOptions MetricsInfluxDb { get; set; } =
            new ClaptrapAppMetricsInfluxDbOptions();
    }
}