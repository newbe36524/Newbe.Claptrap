namespace Newbe.Claptrap.StorageProvider.PostgreSQL
{
    public enum PostgreSQLEventStoreStrategy
    {
        SharedTable,
        OneTypeOneTable,
        OneIdentityOneTable,
    }
}