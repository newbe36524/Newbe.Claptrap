using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class DbFactory : IDbFactory
    {
        private readonly ILogger<DbFactory> _logger;

        public DbFactory(
            ILogger<DbFactory> logger)
        {
            _logger = logger;
        }

        public string GetConnectionString(string dbName)
        {
            EnsureDirectoryCreated(dbName);
            var re = DbHelper.ConnectionString(dbName);
            return re;
        }

        public IDbConnection GetConnection(string dbName)
        {
            EnsureDirectoryCreated(dbName);
            var conn = new SQLiteConnection(GetConnectionString(dbName));
            return conn;
        }

        private void EnsureDirectoryCreated(string dbName)
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
                _logger.LogInformation("{dir} found, do nothing", dataBaseDirectory);
            }

            var claptrapDirectory = Directory.GetParent(Path.Combine(dataBaseDirectory, dbName));

            if (!claptrapDirectory.Exists)
            {
                _logger.LogInformation("{dir} not found, try to create it", claptrapDirectory);
                claptrapDirectory.Create();
                _logger.LogInformation("{dir} not found, created", claptrapDirectory);
            }
            else
            {
                _logger.LogInformation("{dir} found, do nothing", claptrapDirectory);
            }
        }

        private static string GetDataBaseDirectory()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "claptrapStorage");
            return dir;
        }
    }
}