// ReSharper disable InconsistentNaming

#pragma warning disable 8618
namespace Newbe.Claptrap.Dapr.Core
{
    public class EventJsonModel
    {
        /// <summary>
        /// ClaptrapTypeCode
        /// </summary>
        public string ctc { get; set; }

        /// <summary>
        /// ClaptrapId
        /// </summary>
        public string cid { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public long v { get; set; }

        /// <summary>
        /// EventTypeCode
        /// </summary>
        public string etc { get; set; }

        /// <summary>
        /// DataJson
        /// </summary>
        public string d { get; set; }
    }
}
#pragma warning restore 8618