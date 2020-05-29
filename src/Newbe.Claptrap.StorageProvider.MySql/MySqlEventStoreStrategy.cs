namespace Newbe.Claptrap.StorageProvider.MySql
{
    public enum MySqlEventStoreStrategy
    {
        SharedTable,
        OneTypeOneTable,
        OneIdentityOneTable,
    }
}