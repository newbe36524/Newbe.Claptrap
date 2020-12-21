// using Autofac;
//
// namespace Newbe.Claptrap.Dapr.Modules
// {
//     public class OrleansDirectlyEventCenterModule : Module, IClaptrapMasterModule
//     {
//         public string Name { get; } = "Orleans directly event center notifier";
//         public string Description { get; } = "Module for support event notifier based on Orleans directly call";
//
//         protected override void Load(ContainerBuilder builder)
//         {
//             base.Load(builder);
//             builder.RegisterType<OrleansEventCenter>()
//                 .As<IEventCenter>()
//                 .SingleInstance();
//         }
//     }
// }