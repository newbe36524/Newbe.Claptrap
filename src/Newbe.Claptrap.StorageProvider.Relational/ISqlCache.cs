namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface ISqlCache
    {
        string Get(int key);
        void Add(int key, string sql);
        string GetParameterName(string name, int index);
        void AddParameterName(string name, int index);
    }
}