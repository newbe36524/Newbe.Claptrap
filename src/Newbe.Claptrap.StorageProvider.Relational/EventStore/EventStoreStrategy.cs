namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public enum EventStoreStrategy
    {
        /// <summary>
        /// All in shared table
        /// </summary>
        SharedTable,

        /// <summary>
        /// One Type One Table
        /// </summary>
        OneTypeOneTable,
        OneIdentityOneTable,
    }
}