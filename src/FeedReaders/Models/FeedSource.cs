using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FeedReaders.Models
{
    /// <summary>
    /// This specifies the feed source.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FeedSource
    {
        /// <summary>
        /// Identifies no feed source selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Identifies a blog is the feed source.
        /// </summary>
        Blog = 1,

        /// <summary>
        /// Identifies a YouTube channel is the feed source.
        /// </summary>
        YouTube = 2,
    }
}