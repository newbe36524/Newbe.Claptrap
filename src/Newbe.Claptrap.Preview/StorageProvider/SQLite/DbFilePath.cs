using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public class DbFilePath
    {
        public delegate DbFilePath Factory(IClaptrapIdentity identity);

        private readonly IClaptrapIdentity _identity;
        private readonly ILogger<DbFilePath> _logger;

        public DbFilePath(IClaptrapIdentity identity,
            ILogger<DbFilePath> logger)
        {
            _identity = identity;
            _logger = logger;
        }

        public string GetDataBaseDirectory()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "claptrapStorage");
            return dir;
        }

        public string GetClaptrapDirectory()
        {
            var re = Path.Combine(GetDataBaseDirectory(), $"{_identity.TypeCode}_{_identity.Id}");
            return re;
        }

        public string GetEventDbFilename()
        {
            var re = Path.Combine(GetClaptrapDirectory(), "eventDb.db");
            return re;
        }

        public string GetStateDbFilename()
        {
            var re = Path.Combine(GetClaptrapDirectory(), "stateDb.db");
            return re;
        }

        public void EnsureDirectoryCreated()
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

            var claptrapDirectory = GetClaptrapDirectory();
            if (!Directory.Exists(claptrapDirectory))
            {
                _logger.LogInformation("{dir} not found, try to create it", claptrapDirectory);
                Directory.CreateDirectory(claptrapDirectory);
                _logger.LogInformation("{dir} not found, created", claptrapDirectory);
            }
            else
            {
                _logger.LogInformation("{dir} found, do nothing", claptrapDirectory);
            }
        }
    }
}