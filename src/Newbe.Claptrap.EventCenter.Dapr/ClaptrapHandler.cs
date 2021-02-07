using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Http;
using Newbe.Claptrap.Dapr.Core;
using Newbe.Claptrap.Extensions;

namespace Newbe.Claptrap.EventCenter.Dapr
{
    public class ClaptrapHandler : IClaptrapHandler
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IEventSerializer<EventJsonModel> _eventSerializer;
        private readonly IEventStringSerializer _eventStringSerializer;
        private readonly Dictionary<string, IEnumerable<string>> _minionTypeCodes;

        public ClaptrapHandler(IActorProxyFactory actorProxyFactory,
            IClaptrapDesignStore claptrapDesignStore,
            IEventSerializer<EventJsonModel> eventSerializer,
            IEventStringSerializer eventStringSerializer)
        {
            _actorProxyFactory = actorProxyFactory;
            _eventSerializer = eventSerializer;
            _eventStringSerializer = eventStringSerializer;
            _minionTypeCodes = claptrapDesignStore.Where(x => x.IsMinion())
                .GroupBy(x => x.ClaptrapMasterDesign!.ClaptrapTypeCode)
                .ToDictionary(x => x.Key, x => x.Select(a => a.ClaptrapTypeCode));
        }

        public async Task HandleAsync(HttpContext context)
        {
            var sr = new StreamReader(context.Request.Body);
            var body = await sr.ReadToEndAsync();
            var e = _eventStringSerializer.Deserialize(body);
            var jsonModel = _eventSerializer.Serialize(e);

            var minionTypeCode = _minionTypeCodes[e.ClaptrapIdentity.TypeCode];

            var tasks = minionTypeCode.Select(async x =>
            {
                var minionId = new ClaptrapIdentity(e.ClaptrapIdentity.Id, x);
                var actorProxy = _actorProxyFactory.Create(new ActorId(minionId.Id), minionId.TypeCode);
                await actorProxy.InvokeMethodAsync(nameof(IClaptrapMinionActor.MasterEventReceivedJsonAsync),
                    new[] {jsonModel});
            });

            await Task.WhenAll(tasks);
        }
    }
}