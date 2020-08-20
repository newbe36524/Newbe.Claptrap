using System;

namespace Newbe.Claptrap.Saga
{
    public interface IDisposableSagaClaptrap : ISagaClaptrap, IAsyncDisposable
    {
    }
}