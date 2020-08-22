using System;
using System.Diagnostics;
using System.Text;
using FluentAssertions;
using Newbe.Claptrap.EventCenter.RabbitMQ.Impl;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class CompressTest
    {
        [Theory]
        [TestCase(typeof(GzipStreamHelper))]
        [TestCase(typeof(DeflateStreamHelper))]
        public void Test(Type helperType)
        {
            var helper = (IStreamCompressHelper) Activator.CreateInstance(helperType);
            Debug.Assert(helper != null, nameof(helper) + " != null");
            const string source = "test666test666test666";
            var bytes = Encoding.UTF8.GetBytes(source);
            var sourceMemory = new ReadOnlyMemory<byte>(bytes);
            var compressed = helper.Compress(sourceMemory);
            Console.WriteLine($"source : {source} sourceBytes : {bytes.Length} compressed : {compressed.Length}");
            var decompressed = helper.Decompress(compressed);
            var decompressedString = Encoding.UTF8.GetString(decompressed.Span);
            source.Should().Be(decompressedString);
        }
    }
}