using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using HelloClaptrap.IActor;
using HelloClaptrap.Models;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap;
using Newbe.Claptrap.Dapr;

namespace HelloClaptrap.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionItemsController : ControllerBase
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public AuctionItemsController(
            IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }

        [HttpGet("{itemId:int}/status")]
        public async Task<IActionResult> GetStatus(int itemId = 1)
        {
            var id = new ClaptrapIdentity(itemId.ToString(),
                ClaptrapCodes.AuctionItemActor);
            var auctionItemActor = _actorProxyFactory.GetClaptrap<IAuctionItemActor>(id);
            var status = await auctionItemActor.GetStatusAsync();
            var result = new
            {
                status
            };
            return Ok(result);
        }

        [HttpGet("{itemId:int}/topPrice")]
        public async Task<IActionResult> GetTopPrice(int itemId = 1)
        {
            var id = new ClaptrapIdentity(itemId.ToString(),
                ClaptrapCodes.AuctionItemActor);
            var auctionItemActor = _actorProxyFactory.GetClaptrap<IAuctionItemActor>(id);
            var topPrice = await auctionItemActor.GetTopPriceAsync();
            var result = new
            {
                topPrice
            };
            return Ok(result);
        }

        [HttpGet("{itemId:int}")]
        public async Task<IActionResult> GetState(int itemId = 1)
        {
            var id = new ClaptrapIdentity(itemId.ToString(),
                ClaptrapCodes.AuctionItemActor);
            var auctionItemActor = _actorProxyFactory.GetClaptrap<IAuctionItemActor>(id);
            var state = await auctionItemActor.GetStateAsync();
            var result = new
            {
                state = state
            };
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> TryBidding([FromBody] TryBiddingWebApiInput webApiInput)
        {
            var input = new TryBiddingInput
            {
                Price = webApiInput.Price,
                UserId = webApiInput.UserId,
            };
            var itemId = webApiInput.ItemId;
            var id = new ClaptrapIdentity(itemId.ToString(),
                ClaptrapCodes.AuctionItemActor);
            var auctionItemActor = _actorProxyFactory.GetClaptrap<IAuctionItemActor>(id);
            var result = await auctionItemActor.TryBidding(input);
            return Ok(result);
        }

        [HttpGet("{itemId:int}/biddingcount")]
        public async Task<IActionResult> GetBiddingCount(int itemId = 1)
        {
            var id = new ClaptrapIdentity(itemId.ToString(),
                ClaptrapCodes.AuctionItemActor);
            var auctionItemActor = _actorProxyFactory.GetClaptrap<IAuctionItemUserCountMinionActor>(id);
            var result = await auctionItemActor.GetUserBiddingCountAsync();
            return Ok(result);
        }
    }

    public record TryBiddingWebApiInput
    {
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public int ItemId { get; set; }
    }
}