using System;
using RabbitMQ.Client;

namespace Newbe.Claptrap.EventCenter.RabbitMQ
{
    public interface IConnectionManager : IDisposable
    {
        IConnection CreateConnection();
    }
}