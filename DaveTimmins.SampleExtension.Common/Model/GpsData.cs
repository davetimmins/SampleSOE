using Newtonsoft.Json;

namespace DaveTimmins.SampleExtension.Common.Model
{
    [JsonObject]
    public class GpsData : JsonableObject
    {
        /// <summary>
        /// Name of the layer in the map document to interact with
        /// </summary>
        [JsonProperty("lyr")]
        public string LayerName { get; set; }
        
        /// <summary>
        /// Flag indicating whether or not a GPS fix was obtained
        /// </summary>
        [JsonProperty("fix")]
        public bool FixAcquired { get; set; }

        /// <summary>
        /// The number of satellites in the constellation for the GPS data if the fix was obtained
        /// </summary>
        [JsonProperty("sats")]
        public int NumberOfSatellites { get; set; }

        /// <summary>
        /// The coordinate of the GPS data if the fix was obtained
        /// </summary>
        [JsonProperty("pt")]
        public Location Location { get; set; }
    }
}
