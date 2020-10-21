using System.Collections.Generic;
using System.Linq;

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
        /// <param name="items">List of <see cref="FeedItem"/> instances.</param>
        /// <returns>Returns the list of <see cref="FeedReaderResponse"/> instances.</returns>
        public static List<FeedReaderResponse> Clone(List<FeedItem> items)
        {
            var responses = items.Select(item => new FeedReaderResponse()
            {
                Title = item.Title,
                Description = item.Description,
                Link = item.Link,
                ThumbnailLink = item.ThumbnailLink,
            }).ToList();

            return responses;
        }

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
                DatePublished = item.DatePublished,
            };

            return response;
        }
    }
}