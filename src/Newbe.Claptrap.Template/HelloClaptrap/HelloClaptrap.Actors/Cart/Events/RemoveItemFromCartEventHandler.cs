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
            if (stateData.Items == null)
            {
                return new ValueTask();
            }

            if (stateData.Items.TryGetValue(eventData.SkuId, out var oldCount))
            {
                oldCount -= eventData.Count;
            }

            if (oldCount <= 0)
            {
                stateData.Items.Remove(eventData.SkuId);
            }

            return new ValueTask();
        }
    }
}