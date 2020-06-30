using FeedReaders.Models;

namespace FeedReaders.FunctionApp.Models
{
    /// <summary>
    /// This represents the request entity for feed reader.
    /// </summary>
    public class FeedReaderRequest : FeedReaderContext
    {
        /// <summary>
        /// Gets or sets the value of <see cref="FeedSource"/>.
        /// </summary>
        public virtual FeedSource FeedSource { get; set; }
    }
}