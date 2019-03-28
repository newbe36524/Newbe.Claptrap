using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.ScaffoldGenerator;

namespace Newbe.Claptrap.DevTools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ScaffoldGeneratorBuilder();

            const string interfaceProjectName = "Newbe.Claptrap.Demo.Interfaces";
            var interfaceProjectFullPath = FindProjectPath(interfaceProjectName);
            const string scaffoldProjectName = "Newbe.Claptrap.Demo.Scaffold";
            var scaffoldProjectFullPath = FindProjectPath(scaffoldProjectName);

            Console.WriteLine("building scaffoldGenerator");
            builder
                .SetInterfaceAssembly(typeof(IAccount).Assembly)
                .SetInterfaceProjectPath(interfaceProjectFullPath)
                .SetScaffoldProjectPath(scaffoldProjectFullPath);
            var scaffoldGenerator = builder.Build();
            Console.WriteLine("scaffoldGenerator built");

            Console.WriteLine("start to generate scaffold");
            await scaffoldGenerator.Generate();
            Console.WriteLine("finish generating scaffold");
            Console.WriteLine("please adjust namespace in scaffold by Rider or R#. Have fun!");

            string FindProjectPath(string projectName)
            {
                Console.WriteLine($"start to find directory path for {projectName}");
                var path = FindProject(projectName);
                if (string.IsNullOrEmpty(path))
                {
                    throw new DirectoryNotFoundException(
                        $"project directory for {projectName} not found, please check it.");
                }

                Console.WriteLine($"directory path for {projectName} found: {path}");
                return path;
            }
        }

        private static string FindProject(string projectName)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectFullPath = GetParentDirectories()
                .FirstOrDefault(x => Path.GetFileName(x) == projectName);
            return projectFullPath;

            IEnumerable<string> GetParentDirectories()
            {
                var now = baseDirectory;
                do
                {
                    foreach (var directory in Directory.GetDirectories(now))
                    {
                        yield return directory;
                    }

                    now = Directory.GetParent(now).FullName;
                } while (now != Path.GetPathRoot(now));
            }
        }
    }
}