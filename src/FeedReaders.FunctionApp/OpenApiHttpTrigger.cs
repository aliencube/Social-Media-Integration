using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

using Aliencube.AzureFunctions.Extensions.OpenApi.Abstractions;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Extensions;

using FeedReaders.FunctionApp.Configs;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;

namespace Aliencube.AzureFunctions.FunctionAppV3
{
    /// <summary>
    /// This represents the HTTP trigger for Open API.
    /// </summary>
    public class OpenApiHttpTrigger
    {
        private readonly AppSettings _settings;
        private readonly IDocument _document;
        private readonly ISwaggerUI _ui;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenApiHttpTrigger"/> class.
        /// </summary>
        /// <param name="settings"><see cref="AppSettings"/> instance.</param>
        /// <param name="document"><see cref="IDocument"/> instance.</param>
        /// <param name="ui"><see cref="ISwaggerUI"/> instance.</param>
        public OpenApiHttpTrigger(AppSettings settings, IDocument document, ISwaggerUI ui)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._document = document ?? throw new ArgumentNullException(nameof(document));
            this._ui = ui ?? throw new ArgumentNullException(nameof(ui));
        }

        /// <summary>
        /// Renders the Swagger document, ie. swagger.json
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Returns swagger.json</returns>
        [FunctionName(nameof(OpenApiHttpTrigger.RenderSwaggerDocument))]
        [OpenApiIgnore]
        public async Task<IActionResult> RenderSwaggerDocument(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger.json")] HttpRequest req,
            ILogger log)
        {
            var version = OpenApiSpecVersion.OpenApi2_0;
            var format = OpenApiFormat.Json;
            var contentType = format.GetContentType();
            var assembly = Assembly.GetExecutingAssembly();

            var result = await this._document
                                   .InitialiseDocument()
                                   .AddMetadata(this._settings.OpenApiInfo)
                                   .AddServer(req, this._settings.HttpSettings.RoutePrefix)
                                   .Build(assembly)
                                   .RenderAsync(version, format)
                                   .ConfigureAwait(false);

            var content = new ContentResult()
                              {
                                  Content = result,
                                  ContentType = contentType,
                                  StatusCode = (int)HttpStatusCode.OK
                              };

            return content;
        }

        /// <summary>
        /// Renders the Swagger UI page.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/> instance.</param>
        /// <param name="log"><see cref="ILogger"/> instance.</param>
        /// <returns>Swagger UI in HTML.</returns>
        [FunctionName(nameof(OpenApiHttpTrigger.RenderSwaggerUI))]
        [OpenApiIgnore]
        public async Task<IActionResult> RenderSwaggerUI(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "swagger/ui")] HttpRequest req,
            ILogger log)
        {
            var endpoint = "swagger.json";
            var result = await this._ui
                                   .AddMetadata(this._settings.OpenApiInfo)
                                   .AddServer(req, this._settings.HttpSettings.RoutePrefix)
                                   .BuildAsync()
                                   .RenderAsync(endpoint, this._settings.SwaggerAuthKey)
                                   .ConfigureAwait(false);

            var content = new ContentResult()
                              {
                                  Content = result,
                                  ContentType = "text/html",
                                  StatusCode = (int)HttpStatusCode.OK
                              };

            return content;
        }
    }
}