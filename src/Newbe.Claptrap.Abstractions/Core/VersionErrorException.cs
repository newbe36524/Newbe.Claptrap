using System;

namespace Newbe.Claptrap
{
    public class VersionErrorException : Exception
    {
        public VersionErrorException(long nowStateVersion, long eventVersion)
            : this($"version error, current state version : {nowStateVersion}, eventVersion : {eventVersion}",
                nowStateVersion,
                eventVersion)
        {
        }

        public VersionErrorException(string message, long nowStateVersion, long eventVersion) : base(message)
        {
            NowStateVersion = nowStateVersion;
            EventVersion = eventVersion;
        }

        public VersionErrorException(string message, Exception innerException, long nowStateVersion,
            long eventVersion) : base(message, innerException)
        {
            NowStateVersion = nowStateVersion;
            EventVersion = eventVersion;
        }

        public long NowStateVersion { get; }
        public long EventVersion { get; }
    }
}