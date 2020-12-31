using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using Newbe.Claptrap.Dapr;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;

namespace Newbe.Claptrap.Demo.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MinionController : ControllerBase
    {
        private readonly IActorProxyFactory _factory;

        public MinionController(IActorProxyFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<IActionResult> InsertAsync()
        {
            var accountId = "1";
            var account = _factory.GetClaptrap<IAccount>(accountId);
            var accountBalanceMinion = _factory.GetClaptrap<IAccountBalanceMinion>(accountId);
            var oldBalance = await account.GetBalanceAsync();
            var oldMinionBalance = await accountBalanceMinion.GetBalance();
            var newBalance = await account.TransferInAsync(1);
            await Task.Delay(TimeSpan.FromSeconds(1));
            var newMinionBalance = await accountBalanceMinion.GetBalance();
            var re = new Dictionary<string, object>
            {
                {nameof(oldBalance), oldBalance},
                {nameof(oldMinionBalance), oldMinionBalance},
                {nameof(newBalance), newBalance},
                {nameof(newMinionBalance), newMinionBalance},
            };
            return Ok(re);
        }
    }
}