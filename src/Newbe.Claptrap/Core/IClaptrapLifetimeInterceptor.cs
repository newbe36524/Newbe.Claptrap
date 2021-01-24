using System;
using System.Threading.Tasks;

namespace Newbe.Claptrap.Core
{
    public interface IClaptrapLifetimeInterceptor
    {
        Task ActivatingAsync()
        {
            return Task.CompletedTask;
        }

        Task ActivatedAsync()
        {
            return Task.CompletedTask;
        }

        Task ActivatingThrowExceptionAsync(Exception ex)
        {
            return Task.CompletedTask;
        }
        
        Task ActivatedFinallyAsync()
        {
            return Task.CompletedTask;
        }

        Task DeactivatingAsync()
        {
            return Task.CompletedTask;
        }

        Task DeactivatedAsync()
        {
            return Task.CompletedTask;
        }

        Task DeactivatingThrowExceptionAsync(Exception ex)
        {
            return Task.CompletedTask;
        }
        
        Task DeactivatedFinallyAsync()
        {
            return Task.CompletedTask;
        }

        Task HandlingEventAsync(IEvent @event)
        {
            return Task.CompletedTask;
        }

        Task HandledEventAsync(IEvent @event)
        {
            return Task.CompletedTask;
        }

        Task HandlingEventThrowExceptionAsync(IEvent @event, Exception ex)
        {
            return Task.CompletedTask;
        }
        
        Task HandledEventFinallyAsync(IEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}