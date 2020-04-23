using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Autofac
{
    public class OrderBasedActorTypeRegistrationCombiner : IActorTypeRegistrationCombiner
    {
        private readonly ILogger<OrderBasedActorTypeRegistrationCombiner> _logger;

        public OrderBasedActorTypeRegistrationCombiner(
            ILogger<OrderBasedActorTypeRegistrationCombiner> logger)
        {
            _logger = logger;
        }

        public ClaptrapRegistration Combine(IEnumerable<ClaptrapRegistration> claptrapRegistrations)
        {
            _logger.LogInformation(
                "OrderBasedActorTypeRegistrationCombiner is used. The later claptrap registered will be overwrite the former");
            var registrationsArray = claptrapRegistrations as ClaptrapRegistration[] ?? claptrapRegistrations.ToArray();
            _logger.LogInformation("there are {count} ClaptrapRegistration waiting to combine",
                registrationsArray.Length);

            var actorTypeRegistrations = registrationsArray
                .SelectMany(x => x.ActorTypeRegistrations)
                .Reverse()
                .Distinct(ActorTypeRegistration.ActorTypeCodeComparer)
                .ToArray();

            _logger.LogInformation(
                "actor type registrations combined. {leftCount} registrations left.",
                actorTypeRegistrations.Length);

            var eventHandlerTypeRegistrations = registrationsArray
                .SelectMany(x => x.EventHandlerTypeRegistrations)
                .Reverse()
                .Distinct(EventHandlerTypeRegistration.EventTypeCodeActorTypeCodeComparer)
                .ToArray();

            _logger.LogInformation(
                "event handler registrations combined. {leftCount} registrations left.",
                eventHandlerTypeRegistrations.Length);
            var re = new ClaptrapRegistration
            {
                ActorTypeRegistrations = actorTypeRegistrations,
                EventHandlerTypeRegistrations = eventHandlerTypeRegistrations
            };
            return re;
        }
    }
}