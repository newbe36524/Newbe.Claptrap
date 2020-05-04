namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public interface IClaptrapBootstrapperBuilder
    {
        ClaptrapBootstrapperBuilderOptions Options { get; }

        IClaptrapBootstrapper Build();
    }
}