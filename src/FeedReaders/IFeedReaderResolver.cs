using FeedReaders.Models;

namespace FeedReaders
{
    /// <summary>
    /// This provides interfaces to <see cref="FeedReaderResolver"/>.
    /// </summary>
    public interface IFeedReaderResolver
    {
        /// <summary>
        /// Resolve an instance given the <see cref="FeedSource"/> value.
        /// </summary>
        /// <param name="source"><see cref="FeedSource"/> value.</param>
        /// <returns>Returns the <see cref="IFeedReader"/> instance.</returns>
        IFeedReader Resolve(FeedSource source);
    }
}