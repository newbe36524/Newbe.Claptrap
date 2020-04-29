using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public static class DbHelper
    {
        public static string GetEventTableName(IClaptrapIdentity claptrapIdentity)
        {
            return "events";
        }

        public static string GetStateTableName(IClaptrapIdentity claptrapIdentity)
        {
            return "state";
        }

        public static string GetDbFilename(IClaptrapIdentity claptrapIdentity)
        {
            return Path.Combine(GetDatabaseDirectory(), $"{claptrapIdentity.TypeCode}_{claptrapIdentity.Id}.db");
        }

        public static string ConnectionString(IClaptrapIdentity claptrapIdentity)
        {
            var fileName = GetDbFilename(claptrapIdentity);
            var re = $"Data Source={fileName};";
            return re;
        }

        public static string GetDatabaseDirectory()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "claptrapStorage");
            return dir;
        }

        public static SqliteConnection CreateInMemoryConnection(IClaptrapIdentity claptrapIdentity)
        {
            var connectionString =
                $"Data Source={claptrapIdentity.TypeCode}_{claptrapIdentity.Id};Mode=Memory;Cache=Shared";
            return new SqliteConnection(connectionString);
        }
    }
}