using System;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;

using FeedReaders.Models;

namespace FeedReaders
{
    /// <summary>
    /// This represents the feed reader entity. This MUST be inherited.
    /// </summary>
    public abstract class FeedReader : IFeedReader
    {
        private readonly Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReader"/> class.
        /// </summary>
        protected FeedReader()
        {
            this._random = new Random();
        }

        /// <inheritdoc />
        public virtual async Task<FeedItem> GetFeedItemAsync(FeedReaderContext context)
        {
            return await Task.FromResult(default(FeedItem));
        }

        /// <summary>
        /// Loads the <see cref="SyndicationFeed"/> from the given feed URI.
        /// </summary>
        /// <param name="feedUri">Feed URI.</param>
        /// <returns>Returns the <see cref="SyndicationFeed"/> instance.</returns>
        protected async Task<SyndicationFeed> LoadFeedAsync(Uri feedUri)
        {
            return await this.LoadFeedAsync(feedUri.ToString()).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the <see cref="SyndicationFeed"/> from the given feed URI.
        /// </summary>
        /// <param name="feedUri">Feed URI.</param>
        /// <returns>Returns the <see cref="SyndicationFeed"/> instance.</returns>
        protected async Task<SyndicationFeed> LoadFeedAsync(string feedUri)
        {
            var feed = await Task.Factory.StartNew(() =>
            {
                using (var reader = XmlReader.Create(feedUri))
                {
                    return SyndicationFeed.Load(reader);
                }
            });

            return feed;
        }

        /// <summary>
        /// Gets the skip number.
        /// </summary>
        /// <param name="maxCount">Maximum number to skip.</param>
        /// <param name="isRandom">Value indicating whether to return a random skip number or not.</param>
        /// <returns>Returns the skip number.</returns>
        protected int GetSkipNumber(int maxCount, bool isRandom)
        {
            if (!isRandom)
            {
                return 0;
            }

            var index = this._random.Next(maxCount);

            return index;
        }

        /// <summary>
        /// Builds the <see cref="FeedItem"/> instance.
        /// </summary>
        /// <param name="item"><see cref="SyndicationItem"/> instance.</param>
        /// <returns>Returns the <see cref="FeedItem"/> instance.</returns>
        protected virtual FeedItem BuildFeedItem(SyndicationItem item)
        {
            return new FeedItem();
        }
    }
}