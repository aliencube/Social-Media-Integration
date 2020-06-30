using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FeedReaders.FunctionApp.Handlers
{
    public class RequestPayloadHandler : IRequestPayloadHandler
    {
        /// <inheritdoc />
        public async Task<T> DeserialiseAsync<T>(Stream stream)
        {
            var deserialised = default(T);
            using (var reader = new StreamReader(stream))
            {
                var serialised = await reader.ReadToEndAsync();
                deserialised = JsonConvert.DeserializeObject<T>(serialised);
            }
            
            return deserialised;
        }
    }
}