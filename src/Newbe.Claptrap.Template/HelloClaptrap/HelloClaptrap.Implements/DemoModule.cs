using System.Reflection;
using HelloClaptrap.Interfaces.Domain.Account;
using Newbe.Claptrap.Autofac;

namespace HelloClaptrap.Implements
{
    public class DemoModule : ClaptrapModuleBase
    {
        protected override Assembly InterfaceAssembly => typeof(IAccount).Assembly;
        protected override Assembly ImplementAssembly => typeof(DemoModule).Assembly;
    }
}