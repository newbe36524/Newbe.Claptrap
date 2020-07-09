using System;

namespace Newbe.Claptrap
{
    public class ClaptrapDesignNotFoundException : Exception
    {
        public IClaptrapIdentity ClaptrapIdentity { get; }

        public ClaptrapDesignNotFoundException(IClaptrapIdentity claptrapIdentity)
            : this($"No claptrap design found for [{claptrapIdentity.TypeCode}:{claptrapIdentity.Id}]",
                claptrapIdentity)
        {
        }

        public ClaptrapDesignNotFoundException(string message, IClaptrapIdentity claptrapIdentity) : base(message)
        {
            ClaptrapIdentity = claptrapIdentity;
        }

        public ClaptrapDesignNotFoundException(string message, Exception innerException,
            IClaptrapIdentity claptrapIdentity) :
            base(message, innerException)
        {
            ClaptrapIdentity = claptrapIdentity;
        }
    }
}