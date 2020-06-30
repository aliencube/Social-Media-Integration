using System.IO;
using System.Threading.Tasks;

namespace FeedReaders.FunctionApp.Handlers
{
    /// <summary>
    /// This provides interfaces to <see cref="RequestPayloadHandler"/>.
    /// </summary>
    public interface IRequestPayloadHandler
    {
        /// <summary>
        /// Deserialises the request body stream.
        /// </summary>
        /// <param name="stream"><see cref="Stream"/> instance.</param>
        /// <typeparam name="T">Type to be deserialised.</typeparam>
        /// <returns>Returns the payload deserialised.</returns>
        Task<T> DeserialiseAsync<T>(Stream stream);
    }
}