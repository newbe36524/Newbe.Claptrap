using System.Threading.Tasks;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public class ScaffoldGenerator : IScaffoldGenerator
    {
        private readonly IClaptrapScaffoldGenerator _claptrapScaffoldGenerator;
        private readonly IMinionScaffoldGenerator _minionScaffoldGenerator;

        public ScaffoldGenerator(
            IClaptrapScaffoldGenerator claptrapScaffoldGenerator,
            IMinionScaffoldGenerator minionScaffoldGenerator)
        {
            _claptrapScaffoldGenerator = claptrapScaffoldGenerator;
            _minionScaffoldGenerator = minionScaffoldGenerator;
        }

        public async Task Generate(ScaffoldGenerateContext context)
        {
            foreach (var claptrapMetadata in context.ActorMetadataCollection.ClaptrapMetadata)
            {
                await _claptrapScaffoldGenerator.Generate(claptrapMetadata)
                    .ConfigureAwait(false);
            }

            foreach (var minionMetadata in context.ActorMetadataCollection.MinionMetadata)
            {
                await _minionScaffoldGenerator.Generate(minionMetadata)
                    .ConfigureAwait(false);
            }
        }
    }
}