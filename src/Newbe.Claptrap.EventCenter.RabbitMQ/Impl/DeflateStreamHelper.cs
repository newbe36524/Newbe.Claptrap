using System.IO;
using System.IO.Compression;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Impl
{
    public class DeflateStreamHelper
    {
        public const string ContentEncoding = "deflate";

        public static byte[] Compress(Stream input)
        {
            using var compressStream = new MemoryStream();
            using var compressor = new DeflateStream(compressStream, CompressionMode.Compress);
            input.CopyTo(compressor);
            compressor.Close();
            return compressStream.ToArray();
        }

        public static byte[] Decompress(byte[] input)
        {
            var output = new MemoryStream();

            using (var compressStream = new MemoryStream(input))
            using (var decompressor = new DeflateStream(compressStream, CompressionMode.Decompress))
            {
                decompressor.CopyTo(output);
            }

            output.Position = 0;
            return output.ToArray();
        }
    }
}