namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public enum EventStoreStrategy
    {
        /// <summary>
        /// event will be store at default table
        /// </summary>
        SharedTable,
        OneTypeOneTable,
        OneIdentityOneTable,
    }
}