using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using HelloClaptrap.Models;
using HelloClaptrap.Models.Cart;
using HelloClaptrap.Models.Cart.Events;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr.Core;

namespace HelloClaptrap.IActor
{
    [ClaptrapState(typeof(CartState), ClaptrapCodes.CartGrain)]
    [ClaptrapEvent(typeof(AddItemToCartEvent), ClaptrapCodes.AddItemToCart)]
    [ClaptrapEvent(typeof(RemoveItemFromCartEvent), ClaptrapCodes.RemoveItemFromCart)]
    public interface ICartGrain : IClaptrapActor
    {
        Task<Dictionary<string, int>> AddItemAsync(string skuId, int count);
        Task<Dictionary<string, int>> RemoveItemAsync(string skuId, int count);
        Task<Dictionary<string, int>> GetItemsAsync();
    }
}