using System;
using Newbe.Claptrap.StorageProvider.MongoDB.Extensions;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseMongoDB(
            this IClaptrapBootstrapperBuilder builder,
            Action<MongoDBProviderConfigurator> mongoDB)
        {
            return builder.UseMongoDB(x => true, mongoDB);
        }

        public static IClaptrapBootstrapperBuilder UseMongoDB(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<MongoDBProviderConfigurator> mongoDB)
        {
            var configurator = new MongoDBProviderConfigurator(designFilter, builder);
            mongoDB(configurator);
            return builder;
        }
    }
}