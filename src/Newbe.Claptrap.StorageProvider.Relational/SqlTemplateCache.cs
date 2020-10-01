using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class SqlTemplateCache : ISqlTemplateCache
    {
        private readonly Dictionary<string, Dictionary<int, string>>
            _parameterNames = new Dictionary<string, Dictionary<int, string>>();

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

            if (!dic.TryGetValue(index, out _))
            {
                dic[index] = $"@{name}{index}";
            }
        }

        private readonly Dictionary<int, Lazy<string>> sqlDic = new Dictionary<int, Lazy<string>>();

        public void AddSql(int key, Func<string> sqlFunc)
        {
            sqlDic.Add(key, new Lazy<string>(sqlFunc));
        }

        public string GetSql(int key)
        {
            return sqlDic[key].Value;
        }
    }
}