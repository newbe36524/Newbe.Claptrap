using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public class ReportManager : IReportManager
    {
        private readonly ILogger<ReportManager> _logger;
        private readonly string _resultDir;
        private readonly string _latestDir;
        private readonly string _currentDateDir;

        public ReportManager(
            ILogger<ReportManager> logger)
        {
            _logger = logger;
            _resultDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!,
                "TestResults");
            _latestDir = Path.Combine(_resultDir, "latest");
            _currentDateDir = Path.Combine(_resultDir, $"{DateTime.UtcNow:yyyyMMdd}");
        }

        public Task InitAsync()
        {
            _logger.LogInformation("Result dir : {dir}", _resultDir);
            if (Directory.Exists(_resultDir))
            {
                Directory.Delete(_resultDir, true);
            }

            CreateIfNotFound(_resultDir);
            CreateIfNotFound(_latestDir);
            CreateIfNotFound(_currentDateDir);

            return Task.CompletedTask;

            static void CreateIfNotFound(string path)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        public FileStream CreateFile(string filename)
        {
            var path = Path.Combine(_currentDateDir, filename);
            return new FileStream(path, FileMode.CreateNew);
        }

        public Task CopyAsync(string filename)
        {
            var source = Path.Combine(_currentDateDir, filename);
            var dest = Path.Combine(_latestDir, filename);
            File.Copy(source, dest);
            return Task.CompletedTask;
        }
    }
}