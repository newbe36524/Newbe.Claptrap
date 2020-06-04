namespace Newbe.Claptrap
{
    public class ClaptrapStorageProviderOptions
    {
        public IEventSaverOptions EventSaverOptions { get; set; } = null!;
        public IEventLoaderOptions EventLoaderOptions { get; set; } = null!;
        public IStateSaverOptions StateSaverOptions { get; set; } = null!;
        public IStateLoaderOptions StateLoaderOptions { get; set; } = null!;
    }
}