using System;

namespace Newbe.Claptrap
{
    public class ActivateFailException : Exception
    {
        public IClaptrapIdentity ClaptrapIdentity { get; }

        public ActivateFailException(IClaptrapIdentity claptrapIdentity)
            : this(CreateExceptionMessage(claptrapIdentity), claptrapIdentity)
        {
            ClaptrapIdentity = claptrapIdentity;
        }

        public ActivateFailException(string message, IClaptrapIdentity claptrapIdentity) : base(message)
        {
            ClaptrapIdentity = claptrapIdentity;
        }

        public ActivateFailException(string message, Exception innerException, IClaptrapIdentity claptrapIdentity) :
            base(
                message, innerException)
        {
            ClaptrapIdentity = claptrapIdentity;
        }

        public ActivateFailException(Exception innerException, IClaptrapIdentity claptrapIdentity) : base(
            CreateExceptionMessage(claptrapIdentity), innerException)
        {
            ClaptrapIdentity = claptrapIdentity;
        }

        private static string CreateExceptionMessage(IClaptrapIdentity claptrapIdentity)
        {
            return $"failed to activate actor : {claptrapIdentity.TypeCode} {claptrapIdentity.Id}";
        }
    }
}