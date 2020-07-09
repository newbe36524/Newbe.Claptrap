using System.Collections.Generic;
using System.Threading.Tasks;
using HelloClaptrap.Actors.Cart.Events;
using HelloClaptrap.IActor;
using HelloClaptrap.Models;
using HelloClaptrap.Models.Cart;
using HelloClaptrap.Models.Cart.Events;
using Newbe.Claptrap;
using Newbe.Claptrap.Orleans;

namespace HelloClaptrap.Actors.Cart
{
    [ClaptrapEventHandler(typeof(AddItemToCartEventHandler), ClaptrapCodes.AddItemToCart)]
    [ClaptrapEventHandler(typeof(RemoveItemFromCartEventHandler), ClaptrapCodes.RemoveItemFromCart)]
    public class CartGrain : ClaptrapBoxGrain<CartState>, ICartGrain
    {
        public CartGrain(
            IClaptrapGrainCommonService claptrapGrainCommonService)
            : base(claptrapGrainCommonService)
        {
        }

        public async Task<Dictionary<string, int>> AddItemAsync(string skuId, int count)
        {
            var evt = this.CreateEvent(new AddItemToCartEvent
            {
                Count = count,
                SkuId = skuId,
            });
            await Claptrap.HandleEventAsync(evt);
            return StateData.Items;
        }

        public async Task<Dictionary<string, int>> RemoveItemAsync(string skuId, int count)
        {
            var evt = this.CreateEvent(new RemoveItemFromCartEvent
            {
                Count = count,
                SkuId = skuId
            });
            await Claptrap.HandleEventAsync(evt);
            return StateData.Items;
        }

        public Task<Dictionary<string, int>> GetItemsAsync()
        {
            var re = StateData.Items ?? new Dictionary<string, int>();
            return Task.FromResult(re);
        }
    }
}