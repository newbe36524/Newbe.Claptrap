using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.SQLite
{
    public static class DbHelper
    {
        public static string GetEventTableName(IActorIdentity actorIdentity)
        {
            return "events";
        }

        public static string GetStateTableName(IActorIdentity actorIdentity)
        {
            return "state";
        }

        public static string GetDbFilename(IActorIdentity actorIdentity)
        {
            return Path.Combine(GetDatabaseDirectory(), $"{actorIdentity.TypeCode}_{actorIdentity.Id}.db");
        }

        public static string ConnectionString(IActorIdentity actorIdentity)
        {
            var fileName = GetDbFilename(actorIdentity);
            var re = $"Data Source={fileName};";
            return re;
        }

        public static string GetDatabaseDirectory()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "claptrapStorage");
            return dir;
        }

        public static SqliteConnection CreateInMemoryConnection(IActorIdentity actorIdentity)
        {
            var connectionString = $"Data Source={actorIdentity.TypeCode}_{actorIdentity.Id};Mode=Memory;Cache=Shared";
            return new SqliteConnection(connectionString);
        }
    }
}