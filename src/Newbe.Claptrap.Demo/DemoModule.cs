using System.Reflection;
using Autofac;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Demo.Domain.Account.Claptrap;
using Newbe.Claptrap.Demo.Domain.Account.Minion.AccountDuplicate;
using Newbe.Claptrap.Demo.Domain.Account.Minion.ActorFlow;
using Newbe.Claptrap.Demo.Domain.Account.Minion.Database;
using Newbe.Claptrap.Demo.Interfaces;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Demo.Interfaces.DomainService;

namespace Newbe.Claptrap.Demo
{
    public class DemoModule : ClaptrapModuleBase
    {
        protected override Assembly InterfaceAssembly => typeof(IAccount).Assembly;
        protected override Assembly ImplementAssembly => typeof(DemoModule).Assembly;
    }
}