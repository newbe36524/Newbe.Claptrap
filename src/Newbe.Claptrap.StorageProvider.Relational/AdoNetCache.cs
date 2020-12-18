using System;
using System.Collections.Generic;
using System.Data;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public abstract class AdoNetCache<TCommand> : IAdoNetCache<TCommand> where TCommand : IDbCommand
    {
        private readonly Dictionary<string, Dictionary<int, IDataParameter>>
            _parameters = new();

        public IDataParameter GetParameter(string name, int index)
        {
            return _parameters[name][index];
        }

        public void AddParameterName(string name, int index, IDataParameter parameter)
        {
            if (!_parameters.TryGetValue(name, out var dic))
            {
                dic = new Dictionary<int, IDataParameter>();
                _parameters[name] = dic;
            }

            dic[index] = parameter;
        }

        private readonly Dictionary<int, Lazy<TCommand>> _commandCache = new();

        public void AddCommand(int key, Func<TCommand> dbCommandFunc)
        {
            _commandCache[KeyPrefix + key] = new Lazy<TCommand>(dbCommandFunc);
        }

        public TCommand GetCommand(int key)
        {
            var dbCommand = _commandCache[KeyPrefix + key].Value;
            dbCommand.Parameters.Clear();
            return dbCommand;
        }

        public virtual int KeyPrefix { get; } = 10_000_000;
    }
}