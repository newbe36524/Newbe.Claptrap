namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public enum SQLiteEventStoreStrategy
    {
        /// <summary>
        /// Each claptrap own it`s own db file to store event
        /// </summary>
        OneIdOneFile = 1,

        /// <summary>
        /// share table 
        /// </summary>
        SharedTable = 2,
    }
}