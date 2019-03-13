using System.Reflection;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Demo.Interfaces;

namespace Newbe.Claptrap.Demo
{
    public class DemoModule : ClaptrapModuleBase
    {
        protected override Assembly InterfaceAssembly => typeof(IAccount).Assembly;
        protected override Assembly ImplementAssembly => typeof(DemoModule).Assembly;
    }
}