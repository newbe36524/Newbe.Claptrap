namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public enum SQLiteStateStoreStrategy
    {
        /// <summary>
        /// Each claptrap own it`s own db file to store state
        /// </summary>
        OneIdOneFile = 1,

        /// <summary>
        /// Shared Table
        /// </summary>
        SharedTable = 2,
    }
}