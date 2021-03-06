using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteDbFactory : ISQLiteDbFactory
    {
        private readonly ILogger<SQLiteDbFactory> _logger;

        public SQLiteDbFactory(
            ILogger<SQLiteDbFactory> logger)
        {
            _logger = logger;
        }

        public string GetConnectionString(string connectionName)
        {
            var fileName = EnsureDirectoryCreated(connectionName);
            var re = DbHelper.ConnectionString(fileName);
            return re;
        }

        private readonly Dictionary<string, SQLiteConnection> _keepOpenedConnections =
            new Dictionary<string, SQLiteConnection>();

        public SQLiteConnection GetConnection(string connectionName, bool keepOpen = false)
        {
            if (keepOpen && _keepOpenedConnections.TryGetValue(connectionName, out var conn))
            {
                return conn;
            }

            var fileName = EnsureDirectoryCreated(connectionName);
            conn = new SQLiteConnection(GetConnectionString(fileName));
            if (keepOpen)
            {
                _keepOpenedConnections[connectionName] = conn;
                conn.Open();
            }

            return conn;
        }

        private string EnsureDirectoryCreated(string connectionName)
        {
            var dataBaseDirectory = GetDataBaseDirectory();
            if (!Directory.Exists(dataBaseDirectory))
            {
                _logger.LogInformation("{dir} not found, try to create it", dataBaseDirectory);
                Directory.CreateDirectory(dataBaseDirectory);
                _logger.LogInformation("{dir} not found, created", dataBaseDirectory);
            }
            else
            {
                _logger.LogTrace("{dir} found, do nothing", dataBaseDirectory);
            }

            var fileName = Path.Combine(dataBaseDirectory, connectionName);
            var claptrapDirectory = Directory.GetParent(fileName);

            if (!claptrapDirectory.Exists)
            {
                _logger.LogInformation("{dir} not found, try to create it", claptrapDirectory);
                claptrapDirectory.Create();
                _logger.LogInformation("{dir} not found, created", claptrapDirectory);
            }
            else
            {
                _logger.LogTrace("{dir} found, do nothing", claptrapDirectory);
            }

            return fileName;
        }

        private const string DataBaseDirectoryEnvKeyName = "CLAPTRAP_STORAGE";

        private static string GetDataBaseDirectory()
        {
            var dirName = Environment.GetEnvironmentVariable(DataBaseDirectoryEnvKeyName);
            dirName ??= "claptrapStorage";
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dirName);
            return dir;
        }

        public static void SetDataBaseDirectoryName(string name)
        {
            Environment.SetEnvironmentVariable(DataBaseDirectoryEnvKeyName, name);
        }

        public static void RemoveDataBaseDirectory()
        {
            var dir = GetDataBaseDirectory();
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
        }
    }
}