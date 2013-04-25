using System;
using DaveTimmins.SampleExtension.Client;
using DaveTimmins.SampleExtension.Common.Model;

namespace DaveTimmins.SampleExtension.ClientTest
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCallSampleOperation_Click(object sender, EventArgs e)
        {
            var gpsData = new GpsData
            {
                LayerName = "GPS Waypoints", 
                FixAcquired = true, 
                NumberOfSatellites = 4,
                Location = new Location
                {
                 Longitude = 175.495605,
                 Latitude = -39.546977
                }
            };

            var result = OperationHelper.CallSampleOperation(@"http://localhost/ArcGIS/rest/services/Test/Blah/MapServer", gpsData);
        }
    }
}