using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public class SqlCache : ISqlCache
    {
        private readonly Dictionary<string, Dictionary<int, string>>
            _parameterNames = new Dictionary<string, Dictionary<int, string>>();

        private readonly Dictionary<int, string> _sql = new Dictionary<int, string>();

        public string Get(int key)
        {
            return _sql[key];
        }

        public void Add(int key, string sql)
        {
            _sql[key] = sql;
        }

        public string GetParameterName(string name, int index)
        {
            return _parameterNames[name][index];
        }

        public void AddParameterName(string name, int index)
        {
            if (!_parameterNames.TryGetValue(name, out var dic))
            {
                dic = new Dictionary<int, string>();
                _parameterNames.Add(name, dic);
            }

            if (!dic.TryGetValue(index, out var p))
            {
                dic[index] = $"@{name}{index}";
            }
        }
    }
}