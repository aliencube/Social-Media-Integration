using FeedReaders.FunctionApp.Configs;
using FeedReaders.FunctionApp.Handlers;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FeedReaders.FunctionApp.StartUp))]
namespace FeedReaders.FunctionApp
{
    /// <summary>
    /// This represents the entity to be invoked during the runtime startup.
    /// </summary>
    public class StartUp : FunctionsStartup
    {
        /// <inheritdoc />
        public override void Configure(IFunctionsHostBuilder builder)
        {
            this.ConfigureConfigurations(builder.Services);
            this.ConfigureHandlers(builder.Services);
            this.ConfigureReaders(builder.Services);
        }
        
        private void ConfigureConfigurations(IServiceCollection services)
        {
            services.AddSingleton<AppSettings>();
        }

        private void ConfigureHandlers(IServiceCollection services)
        {
            services.AddTransient<IRequestPayloadHandler, RequestPayloadHandler>();
        }

        private void ConfigureReaders(IServiceCollection services)
        {
            services.AddTransient<YouTubeFeedReader>();
            services.AddTransient<IFeedReaderResolver, FeedReaderResolver>();
        }
    }
}