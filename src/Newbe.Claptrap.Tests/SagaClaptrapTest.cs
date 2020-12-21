using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            serviceCollection.AddLogging(logging => logging.AddConsole());
            var builder = new ContainerBuilder();
            builder.Populate(serviceCollection);
            var autofacClaptrapBootstrapperBuilder =
                new AutofacClaptrapBootstrapperBuilder(new NullLoggerFactory());
            var claptrapBootstrapper = (AutofacClaptrapBootstrapper) autofacClaptrapBootstrapperBuilder
                .ScanClaptrapDesigns(new[] {typeof(SagaClaptrap).Assembly})
                .ScanClaptrapModule()
                .ConfigureClaptrapDesign(x =>
                    x.ClaptrapOptions.EventCenterOptions.EventCenterType = EventCenterType.None)
                .UseSQLiteAsTestingStorage()
                .Build();
            claptrapBootstrapper.Builder = builder;
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
            var masterIdentity = new ClaptrapIdentity(Guid.NewGuid().ToString(), "TestCode");
            await using var claptrap = scope.CreateSagaClaptrap(masterIdentity, "testFlow", typeof(TestFlowData));
            var sagaFlow = SagaFlowBuilder.Create()
                .WithStep<TestStep1, CompensateStep1>()
                .WithStep<TestStep2, CompensateStep2>()
                .WithUserData(new TestFlowData())
                .Build();
            await claptrap.RunAsync(sagaFlow);
            var claptrapAccessor = scope.Resolve<IClaptrapAccessor>();
            var claptrapStateData = (SagaStateData<TestFlowData>) claptrapAccessor.Claptrap.State.Data;
            claptrapStateData.SagaFlowState.IsCompleted.Should().BeTrue();
            var flowData = (TestFlowData) claptrapStateData.GetUserData();
            flowData.Test1.Should().BeTrue();
            flowData.Test2.Should().BeTrue();
            // there will be nothing change since flow completed.
            await claptrap.RunAsync(sagaFlow);
        }

        [Test]
        public async Task ExceptionAndCompensate()
        {
            var container = CreateContainer();
            await using var scope = container.BeginLifetimeScope();
            var masterIdentity = new ClaptrapIdentity(Guid.NewGuid().ToString(), "TestCode");
            await using var claptrap = scope.CreateSagaClaptrap(masterIdentity, "testFlow", typeof(TestFlowData));
            var sagaFlow = SagaFlowBuilder.Create()
                .WithStep<TestStep1, CompensateStep1>()
                .WithStep<ExceptionStep, CompensateStep2>()
                .WithUserData(new TestFlowData())
                .Build();
            var claptrapAccessor = scope.Resolve<IClaptrapAccessor>();
            await claptrap.RunAsync(sagaFlow);
            var claptrapStateData = (SagaStateData<TestFlowData>) claptrapAccessor.Claptrap.State.Data;
            var flowState = claptrapStateData.SagaFlowState;
            flowState.IsCompleted.Should().BeFalse();
            flowState.IsCompensating.Should().BeFalse();
            flowState.IsCompensated.Should().BeTrue();
            flowState.StepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Error);
            flowState.CompensateStepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Completed);
            var flowData = (TestFlowData) claptrapStateData.GetUserData();
            flowData.Test1.Should().BeFalse();
            flowData.Test2.Should().BeFalse();
            await claptrap.RunAsync(sagaFlow);
        }

        [Test]
        public async Task CompensateWithException()
        {
            var container = CreateContainer();
            await using var scope = container.BeginLifetimeScope();
            var masterIdentity = new ClaptrapIdentity(Guid.NewGuid().ToString(), "TestCode");
            await using var claptrap = scope.CreateSagaClaptrap(masterIdentity, "testFlow", typeof(TestFlowData));
            var sagaFlow = SagaFlowBuilder.Create()
                .WithStep<TestStep1, CompensateStep1>()
                .WithStep<ExceptionStep, ExceptionStep>()
                .WithUserData(new TestFlowData())
                .Build();
            Assert.ThrowsAsync<Exception>(() => claptrap.RunAsync(sagaFlow));
            var claptrapAccessor = scope.Resolve<IClaptrapAccessor>();
            var claptrapStateData = (SagaStateData<TestFlowData>) claptrapAccessor.Claptrap.State.Data;
            var flowState = claptrapStateData.SagaFlowState;
            flowState.IsCompleted.Should().BeFalse();
            flowState.IsCompensating.Should().BeTrue();
            flowState.IsCompensated.Should().BeFalse();
            flowState.StepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Error);
            flowState.CompensateStepStatuses.Should().Equal(StepStatus.Completed, StepStatus.Error);
            var flowData = (TestFlowData) claptrapStateData.GetUserData();
            flowData.Test1.Should().BeFalse();
            flowData.Test2.Should().BeFalse();
        }

        public class TestStep1 : SagaStep<TestFlowData>
        {
            public override Task RunAsync(SagaStepData<TestFlowData> stepData)
            {
                stepData.UserData.Test1 = true;
                return Task.CompletedTask;
            }
        }

        public class CompensateStep1 : SagaStep<TestFlowData>
        {
            public override Task RunAsync(SagaStepData<TestFlowData> stepData)
            {
                stepData.UserData.Test1 = false;
                return Task.CompletedTask;
            }
        }

        public class TestStep2 : SagaStep<TestFlowData>
        {
            public override Task RunAsync(SagaStepData<TestFlowData> stepData)
            {
                stepData.UserData.Test2 = true;
                return Task.CompletedTask;
            }
        }

        public class CompensateStep2 : SagaStep<TestFlowData>
        {
            public override Task RunAsync(SagaStepData<TestFlowData> stepData)
            {
                stepData.UserData.Test2 = false;
                return Task.CompletedTask;
            }
        }

        public class ExceptionStep : ISagaStep
        {
            public Task RunAsync(SagaStepData data)
            {
                throw new Exception();
            }
        }

        public class TestFlowData : IStateData
        {
            public bool Test1 { get; set; }
            public bool Test2 { get; set; }
        }
    }
}