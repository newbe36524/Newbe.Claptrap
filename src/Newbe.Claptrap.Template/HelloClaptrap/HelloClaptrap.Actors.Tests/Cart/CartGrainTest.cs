using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using HelloClaptrap.Actors.Cart;
using HelloClaptrap.Models.Cart;
using NUnit.Framework;

namespace HelloClaptrap.Actors.Tests.Cart
{
    public class CartGrainTest
    {
        [Test]
        public async Task GetItemsAsync()
        {
            using var mocker = AutoMock.GetStrict();

            mocker.Mock<CartGrain>()
                .Setup(x => x.StateData)
                .Returns(new CartState());

            var handler = mocker.Create<CartGrain>();
            var items = await handler.GetItemsAsync();
            items.Should().BeEmpty();
        }
    }
}