using System;
using System.IO;
using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public class ReportManager : IReportManager
    {
        private readonly string _resultDir;
        private readonly string _latestDir;
        private readonly string _currentDateDir;

        public ReportManager()
        {
            _resultDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory!,
                "TestResults");
            _latestDir = Path.Combine(_resultDir, "latest");
            _currentDateDir = Path.Combine(_resultDir, $"{DateTime.UtcNow:yyyyMMdd}");
        }

        public Task InitAsync()
        {
            if (Directory.Exists(_resultDir))
            {
                Directory.Delete(_resultDir, true);
            }

            CreateIfNotFound(_resultDir);
            CreateIfNotFound(_latestDir);
            CreateIfNotFound(_currentDateDir);

            Environment.SetEnvironmentVariable("BENCHMARK_RESULT_DIR", _resultDir, EnvironmentVariableTarget.User);
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