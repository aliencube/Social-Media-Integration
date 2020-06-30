using System.Threading.Tasks;

using FeedReaders.Models;

namespace FeedReaders
{
    /// <summary>
    /// This provides interfaces to <see cref="FeedReader"/>.
    /// </summary>
    public interface IFeedReader
    {
        /// <summary>
        /// Gets the <see cref="FeedItem"/> instance.
        /// </summary>
        /// <param name="context"><see cref="FeedReaderContext"/> instance.</param>
        /// <returns>Returns the <see cref="FeedItem"/> instance.</returns>
        Task<FeedItem> GetFeedItemAsync(FeedReaderContext context);
    }
}