using System;
using System.Reflection;

using FeedReaders.Models;

namespace FeedReaders
{
    /// <summary>
    /// This represents the resolver entity for feed reader.
    /// </summary>
    public class FeedReaderResolver : IFeedReaderResolver
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderResolver"/> class.
        /// </summary>
        /// <param name="provider"><see cref="FeedReaderResolver"/> instance.</param>
        public FeedReaderResolver(IServiceProvider provider)
        {
            this._provider = provider;
        }

        /// <inheritdoc />
        public IFeedReader Resolve(FeedSource source)
        {
            var type = Assembly.GetAssembly(typeof(FeedReaderResolver)).GetType($"FeedReaders.{source}FeedReader");
            var instance = this._provider.GetService(type);

            return instance as IFeedReader;
        }
    }
}