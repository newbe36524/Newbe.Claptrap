namespace Newbe.Claptrap.Orleans
{
    public class ClaptrapOrleansOptions
    {
        public string Hostname { get; set; } = null!;
        public int? GatewayPort { get; set; }
        public int? SiloPort { get; set; }
    }
}