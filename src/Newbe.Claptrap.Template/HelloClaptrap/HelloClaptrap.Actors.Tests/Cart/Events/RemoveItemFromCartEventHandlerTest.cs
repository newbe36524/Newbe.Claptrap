using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using HelloClaptrap.Actors.Cart.Events;
using HelloClaptrap.Models.Cart;
using HelloClaptrap.Models.Cart.Events;
using Newbe.Claptrap;
using NUnit.Framework;

namespace HelloClaptrap.Actors.Tests.Cart.Events
{
    public class RemoveItemFromCartEventHandlerTest
    {
        [Test]
        public async Task RemoveOne()
        {
            using var mocker = AutoMock.GetStrict();

            await using var handler = mocker.Create<RemoveItemFromCartEventHandler>();
            const string oldKey = "oneKey";
            var state = new CartState
            {
                Items = new Dictionary<string, int>
                {
                    {oldKey, 100}
                }
            };
            var evt = new RemoveItemFromCartEvent
            {
                SkuId = oldKey,
                Count = 10
            };
            await handler.HandleEvent(state, evt, default);

            state.Items.Count.Should().Be(1);
            var (key, value) = state.Items.Single();
            key.Should().Be(oldKey);
            value.Should().Be(90);
        }

        [Test]
        public async Task RemoveAll()
        {
            using var mocker = AutoMock.GetStrict();

            await using var handler = mocker.Create<RemoveItemFromCartEventHandler>();
            const string oldKey = "oneKey";
            var state = new CartState
            {
                Items = new Dictionary<string, int>
                {
                    {oldKey, 100}
                }
            };
            var evt = new RemoveItemFromCartEvent
            {
                SkuId = oldKey,
                Count = 100
            };
            await handler.HandleEvent(state, evt, default);

            state.Items.Should().BeEmpty();
        }

        [Test]
        public async Task RemoveKeyNotFoundAtFirstTime()
        {
            using var mocker = AutoMock.GetStrict();

            await using var handler = mocker.Create<RemoveItemFromCartEventHandler>();
            var state = new CartState();
            var evt = new RemoveItemFromCartEvent
            {
                SkuId = "skuId1",
                Count = 10
            };
            await handler.HandleEvent(state, evt, default);

            state.Items.Should().BeNull();
        }

        [Test]
        public async Task RemoveKeyNotFound()
        {
            using var mocker = AutoMock.GetStrict();

            await using var handler = mocker.Create<RemoveItemFromCartEventHandler>();
            const string oldKey = "oneKey";
            var state = new CartState
            {
                Items = new Dictionary<string, int>
                {
                    {oldKey, 100}
                }
            };
            var evt = new RemoveItemFromCartEvent
            {
                SkuId = "skuId1",
                Count = 10
            };
            await handler.HandleEvent(state, evt, default);

            state.Items.Count.Should().Be(1);
            var (key, _) = state.Items.Single();
            key.Should().Be(oldKey);
        }
    }
}