using Autofac;
using Newbe.Claptrap.StorageTestConsole.Services;

namespace Newbe.Claptrap.StorageTestConsole
{
    public class StorageTestConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<EventSavingDirectlyTestJob>()
                .Keyed<ITestJob>(TestJobType.EventSavingDirectly)
                .SingleInstance();
            // builder.RegisterType<EventSavingActorSaverTestJob>()
            //     .Keyed<ITestJob>(TestJobType.EventSavingActorSaver)
            //     .SingleInstance();
            builder.RegisterType<SavingEventResultReportFormat>()
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterType<ReportManager>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }

    public enum TestJobType
    {
        EventSavingDirectly = 0,
        EventSavingActorSaver = 1
    }
}