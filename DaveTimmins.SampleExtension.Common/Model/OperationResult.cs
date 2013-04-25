using Newtonsoft.Json;

namespace DaveTimmins.SampleExtension.Common.Model
{
    [JsonObject]
    public class OperationResult : JsonableObject
    {
        [JsonProperty("ok")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public string ExtraData { get; set; }
    }
}
