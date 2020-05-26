using System;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class MissingDbException : Exception
    {
        public string DbName { get; }

        public MissingDbException(string dbName)
            : this($"Db named {dbName} is missing, please check you code or configuration", dbName)
        {
            DbName = dbName;
        }

        public MissingDbException(string message, string dbName) : base(message)
        {
            DbName = dbName;
        }

        public MissingDbException(string message, Exception innerException, string dbName) : base(message,
            innerException)
        {
            DbName = dbName;
        }
    }
}