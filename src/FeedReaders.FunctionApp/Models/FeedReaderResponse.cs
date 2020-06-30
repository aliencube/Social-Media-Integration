using FeedReaders.Models;

namespace FeedReaders.FunctionApp.Models
{
    /// <summary>
    /// This represents the response entity for feed item.
    /// </summary>
    public class FeedReaderResponse : FeedItem
    {
        /// <summary>
        /// Clones the <see cref="FeedItem"/> instance to <see cref="FeedReaderResponse"/>.
        /// </summary>
        /// <param name="item"><see cref="FeedItem"/> instance.</param>
        /// <returns>Returns the <see cref="FeedReaderResponse"/> instance.</returns>
        public static FeedReaderResponse Clone(FeedItem item)
        {
            var response = new FeedReaderResponse()
            {
                Title = item.Title,
                Description = item.Description,
                Link = item.Link,
                ThumbnailLink = item.ThumbnailLink,
            };

            return response;
        }
    }
}