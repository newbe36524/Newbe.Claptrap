using System.Collections.Generic;
using System.Threading.Tasks;
using HelloClaptrap.Models.Cart;
using HelloClaptrap.Models.Cart.Events;
using Newbe.Claptrap;

namespace HelloClaptrap.Actors.Cart.Events
{
    public class AddItemToCartEventHandler
        : NormalEventHandler<CartState, AddItemToCartEvent>
    {
        public override ValueTask HandleEvent(CartState stateData, AddItemToCartEvent eventData,
            IEventContext eventContext)
        {
            var items = stateData.Items ?? new Dictionary<string, int>();
            if (items.TryGetValue(eventData.SkuId, out var itemCount))
            {
                itemCount += eventData.Count;
            }
            // else
            // {
            //     itemCount = eventData.Count;
            // }

            items[eventData.SkuId] = itemCount;
            stateData.Items = items;
            return new ValueTask();
        }
    }
}