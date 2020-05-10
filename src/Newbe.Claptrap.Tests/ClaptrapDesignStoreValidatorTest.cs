using FluentAssertions;
using Newbe.Claptrap.Design;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapDesignStoreValidatorTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ClaptrapDesignStoreValidatorTest(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void AllNull()
        {
            using var mocker = AutoMockHelper.Create(_testOutputHelper);
            var validator = mocker.Create<ClaptrapDesignStoreValidator>();
            var (isOk, errorMessage) = validator.Validate(new[] {new ClaptrapDesign()});
            isOk.Should().Be(false);
            errorMessage.Should().NotBeNullOrEmpty();
            _testOutputHelper.WriteLine(errorMessage);
        }
    }
}