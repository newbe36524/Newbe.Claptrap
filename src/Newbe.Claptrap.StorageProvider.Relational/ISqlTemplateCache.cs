using System;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface ISqlTemplateCache
    {
        string GetOrAddGetParameterName(string name, int index);
        string GetOrAddSql(int key, Func<int, string> factory);
    }
}