using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class SqlTemplateCache : ISqlTemplateCache
    {
        private readonly object _locker = new();

        private readonly Dictionary<string, Dictionary<int, string>>
            _parameterNames = new();

        public string GetParameterName(string name, int index)
        {
            return _parameterNames[name][index];
        }

        public void AddParameterName(string name, int index)
        {
            lock (_locker)
            {
                if (!_parameterNames.TryGetValue(name, out var dic))
                {
                    dic = new Dictionary<int, string>();
                    _parameterNames.Add(name, dic);
                }

                if (!dic.TryGetValue(index, out _))
                {
                    dic[index] = $"@{name}{index}";
                }
            }
        }

        private readonly Dictionary<int, Lazy<string>> _sqlDic = new();

        public void AddSql(int key, Func<string> sqlFunc)
        {
            lock (_locker)
            {
                _sqlDic[key] = new Lazy<string>(sqlFunc);
            }
        }

        public string GetSql(int key)
        {
            return _sqlDic[key].Value;
        }
    }
}