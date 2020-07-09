using Newbe.Claptrap.StorageProvider.Relational.Options;

namespace Newbe.Claptrap.StorageProvider.MySql.Options
{
    public interface IMySqlMigrationOptions :
        IAutoMigrationOptions,
        IMySqlStorageProviderOptions
    {
    }
}