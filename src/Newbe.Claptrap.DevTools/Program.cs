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
            builder
                .SetInterfaceAssembly(typeof(IAccount).Assembly)
                .SetInterfaceProjectPath("../Newbe.Claptrap.Demo.Interfaces")
                .SetScaffoldProjectPath("../Newbe.Claptrap.Demo.Scaffold");
            var scaffoldGenerator = builder.Build();
            await scaffoldGenerator.Generate();
        }
    }
}