using System;
using System.Data;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface IAdoNetCache<TCommand>
        where TCommand : IDbCommand
    {
        IDataParameter GetParameter(string name, int index);
        void AddParameterName(string name, int index, IDataParameter parameter);
        TCommand GetCommand(int key);
        void AddCommand(int key, Func<TCommand> dbCommandFunc);
    }
}