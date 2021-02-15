using System;
using System.Collections.Concurrent;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class SqlTemplateCache : ISqlTemplateCache
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, string>> _ps = new();

        public string GetOrAddGetParameterName(string name, int index)
        {
            var innerDic = _ps.GetOrAdd(name, n => new ConcurrentDictionary<int, string>());
            var result = innerDic.GetOrAdd(index, i => $"@{name}{i}");
            return result;
        }

        private readonly ConcurrentDictionary<int, string> _sqlDic = new();

        public string GetOrAddSql(int key, Func<int, string> factory)
        {
            var re = _sqlDic.GetOrAdd(key, factory.Invoke);
            return re;
        }
    }
}