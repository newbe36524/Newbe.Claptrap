using System.Collections.Generic;
using System.Threading.Tasks;
using HelloClaptrap.Models;
using HelloClaptrap.Models.Cart;
using HelloClaptrap.Models.Cart.Events;
using Newbe.Claptrap;
using Newbe.Claptrap.Orleans;

namespace HelloClaptrap.IActor
{
    [ClaptrapState(typeof(CartState), ClaptrapCodes.CartGrain)]
    [ClaptrapEvent(typeof(AddItemToCartEvent), ClaptrapCodes.AddItemToCart)]
    [ClaptrapEvent(typeof(RemoveItemFromCartEvent), ClaptrapCodes.RemoveItemFromCart)]
    public interface ICartGrain : IClaptrapGrain
    {
        Task<Dictionary<string, int>> AddItemAsync(string skuId, int count);
        Task<Dictionary<string, int>> RemoveItemAsync(string skuId, int count);
        Task<Dictionary<string, int>> GetItemsAsync();
    }
}