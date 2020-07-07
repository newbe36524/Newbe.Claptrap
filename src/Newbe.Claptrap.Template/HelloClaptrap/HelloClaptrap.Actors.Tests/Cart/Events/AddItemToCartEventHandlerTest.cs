using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using HelloClaptrap.Actors.Cart.Events;
using HelloClaptrap.Models.Cart;
using HelloClaptrap.Models.Cart.Events;
using NUnit.Framework;

namespace HelloClaptrap.Actors.Tests.Cart.Events
{
    public class AddItemToCartEventHandlerTest
    {
        [Test]
        public async Task AddFirstOne()
        {
            using var mocker = AutoMock.GetStrict();
            await using var handler = mocker.Create<AddItemToCartEventHandler>();
            var state = new CartState();
            var evt = new AddItemToCartEvent
            {
                SkuId = "skuId1",
                Count = 10
            };
            await handler.HandleEvent(state, evt, default);

            state.Items.Count.Should().Be(1);
            state.Items.Keys.Contains(evt.SkuId).Should().BeTrue();
            state.Items.Values.Contains(evt.Count).Should().BeTrue();
        }
    }
}