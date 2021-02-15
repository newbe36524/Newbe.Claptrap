namespace Newbe.Claptrap.Dapr.Tests.TestActor.Core
{
    public record NewBidderEvent : IEventData
    {
        public int UserId { get; set; }
        public decimal Price { get; set; }
    }
}