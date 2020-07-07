using System.Threading.Tasks;
using HelloClaptrap.Models.Cart;
using HelloClaptrap.Models.Cart.Events;
using Newbe.Claptrap;

namespace HelloClaptrap.Actors.Cart.Events
{
    public class RemoveItemFromCartEventHandler
        : NormalEventHandler<CartState, RemoveItemFromCartEvent>
    {
        public override ValueTask HandleEvent(CartState stateData, RemoveItemFromCartEvent eventData,
            IEventContext eventContext)
        {
            var items = stateData.Items;

            if (items == null)
            {
                return new ValueTask();
            }

            var skuId = eventData.SkuId;
            if (items.TryGetValue(skuId, out var oldCount))
            {
                oldCount -= eventData.Count;
            }

            if (oldCount <= 0)
            {
                items.Remove(skuId);
            }

            return new ValueTask();
        }
    }
}