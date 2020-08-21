using System.Linq;
using FluentAssertions;
using Newbe.Claptrap.Design;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class ClaptrapDesignStoreTest
    {
        [Theory]
        [TestCase("testCode")]
        public void Add(string typeCode)
        {
            using var mocker = AutoMockHelper.Create();
            var claptrapDesignStore = mocker.Create<ClaptrapDesignStore>();
            var actorIdentity = new TestClaptrapIdentity("123", typeCode);
            var design = new ClaptrapDesign
            {
                ClaptrapTypeCode = typeCode
            };
            claptrapDesignStore.AddOrReplace(design);
            var claptrapDesign = claptrapDesignStore.FindDesign(actorIdentity);
            claptrapDesign.Should().Be(design);
        }

        [Theory]
        [TestCase("testCode")]
        public void AddFactory(string typeCode)
        {
            using var mocker = AutoMockHelper.Create();
            var claptrapDesignStore = mocker.Create<ClaptrapDesignStore>();
            var actorIdentity = new TestClaptrapIdentity("123", typeCode);
            var design = new ClaptrapDesign
            {
                ClaptrapTypeCode = typeCode
            };
            claptrapDesignStore.AddOrReplace(design);
            var factoryInvoked = false;
            claptrapDesignStore.AddOrReplaceFactory(typeCode, (identity, sourceDesign) =>
            {
                factoryInvoked = true;
                return sourceDesign;
            });
            var claptrapDesign = claptrapDesignStore.FindDesign(actorIdentity);
            claptrapDesign.Should().Be(design);
            factoryInvoked.Should().BeTrue();
        }

        [Theory]
        [TestCase("123", "testCode")]
        [TestCase(null, "testCode")]
        public void Replace(string id, string typeCode)
        {
            using var mocker = AutoMockHelper.Create();
            var claptrapDesignStore = mocker.Create<ClaptrapDesignStore>();
            var actorIdentity = new TestClaptrapIdentity(id, typeCode);
            var design = new ClaptrapDesign
            {
                ClaptrapTypeCode = typeCode
            };
            claptrapDesignStore.AddOrReplace(design);
            var newDesign = new ClaptrapDesign
            {
                ClaptrapTypeCode = typeCode
            };
            claptrapDesignStore.AddOrReplace(newDesign);
            var claptrapDesign = claptrapDesignStore.FindDesign(actorIdentity);
            claptrapDesign.Should().Be(newDesign);
        }

        [Test]
        public void NotFound()
        {
            using var mocker = AutoMockHelper.Create();
            var claptrapDesignStore = mocker.Create<ClaptrapDesignStore>();
            var actorIdentity = new TestClaptrapIdentity("123", "testCode");
            Assert.Throws<ClaptrapDesignNotFoundException>(() =>
                claptrapDesignStore.FindDesign(actorIdentity));
        }

        [Test]
        public void DesignFound()
        {
            using var mocker = AutoMockHelper.Create();
            var claptrapDesignStore = mocker.Create<ClaptrapDesignStore>();
            const string typeCode = "testCode";
            var design = new ClaptrapDesign
            {
                ClaptrapTypeCode = typeCode
            };
            claptrapDesignStore.AddOrReplace(design);
            var claptrapDesign = claptrapDesignStore.FindDesign(new TestClaptrapIdentity("456", typeCode));
            claptrapDesign.Should().Be(design);
        }

        [Test]
        public void Remove()
        {
            using var mocker = AutoMockHelper.Create();
            var claptrapDesignStore = mocker.Create<ClaptrapDesignStore>();
            const string typeCode = "testCode";
            var design = new ClaptrapDesign
            {
                ClaptrapTypeCode = typeCode
            };
            claptrapDesignStore.AddOrReplace(design);
            claptrapDesignStore.Remove(x => x.ClaptrapTypeCode == typeCode);
            claptrapDesignStore.ToArray().Should().BeEmpty();
        }

        [Test]
        public void RemoveFactory()
        {
            const string typeCode = "testCode";
            using var mocker = AutoMockHelper.Create();
            var store = mocker.Create<ClaptrapDesignStore>();
            var actorIdentity = new TestClaptrapIdentity("123", typeCode);
            var design = new ClaptrapDesign
            {
                ClaptrapTypeCode = typeCode
            };
            store.AddOrReplace(design);
            store.AddOrReplaceFactory(typeCode,
                (identity, sourceDesign) => new ClaptrapDesign
                {
                    StateDataType = typeof(int)
                });
            var claptrapDesign = store.FindDesign(actorIdentity);
            claptrapDesign.StateDataType.Should().Be(typeof(int));
            store.RemoveFactory(typeCode);
            claptrapDesign = store.FindDesign(actorIdentity);
            claptrapDesign.Should().Be(design);
        }
    }
}