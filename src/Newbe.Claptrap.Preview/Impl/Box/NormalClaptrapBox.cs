using Newbe.Claptrap.Preview.Abstractions.Box;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl.Box
{
    public class NormalClaptrapBox : IClaptrapBox
    {
        public delegate NormalClaptrapBox Factory(IClaptrapIdentity identity);

        public NormalClaptrapBox(
            IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory)
        {
            Claptrap = claptrapFactory.Create(identity);
        }

        public IClaptrap Claptrap { get; }
    }
}