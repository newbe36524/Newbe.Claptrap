using System.Data;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public interface IAdoParameterCache
    {
        IDataParameter GetParameter(string name, int index);
        void AddParameterName(string name, int index, IDataParameter parameter);
    }
}