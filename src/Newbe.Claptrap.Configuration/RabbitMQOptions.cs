using System;

namespace Newbe.Claptrap
{
    public class RabbitMQOptions
    {
        public bool? Enabled { get; set; }
        public CompressType? CompressType { get; set; }
        public Uri? Uri { get; set; }
    }
}