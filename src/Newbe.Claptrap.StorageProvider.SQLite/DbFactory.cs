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

        public string GetConnectionString(string connectionName)
        {
            var fileName = EnsureDirectoryCreated(connectionName);
            var re = DbHelper.ConnectionString(fileName);
            return re;
        }

        public IDbConnection GetConnection(string connectionName)
        {
            var fileName = EnsureDirectoryCreated(connectionName);
            var conn = new SQLiteConnection(GetConnectionString(fileName));
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
                _logger.LogInformation("{dir} found, do nothing", dataBaseDirectory);
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
                _logger.LogInformation("{dir} found, do nothing", claptrapDirectory);
            }

            return fileName;
        }

        private static string GetDataBaseDirectory()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "claptrapStorage");
            return dir;
        }
    }
}