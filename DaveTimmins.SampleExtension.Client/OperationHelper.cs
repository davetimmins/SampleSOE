using System.Collections.Generic;
using DaveTimmins.SampleExtension.Common;
using DaveTimmins.SampleExtension.Common.Model;

namespace DaveTimmins.SampleExtension.Client
{
    public static class OperationHelper
    {
        public static OperationResult CallSampleOperation(string restEndpoint, GpsData gpsData)
        {
            return CallSampleOperation(restEndpoint, gpsData, string.Empty);
        }

        /// <summary>
        /// Helper method that encapsulates the call to the ArcGIS Server REST SOE for the sample operation
        /// </summary>
        /// <param name="restEndpoint">Url of the REST endpoint of the MapServer</param>
        /// <param name="gpsData"><c>GpsData</c> to be used by the SOE</param>
        /// <param name="proxyUrl">Url of the proxy page used to provide secure access to the service</param>
        /// <returns>An <c>OperationResult</c> which indicates the success or failure of the request</returns>
        public static OperationResult CallSampleOperation(string restEndpoint, GpsData gpsData, string proxyUrl)
        {
            var fullUrl = string.Format("{0}/exts/{1}/{2}", restEndpoint.TrimEnd('/'), Names.SoeName, Names.SampleOperationName);

            var parameters = new Dictionary<string, string> { { typeof(GpsData).Name, gpsData.ToJson() } };

            var request = new Request(fullUrl, proxyUrl);
            return request.SubmitRequest<OperationResult>(parameters);
        }
    }
}
