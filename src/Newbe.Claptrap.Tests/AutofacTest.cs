using Autofac;
using FluentAssertions;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class AutofacTest
    {
        [Test]
        public void SingleDelegateFactory()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Store>()
                .InstancePerDependency()
                .InstancePerLifetimeScope();
            builder.RegisterType<StoreFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();

            var container = builder.Build();

            using var scope = container.BeginLifetimeScope(scopeBuilder =>
            {
                scopeBuilder.Register(t => t.Resolve<StoreFactory>().Create())
                    .As<IStore>()
                    .SingleInstance();
            });

            var store1 = scope.Resolve<IStore>();
            store1.Name = "123";
            var store2 = scope.Resolve<IStore>();
            store1.Should().Be(store2, "In the same lifeTimeScope, that will be the same IStore instance");
            store1.Name.Should().Be(store2.Name);
        }

        [Test]
        public void DiffPerLifetimeScope()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Store>()
                .InstancePerDependency()
                .InstancePerLifetimeScope();
            builder.RegisterType<StoreFactory>()
                .AsSelf()
                .InstancePerLifetimeScope();

            var container = builder.Build();

            using var scope1 = container.BeginLifetimeScope(scopeBuilder =>
            {
                scopeBuilder.Register(t => t.Resolve<StoreFactory>().Create())
                    .As<IStore>()
                    .SingleInstance();
            });

            using var scope2 = container.BeginLifetimeScope(scopeBuilder =>
            {
                scopeBuilder.Register(t => t.Resolve<StoreFactory>().Create())
                    .As<IStore>()
                    .SingleInstance();
            });

            var store1 = scope1.Resolve<IStore>();
            store1.Name = "123";
            var store2 = scope2.Resolve<IStore>();
            store1.Should().NotBe(store2);
            store1.Name.Should().NotBe(store2.Name);
        }

        public interface IStore
        {
            string Name { get; set; }
        }

        public class Store : IStore
        {
            public delegate Store Factory();

            public string Name { get; set; }
        }

        public class StoreFactory
        {
            private readonly Store.Factory _factory;

            public StoreFactory(
                Store.Factory factory)
            {
                _factory = factory;
            }

            public Store Create()
            {
                return _factory();
            }
        }
    }
}