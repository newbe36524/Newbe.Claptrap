namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface ISqlTemplateCache
    {
        string GetParameterName(string name, int index);
        void AddParameterName(string name, int index);
    }
}