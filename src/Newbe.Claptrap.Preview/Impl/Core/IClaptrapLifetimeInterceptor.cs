using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IClaptrapLifetimeInterceptor
    {
        Task ActivatingAsync();
        Task ActivatedAsync();
        Task ActivatingThrowExceptionAsync(Exception ex);

        Task DeactivatingAsync();
        Task DeactivatedAsync();
        Task DeactivatingThrowExceptionAsync(Exception ex);

        Task HandlingEventAsync(IEvent @event);
        Task HandledEventAsync(IEvent @event);
        Task HandlingEventThrowExceptionAsync(IEvent @event, Exception ex);
    }
}