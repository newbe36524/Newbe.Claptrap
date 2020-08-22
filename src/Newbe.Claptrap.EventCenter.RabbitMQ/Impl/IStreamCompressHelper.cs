using System;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public interface IStreamCompressHelper
    {
        ReadOnlyMemory<byte> Compress(ReadOnlyMemory<byte> input);
        ReadOnlyMemory<byte> Decompress(ReadOnlyMemory<byte> input);
    }
}