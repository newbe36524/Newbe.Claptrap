using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dapper;
using NUnit.Framework;

namespace Newbe.Claptrap.StorageProvider.SQLite.Tests
{
    public class SQLiteInsertTest
    {
        public static async Task ResetDatabase(string file)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            var connectionString = DbHelper.ConnectionString(file);
            await using var sqLiteConnection = new SQLiteConnection(connectionString);
            await sqLiteConnection.OpenAsync();

            await sqLiteConnection.ExecuteAsync(@"
CREATE TABLE IF NOT EXISTS TestTable 
(
    someValue text not null
)  ;");
        }

        public static (SQLiteCommand cmd, IDataParameter p) CreateCommand()
        {
            const string sql = "INSERT INTO TestTable(someValue) VALUES ($value)";
            var p = new SQLiteParameter
            {
                ParameterName = "$value"
            };
            var cmd = new SQLiteCommand
            {
                CommandText = sql,
                CommandType = CommandType.Text,
                Parameters =
                {
                    p
                }
            };
            return (cmd, p);
        }

        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public async Task InsertWithCmd(int count)
        {
            const string methodName = nameof(InsertWithCmd);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            const int batchSize = 10000;
            var guid = Guid.NewGuid().ToString();
            var (cmd, p) = CreateCommand();

            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < pageCount; i++)
            {
                await using var sqLiteConnection = new SQLiteConnection(connectionString);
                cmd.Connection = sqLiteConnection;
                await sqLiteConnection.OpenAsync();
                await using var transaction = await sqLiteConnection.BeginTransactionAsync();
                for (var j = 0; j < batchSize; j++)
                {
                    p.Value = guid;
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }

            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds} ms");
        }

        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public async Task InsertCacheCmd(int count)
        {
            const string methodName = nameof(InsertCacheCmd);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            const int batchSize = 10000;
            var guid = Guid.NewGuid().ToString();
            var (cmd, p) = CreateCommand();
            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                await using var sqLiteConnection = new SQLiteConnection(connectionString);
                cmd.Connection = sqLiteConnection;
                await sqLiteConnection.OpenAsync();
                await using var beginTransactionAsync = await sqLiteConnection.BeginTransactionAsync();
                for (var j = 0; j < batchSize; j++)
                {
                    p.Value = guid;
                    await cmd.ExecuteNonQueryAsync();
                }

                await beginTransactionAsync.CommitAsync();
            }

            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds}");
        }

        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        public async Task InsertTransaction(int count)
        {
            const string methodName = nameof(InsertTransaction);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            var batchSize = 10000;
            var guid = Guid.NewGuid().ToString();
            var (cmd, p) = CreateCommand();

            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                await using var sqLiteConnection = new SQLiteConnection(connectionString);
                await sqLiteConnection.OpenAsync();
                await using var beginTransactionAsync = await sqLiteConnection.BeginTransactionAsync();
                cmd.Connection = sqLiteConnection;
                for (var j = 0; j < batchSize; j++)
                {
                    p.Value = guid;
                    await cmd.ExecuteNonQueryAsync();
                }

                await beginTransactionAsync.CommitAsync();
            }

            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Cache SQL Command
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [Test]
        [TestCase(100)]
        [TestCase(1_000)]
        [TestCase(10_000)]
        [TestCase(100_000)]
        [TestCase(500_000)]
        [TestCase(1_000_000)]
        public async Task InsertSingleConnection(int count)
        {
            const string methodName = nameof(InsertSingleConnection);
            var file = $"db{count}{methodName}.db";
            var connectionString = DbHelper.ConnectionString(file);
            await ResetDatabase(file);
            await using var sqLiteConnection = new SQLiteConnection(connectionString);
            await sqLiteConnection.OpenAsync();
            var batchSize = 10000;
            var guid = Guid.NewGuid().ToString();

            var (cmd, p) = CreateCommand();
            cmd.Connection = sqLiteConnection;
            var pageCount = count / batchSize;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < pageCount; i++)
            {
                await using var transaction = await sqLiteConnection.BeginTransactionAsync();
                for (var j = 0; j < batchSize; j++)
                {
                    p.Value = guid;
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }

            sw.Stop();
            Console.WriteLine($"cost: {sw.ElapsedMilliseconds} ms");
        }
    }
}