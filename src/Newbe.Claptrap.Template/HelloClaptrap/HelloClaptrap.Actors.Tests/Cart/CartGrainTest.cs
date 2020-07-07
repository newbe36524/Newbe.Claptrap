using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using HelloClaptrap.Actors.Cart;
using HelloClaptrap.Models.Cart;
using Newbe.Claptrap;
using Newbe.Claptrap.Orleans;
using NUnit.Framework;

namespace HelloClaptrap.Actors.Tests.Cart
{
    public class CartGrainTest
    {
        [Test]
        public async Task GetItemsAsync()
        {
            using var mocker = AutoMock.GetStrict();

            mocker.Mock<IClaptrapGrainCommonService>()
                .Setup(x => x.ClaptrapAccessor.Claptrap.State.Data)
                .Returns(new CartState());

            var handler = mocker.Create<CartGrain>();
            var items = await handler.GetItemsAsync();
            items.Should().BeEmpty();
        }
    }
}