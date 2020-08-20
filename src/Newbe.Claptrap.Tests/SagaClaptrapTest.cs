using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.Saga;
using NUnit.Framework;

namespace Newbe.Claptrap.Tests
{
    public class SagaClaptrapTest
    {
        private static IContainer CreateContainer()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            var builder = new ContainerBuilder();
            builder.Populate(serviceCollection);
            var autofacClaptrapBootstrapperBuilder =
                new AutofacClaptrapBootstrapperBuilder(new NullLoggerFactory(), builder);
            var claptrapBootstrapper = autofacClaptrapBootstrapperBuilder
                .ScanClaptrapDesigns(new[] {typeof(SagaClaptrap).Assembly})
                .ScanClaptrapModule()
                .UseSQLiteAsTestingStorage()
                .Build();
            claptrapBootstrapper.Boot();
            builder.RegisterType<SagaClaptrap>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<TestStep1>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<TestStep2>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<CompensateStep1>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<CompensateStep2>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<ExceptionStep>()
                .AsSelf()
                .InstancePerLifetimeScope();
            var container = builder.Build();
            return container;
        }

        [Test]
        public async Task Success()
        {
            var container = CreateContainer();
            await using var scope = container.BeginLifetimeScope();
            var factory = scope.Resolve<SagaClaptrap.Factory>();
            var identity = new ClaptrapIdentity(Guid.NewGuid().ToString(), SagaCodes.ClaptrapTypeCode);
            var claptrap = factory.Invoke(identity);
            await claptrap.Claptrap.ActivateAsync();
            var sagaFlow = SagaFlowBuilder.Create()
                .WithStep<TestStep1, CompensateStep1>()
                .WithStep<TestStep2, CompensateStep2>()
                .WithUserData(new Dictionary<string, string>())
                .Build();
            await claptrap.RunAsync(sagaFlow);
            var claptrapStateData = claptrap.StateData;
            claptrapStateData.SagaFlowState.IsCompleted.Should().BeTrue();
            var flowData = claptrapStateData.UserData;
            flowData.Should().Contain("1", true.ToString());
            flowData.Should().Contain("2", true.ToString());
            // there will be nothing change since flow completed.
            await claptrap.RunAsync(sagaFlow);
        }

        [Test]
        public async Task ExceptionAndCompensate()
        {
            var container = CreateContainer();
            await using var scope = container.BeginLifetimeScope();
            var factory = scope.Resolve<SagaClaptrap.Factory>();
            var identity = new ClaptrapIdentity(Guid.NewGuid().ToString(), SagaCodes.ClaptrapTypeCode);
            var claptrap = factory.Invoke(identity);
            await claptrap.Claptrap.ActivateAsync();
            var sagaFlow = SagaFlowBuilder.Create()
                .WithStep<TestStep1, CompensateStep1>()
                .WithStep<ExceptionStep, CompensateStep2>()
                .WithUserData(new Dictionary<string, string>())
                .Build();
            await claptrap.RunAsync(sagaFlow);
            var claptrapStateData = claptrap.StateData;
            var flowState = claptrapStateData.SagaFlowState;
            flowState.IsCompleted.Should().BeFalse();
            flowState.IsCompensating.Should().BeFalse();
            flowState.IsCompensated.Should().BeTrue();
            flowState.StepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Error);
            flowState.CompensateStepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Completed);
            var flowData = claptrapStateData.UserData;
            flowData.Should().Contain("1", false.ToString());
            flowData.Should().Contain("2", false.ToString());
        }

        [Test]
        public async Task CompensateWithException()
        {
            var container = CreateContainer();
            await using var scope = container.BeginLifetimeScope();
            var factory = scope.Resolve<SagaClaptrap.Factory>();
            var identity = new ClaptrapIdentity(Guid.NewGuid().ToString(), SagaCodes.ClaptrapTypeCode);
            var claptrap = factory.Invoke(identity);
            await claptrap.Claptrap.ActivateAsync();
            var sagaFlow = SagaFlowBuilder.Create()
                .WithStep<TestStep1, CompensateStep1>()
                .WithStep<ExceptionStep, ExceptionStep>()
                .WithUserData(new Dictionary<string, string>())
                .Build();
            Assert.ThrowsAsync<Exception>(() => claptrap.RunAsync(sagaFlow));
            var claptrapStateData = claptrap.StateData;
            var flowState = claptrapStateData.SagaFlowState;
            flowState.IsCompleted.Should().BeFalse();
            flowState.IsCompensating.Should().BeTrue();
            flowState.IsCompensated.Should().BeFalse();
            flowState.StepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Error);
            flowState.CompensateStepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Error);
            var flowData = claptrapStateData.UserData;
            flowData.Should().Contain("1", false.ToString());
            flowData.Should().NotContainKey("2");
        }

        public class TestStep1 : ISagaStep
        {
            public Task RunAsync(int stepIndex, SagaFlowState flowState, Dictionary<string, string> userData)
            {
                userData["1"] = true.ToString();
                return Task.CompletedTask;
            }
        }

        public class CompensateStep1 : ISagaStep
        {
            public Task RunAsync(int stepIndex, SagaFlowState flowState, Dictionary<string, string> userData)
            {
                userData["1"] = false.ToString();
                return Task.CompletedTask;
            }
        }

        public class TestStep2 : ISagaStep
        {
            public Task RunAsync(int stepIndex, SagaFlowState flowState, Dictionary<string, string> userData)
            {
                userData["2"] = true.ToString();
                return Task.CompletedTask;
            }
        }

        public class CompensateStep2 : ISagaStep
        {
            public Task RunAsync(int stepIndex, SagaFlowState flowState, Dictionary<string, string> userData)
            {
                userData["2"] = false.ToString();
                return Task.CompletedTask;
            }
        }

        public class ExceptionStep : ISagaStep
        {
            public Task RunAsync(int stepIndex, SagaFlowState flowState, Dictionary<string, string> userData)
            {
                throw new Exception();
            }
        }
    }
}