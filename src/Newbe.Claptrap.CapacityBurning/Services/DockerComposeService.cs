using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.CapacityBurning.Services
{
    public class DockerComposeService : IDockerComposeService
    {
        private readonly ILogger<DockerComposeService> _logger;

        public DockerComposeService(
            ILogger<DockerComposeService> logger)
        {
            _logger = logger;
        }

        public Task UpAsync(string composeContextDirectory)
        {
            RunProcess(composeContextDirectory, "docker-compose", "up", "-d");
            return Task.CompletedTask;
        }

        public Task DownAsync(string composeContextDirectory)
        {
            RunProcess(composeContextDirectory, "docker-compose", "down");
            return Task.CompletedTask;
        }

        private void RunProcess(string workingDirectory, string cmd, params string[] ps)
        {
            var cmdline = string.Join(" ", GetCmdLineParts());
            _logger.LogInformation("start to run : {cmdline}", cmdline);
            var startInfo = new ProcessStartInfo
            {
                FileName = cmd,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
            };
            if (ps?.Any() == true)
            {
                foreach (var s in ps)
                {
                    startInfo.ArgumentList.Add(s);
                }
            }

            var p = new Process
            {
                StartInfo = startInfo
            };
            p.OutputDataReceived += (sender, args) => { _logger.LogTrace(args.Data); };
            p.ErrorDataReceived += (sender, args) => { _logger.LogTrace(args.Data); };
            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();
            p.WaitForExit();
            _logger.LogInformation("run to end : {cmdline}", cmdline);

            IEnumerable<string> GetCmdLineParts()
            {
                yield return cmd;
                if (ps?.Any() == true)
                {
                    foreach (var s in ps)
                    {
                        yield return s;
                    }
                }
            }
        }
    }
}