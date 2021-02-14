namespace Newbe.Claptrap
{
    public interface IClaptrapBootstrapperBuilder
    {
        ClaptrapBootstrapperBuilderOptions Options { get; }
        IClaptrapBootstrapper Build();
    }
}