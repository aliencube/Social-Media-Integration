using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Enums;

using FeedReaders.FunctionApp.Handlers;
using FeedReaders.FunctionApp.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FeedReaders.FunctionApp
{
    /// <summary>
    /// This represents the HTTP trigger entity for feed reader.
    /// </summary>
    public class FeedReaderHttpTrigger
    {
        private readonly IRequestPayloadHandler _handler;
        private readonly IFeedReaderResolver _resolver;
        /// <summary>
        /// Initializes a new instance of the <see cref="FeedReaderHttpTrigger"/> class.
        /// </summary>
        /// <param name="handler"><see cref="IRequestPayloadHandler"/> instance.</param>
        /// <param name="resolver"><see cref="IFeedReaderResolver"/> instance.</param>
        public FeedReaderHttpTrigger(IRequestPayloadHandler handler, IFeedReaderResolver resolver)
        {
            this._handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this._resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        /// <summary>
        /// Gets the feed item.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the list of feed items as a response.</returns>
        [FunctionName(nameof(FeedReaderHttpTrigger.GetFeedItemsAsync))]
        [OpenApiOperation(operationId: "getFeedItems", tags: new[] { "feedItem" }, Summary = "Gets a list of feed items from the given feed", Description = "This operation returns a list of feed items from the given feed URI.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(FeedReaderRequest), Description = "Feed reader request payload")]
        [OpenApiResponseBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<FeedReaderResponse>), Summary = "List of feed reader response payload")]
        public async Task<IActionResult> GetFeedItemsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "feeds/items")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var request = await this._handler
                                    .DeserialiseAsync<FeedReaderRequest>(req.Body)
                                    .ConfigureAwait(false);

            var result = default(IActionResult);
            try
            {
                var feedItems = await this._resolver
                                          .Resolve(request.FeedSource)
                                          .GetFeedItemsAsync(request)
                                          .ConfigureAwait(false);
                var responses = FeedReaderResponse.Clone(feedItems);

                result = new OkObjectResult(responses);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                result = new ObjectResult(error)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            } 

            return result;
        }

        /// <summary>
        /// Gets the feed item.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns the feed item as a response.</returns>
        [FunctionName(nameof(FeedReaderHttpTrigger.GetFeedItemAsync))]
        [OpenApiOperation(operationId: "getFeedItem", tags: new[] { "feedItem" }, Summary = "Gets a single feed item from the given feed", Description = "This operation returns a single feed item from the given feed URI.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(FeedReaderRequest), Description = "Feed reader request payload")]
        [OpenApiResponseBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(FeedReaderResponse), Summary = "Feed reader response payload")]
        public async Task<IActionResult> GetFeedItemAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "feeds/item")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var request = await this._handler
                                    .DeserialiseAsync<FeedReaderRequest>(req.Body)
                                    .ConfigureAwait(false);

            var result = default(IActionResult);
            try
            {
                var feedItem = await this._resolver
                                         .Resolve(request.FeedSource)
                                         .GetFeedItemAsync(request)
                                         .ConfigureAwait(false);
                var response = FeedReaderResponse.Clone(feedItem);

                result = new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                result = new ObjectResult(error)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            } 

            return result;
        }
    }
}
