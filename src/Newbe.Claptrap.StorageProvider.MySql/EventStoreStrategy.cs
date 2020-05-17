namespace Newbe.Claptrap.StorageProvider.MySql
{
    public enum EventStoreStrategy
    {
        SharedTable,
        OneTypeOneTable,
        OneIdentityOneTable,
    }
}