using Aliencube.AzureFunctions.Extensions.OpenApi.Configurations;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace FeedReaders.FunctionApp.Configs
{
    public class AppSettings : OpenApiAppSettingsBase
    {
        public AppSettings() : base()
        {
            var settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() },
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                Converters = { new StringEnumConverter() }
            };

            this.JsonSerializerSettings = settings;
        }

        public virtual JsonSerializerSettings JsonSerializerSettings { get; }
    }
}