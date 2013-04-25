using Newtonsoft.Json;

namespace DaveTimmins.SampleExtension.Common.Model
{
    public abstract class JsonableObject
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T FromJson<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }   
    }
}
