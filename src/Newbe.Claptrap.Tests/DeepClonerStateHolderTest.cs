using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class DeepClonerStateHolderTest
    {
        [Test]
        public void TestClass()
        {
            var testData = new TestData
            {
                Name = "name"
            };
            var deepClonerStateHolder = new DeepClonerStateHolder(TestClaptrapIdentity.Instance);
            var re = deepClonerStateHolder.DeepCopy(testData);
            re.Should().NotBe(testData);
            re.Should().BeOfType<TestData>();
            ((TestData) re).Name.Should().Be(testData.Name);
        }

        [Test]
        public void TestList()
        {
            const int listCount = 10;
            var testData = new TestData
            {
                List = Enumerable.Range(0, listCount).ToList()
            };
            var deepClonerStateHolder = new DeepClonerStateHolder(TestClaptrapIdentity.Instance);
            var re = deepClonerStateHolder.DeepCopy(testData);
            re.Should().NotBe(testData);
            re.Should().BeOfType<TestData>();
            ((TestData) re).List.Clear();
            testData.List.Count.Should().Be(listCount);
        }

        public class TestData : IState
        {
            public string Name { get; set; }
            public List<int> List { get; set; }
            public IClaptrapIdentity Identity { get; }
            public IStateData Data { get; }
            public long Version { get; }

            public void IncreaseVersion()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}