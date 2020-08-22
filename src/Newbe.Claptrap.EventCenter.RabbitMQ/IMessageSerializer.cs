using System;

namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public interface IMessageSerializer
    {
        ReadOnlyMemory<byte> Serialize(string source);
        string Deserialize(ReadOnlyMemory<byte> bytes);
    }
}