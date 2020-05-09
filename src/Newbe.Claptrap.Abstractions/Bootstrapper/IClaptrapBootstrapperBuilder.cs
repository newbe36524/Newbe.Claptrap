namespace Newbe.Claptrap.Bootstrapper
{
    public interface IClaptrapBootstrapperBuilder
    {
        ClaptrapBootstrapperBuilderOptions Options { get; }

        IClaptrapBootstrapper Build();
    }
}