using System;

namespace Newbe.Claptrap.Box
{
    public abstract class NormalClaptrapBox : IClaptrapBox
    {
        private readonly IClaptrapAccessor _claptrapAccessor;

        public delegate NormalClaptrapBox Factory(IClaptrapIdentity identity);

        private readonly Lazy<IClaptrap> _createFromFactory;

        protected NormalClaptrapBox(
            IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory,
            IClaptrapAccessor claptrapAccessor)
        {
            _claptrapAccessor = claptrapAccessor;
            _createFromFactory = new Lazy<IClaptrap>(() =>
                _claptrapAccessor.Claptrap ??= claptrapFactory.Create(identity)
            );
        }

        public IClaptrap Claptrap => _claptrapAccessor.Claptrap ?? _createFromFactory.Value;
    }

    public abstract class NormalClaptrapBox<TStateData> : NormalClaptrapBox
        where TStateData : IStateData
    {
        protected NormalClaptrapBox(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory,
            IClaptrapAccessor claptrapAccessor) : base(identity,
            claptrapFactory,
            claptrapAccessor)
        {
        }

        public TStateData StateData => (TStateData) Claptrap!.State.Data;
    }
}