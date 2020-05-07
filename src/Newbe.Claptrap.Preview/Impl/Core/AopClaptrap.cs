using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class AopClaptrap : IClaptrap
    {
        private readonly IClaptrap _claptrap;
        private readonly IEnumerable<IClaptrapLifetimeInterceptor> _interceptors;
        private readonly ILogger<AopClaptrap> _logger;

        public AopClaptrap(
            IClaptrap claptrap,
            IEnumerable<IClaptrapLifetimeInterceptor> interceptors,
            ILogger<AopClaptrap> logger)
        {
            _claptrap = claptrap;
            _interceptors = interceptors;
            _logger = logger;
        }

        public IState State => _claptrap.State;

        public async Task ActivateAsync()
        {
            await RunInterceptors(x => x.ActivatingAsync());

            try
            {
                await _claptrap.ActivateAsync();
                await RunInterceptors(x => x.ActivatedAsync());
            }
            catch (Exception e)
            {
                await RunInterceptors(x => x.ActivatingThrowExceptionAsync(e));
                throw;
            }
        }

        public async Task DeactivateAsync()
        {
            await RunInterceptors(x => x.DeactivatingAsync());

            try
            {
                await _claptrap.DeactivateAsync();
                await RunInterceptors(x => x.DeactivatedAsync());
            }
            catch (Exception e)
            {
                await RunInterceptors(x => x.DeactivatingThrowExceptionAsync(e));
                throw;
            }
        }

        public async Task HandleEventAsync(IEvent @event)
        {
            await RunInterceptors(x => x.HandlingEventAsync(@event));

            try
            {
                await _claptrap.HandleEventAsync(@event);
                await RunInterceptors(x => x.HandledEventAsync(@event));
            }
            catch (Exception e)
            {
                await RunInterceptors(x => x.HandlingEventThrowExceptionAsync(@event, e));
                throw;
            }
        }

        private Task RunInterceptors(Func<IClaptrapLifetimeInterceptor, Task> action)
        {
            return Task.WhenAll(CreateTasks());

            IEnumerable<Task> CreateTasks()
            {
                foreach (var interceptor in _interceptors)
                {
                    yield return Task.Run(async () =>
                    {
                        try
                        {
                            await action.Invoke(interceptor);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Something error");
                        }
                    });
                }
            }
        }
    }
}