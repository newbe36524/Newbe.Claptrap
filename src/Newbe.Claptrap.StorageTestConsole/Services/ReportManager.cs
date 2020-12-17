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

        public ReportManager(
            ILogger<ReportManager> logger)
        {
            _logger = logger;
            _resultDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!,
                "TestResults");
        }

        public Task InitAsync()
        {
            _logger.LogInformation("Result dir : {dir}", _resultDir);
            if (Directory.Exists(_resultDir))
            {
                Directory.Delete(_resultDir, true);
            }

            CreateIfNotFound(_resultDir);

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
            var path = Path.Combine(_resultDir, filename);
            return new FileStream(path, FileMode.CreateNew);
        }
    }
}