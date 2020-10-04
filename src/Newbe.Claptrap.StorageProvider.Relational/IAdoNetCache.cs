using System;
using System.Data;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface IAdoNetCache<TCommand> : IAdoParameterCache
        where TCommand : IDbCommand
    {
        TCommand GetCommand(int key);
        void AddCommand(int key, Func<TCommand> dbCommandFunc);
    }
}