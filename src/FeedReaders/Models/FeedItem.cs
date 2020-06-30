namespace FeedReaders.Models
{
    /// <summary>
    /// This represents the feed item entity.
    /// </summary>
    public class FeedItem
    {
        /// <summary>
        /// Gets or sets the feed item title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the feed item description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the feed item link URL.
        /// </summary>
        public virtual string Link { get; set; }

        /// <summary>
        /// Gets or sets the feed item thumbnail URL.
        /// </summary>
        public virtual string ThumbnailLink { get; set; }
    }
}