using Newbe.Claptrap;

namespace HelloClaptrap.Models.Cart.Events
{
    public class AddItemToCartEvent : IEventData
    {
        public string SkuId { get; set; }
        public int Count { get; set; }
    }
}