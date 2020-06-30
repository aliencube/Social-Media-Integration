using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml.Linq;

using FeedReaders.Extensions;
using FeedReaders.Models;

namespace FeedReaders
{
    /// <summary>
    /// This represents the feed reader entity for YouTube.
    /// </summary>
    public class YouTubeFeedReader : FeedReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="YouTubeFeedReader"/> class.
        /// </summary>
        public YouTubeFeedReader()
            : base()
        {
        }

        /// <inheritdoc />
        public override async Task<FeedItem> GetFeedItemAsync(FeedReaderContext context)
        {
            var feed = await this.LoadFeedAsync(context.FeedUri).ConfigureAwait(false);

            var items = feed.Items
                            .FilterIncluded(context.PrefixesIncluded)
                            .FilterExcluded(context.PrefixesExcluded)
                            .Take(context.NumberOfFeedItems)
                            .ToList();

            var index = this.GetSkipNumber(context.NumberOfFeedItems > items.Count ? items.Count : context.NumberOfFeedItems, context.IsRandom);
            var item = items.Skip(index).Take(1).SingleOrDefault();
            var feedItem = this.BuildFeedItem(item);

            return feedItem;
        }

        /// <inheritdoc />
        protected override FeedItem BuildFeedItem(SyndicationItem item)
        {
            var group = item.ElementExtensions.FirstOrDefault(p => p.OuterName == "group").GetObject<XElement>();
            var title = group.Elements().SingleOrDefault(p => p.Name.LocalName == "title").Value;
            var description = group.Elements().SingleOrDefault(p => p.Name.LocalName == "description").Value;
            var thumbnailLink = group.Elements().SingleOrDefault(p => p.Name.LocalName == "thumbnail").Attribute("url").Value;

            var feedItem = new FeedItem()
            {
                Title = item.Title.Text,
                Description = description.Substring(0, description.IndexOf("*") < 0 ? description.IndexOf("\\n") : description.IndexOf("*")),
                Link = item.Links.First().Uri.ToString(),
                ThumbnailLink = thumbnailLink,
            };

            return feedItem;
        }
    }
}