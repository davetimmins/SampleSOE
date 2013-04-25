using Newtonsoft.Json;

namespace DaveTimmins.SampleExtension.Common.Model
{
    [JsonObject]
    public class Location : JsonableObject
    {
        [JsonProperty("x")]
        public double Longitude { get; set; }

        [JsonProperty("y")]
        public double Latitude { get; set; }

        public ESRI.ArcGIS.Geometry.Point ToEsriPoint()
        {
            return new ESRI.ArcGIS.Geometry.Point { X = Longitude, Y = Latitude };
        }
    }
}
