using System.Collections.Generic;
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
            var (key, value) = state.Items.Single();
            key.Should().Be(evt.SkuId);
            value.Should().Be(evt.Count);
        }

        [Test]
        public async Task AddOneKeyFound()
        {
            using var mocker = AutoMock.GetStrict();

            await using var handler = mocker.Create<AddItemToCartEventHandler>();
            const string skuId = "skuId1";
            var state = new CartState
            {
                Items = new Dictionary<string, int>
                {
                    {skuId, 60}
                }
            };
            var evt = new AddItemToCartEvent
            {
                SkuId = skuId,
                Count = 10
            };
            await handler.HandleEvent(state, evt, default);

            var (key, value) = state.Items.Single();
            key.Should().Be(skuId);
            value.Should().Be(70);
        }
    }
}