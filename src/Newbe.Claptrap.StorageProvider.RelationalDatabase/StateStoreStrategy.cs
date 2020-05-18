namespace Newbe.Claptrap.StorageProvider.RelationalDatabase
{
    public enum StateStoreStrategy
    {
        /// <summary>
        /// event will be store at default table
        /// </summary>
        DefaultTable,
        OneTypeOneTable,
        OneIdentityOneTable,
    }
}