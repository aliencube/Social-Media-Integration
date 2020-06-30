using System.Collections.Generic;

namespace FeedReaders.Models
{
    /// <summary>
    /// This represents the context entity for feed reader.
    /// </summary>
    public class FeedReaderContext
    {
        /// <summary>
        /// Gets or sets the feed URI.
        /// </summary>
        public virtual string FeedUri { get; set; }

        /// <summary>
        /// Gets or sets the number of feed items to return.
        /// </summary>
        public virtual int NumberOfFeedItems { get; set; } = 10;

        /// <summary>
        /// Gets or sets the value indicating whether to take one feed item randomly or not.
        /// </summary>
        public virtual bool IsRandom { get; set; } = false;

        /// <summary>
        /// Gets or sets the list of prefixes that the feed item title to include.
        /// </summary>
        public virtual List<string> PrefixesIncluded { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the list of prefixes that the feed item title to exclude.
        /// </summary>
        public virtual List<string> PrefixesExcluded { get; set; } = new List<string>();
    }
}