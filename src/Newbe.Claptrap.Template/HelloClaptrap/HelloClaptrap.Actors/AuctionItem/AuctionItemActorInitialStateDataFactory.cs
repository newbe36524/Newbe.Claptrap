using System;
using System.Threading.Tasks;
using HelloClaptrap.Models.AuctionItem;
using Newbe.Claptrap;

namespace HelloClaptrap.Actors.AuctionItem
{
    public class AuctionItemActorInitialStateDataFactory : IInitialStateDataFactory
    {
        public static AuctionItemState[] States { get; set; }

        static AuctionItemActorInitialStateDataFactory()
        {
            var dateTime = DateTime.Now;
            var endTime = dateTime.AddHours(+2);
            var startTime = dateTime.AddHours(-2);
            States = new[]
            {
                new AuctionItemState
                {
                    StartTime = startTime.AddHours(-4),
                    EndTime = startTime
                },
                new AuctionItemState
                {
                    StartTime = startTime,
                    EndTime = endTime
                },
                new AuctionItemState
                {
                    StartTime = endTime,
                    EndTime = endTime.AddHours(4)
                }
            };
        }

        public Task<IStateData> Create(IClaptrapIdentity identity)
        {
            var i = int.Parse(identity.Id);
            var i1 = i % 3;

            var state = States[i1] with {BasePrice = 10};
            return Task.FromResult((IStateData) state);
        }
    }
}