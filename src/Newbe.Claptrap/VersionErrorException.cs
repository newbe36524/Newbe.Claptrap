using System;

namespace Newbe.Claptrap
{
    public class VersionErrorException : Exception
    {
        public VersionErrorException(ulong nowStateVersion, ulong eventVersion)
            : this($"version error, current state version : {nowStateVersion}, eventVersion : {eventVersion}",
                nowStateVersion,
                eventVersion)
        {
        }

        public VersionErrorException(string message, ulong nowStateVersion, ulong eventVersion) : base(message)
        {
            NowStateVersion = nowStateVersion;
            EventVersion = eventVersion;
        }

        public VersionErrorException(string message, Exception innerException, ulong nowStateVersion,
            ulong eventVersion) : base(message, innerException)
        {
            NowStateVersion = nowStateVersion;
            EventVersion = eventVersion;
        }

        public ulong NowStateVersion { get; set; }
        public ulong EventVersion { get; set; }
    }
}