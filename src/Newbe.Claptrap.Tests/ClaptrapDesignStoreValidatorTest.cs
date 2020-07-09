using System;
using FluentAssertions;
using Newbe.Claptrap.Design;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapDesignStoreValidatorTest
    {
        [Test]
        public void AllNull()
        {
            using var mocker = AutoMockHelper.Create();
            var validator = mocker.Create<ClaptrapDesignStoreValidator>();
            var (isOk, errorMessage) = validator.Validate(new[] {new ClaptrapDesign()});
            isOk.Should().Be(false);
            errorMessage.Should().NotBeNullOrEmpty();
            Console.WriteLine(errorMessage);
        }
    }
}