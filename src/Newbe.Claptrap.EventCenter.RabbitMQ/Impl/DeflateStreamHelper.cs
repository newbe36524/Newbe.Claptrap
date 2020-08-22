using System;
using System.IO;
using System.IO.Compression;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class DeflateStreamHelper : IStreamCompressHelper
    {
        public const string ContentEncoding = "deflate";

        public static ReadOnlyMemory<byte> Compress(ReadOnlyMemory<byte> input)
        {
            using var compressStream = new MemoryStream();
            using var compressor = new DeflateStream(compressStream, CompressionMode.Compress);
            compressor.Write(input.Span);
            compressor.Close();
            return compressStream.ToArray();
        }

        public static ReadOnlyMemory<byte> Decompress(ReadOnlyMemory<byte> input)
        {
            using var sourceStream = new MemoryStream();
            using var decompressStream = new MemoryStream();
            using var decompressor = new DeflateStream(sourceStream, CompressionMode.Decompress);
            sourceStream.Write(input.Span);
            sourceStream.Seek(0, SeekOrigin.Begin);
            decompressor.CopyTo(decompressStream);
            return decompressStream.ToArray();
        }

        ReadOnlyMemory<byte> IStreamCompressHelper.Decompress(ReadOnlyMemory<byte> input)
        {
            return Decompress(input);
        }

        ReadOnlyMemory<byte> IStreamCompressHelper.Compress(ReadOnlyMemory<byte> input)
        {
            return Compress(input);
        }
    }
}