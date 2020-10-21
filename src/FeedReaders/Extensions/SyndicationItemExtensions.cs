using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;

namespace FeedReaders.Extensions
{
    /// <summary>
    /// This represents the extension entity for <see cref="SyndicationItem"/>.
    /// </summary>
    public static class SyndicationItemExtensions
    {
        /// <summary>
        /// Gets the list of <see cref="SyndicationItem"/> instances filtered by included lists.
        /// </summary>
        /// <param name="items">List of <see cref="SyndicationItem"/> instances.</param>
        /// <param name="prefixesIncluded">List of prefixes to include.</param>
        /// <returns>Returns the list of <see cref="SyndicationItem"/> instances filtered.</returns>
        public static IEnumerable<SyndicationItem> FilterIncluded(this IEnumerable<SyndicationItem> items, List<string> prefixesIncluded)
        {
            if (prefixesIncluded == null)
            {
                return items;
            }

            prefixesIncluded.Remove(string.Empty);
            if (!prefixesIncluded.Any())
            {
                return items;
            }

            var result = items.Where(item => prefixesIncluded.Any(prefix => item.Title.Text.StartsWith(prefix, ignoreCase: true, CultureInfo.InvariantCulture)));

            return result;
        }

        /// <summary>
        /// Gets the list of <see cref="SyndicationItem"/> instances filtered by excluded lists.
        /// </summary>
        /// <param name="items">List of <see cref="SyndicationItem"/> instances.</param>
        /// <param name="prefixesExcluded">List of prefixes to exclude.</param>
        /// <returns>Returns the list of <see cref="SyndicationItem"/> instances filtered.</returns>
        public static IEnumerable<SyndicationItem> FilterExcluded(this IEnumerable<SyndicationItem> items, List<string> prefixesExcluded)
        {
            if (prefixesExcluded == null)
            {
                return items;
            }

            prefixesExcluded.Remove(string.Empty);
            if (!prefixesExcluded.Any())
            {
                return items;
            }

            var result = items.Where(item => !prefixesExcluded.Any(prefix => item.Title.Text.StartsWith(prefix, ignoreCase: true, CultureInfo.InvariantCulture)));

            return result;
        }
    }
}