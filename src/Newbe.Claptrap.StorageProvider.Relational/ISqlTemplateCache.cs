using System;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface ISqlTemplateCache
    {
        string GetParameterName(string name, int index);
        void AddParameterName(string name, int index);
        void AddSql(int key, Func<string> sqlFunc);
        string GetSql(int key);
    }
}