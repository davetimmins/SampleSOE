using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.EnterpriseServices;
using DaveTimmins.SampleExtension.Common.Model;
using DaveTimmins.SampleExtension.Extensions;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SOESupport;

namespace DaveTimmins.SampleExtension
{
    [ComVisible(true)]
    [Guid("bb5b053e-c91e-444c-8c59-5d9e41dbf7bd")]    
    [ClassInterface(ClassInterfaceType.None)]
    public class Extension : ServicedComponent, IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {
        private string _soeName;
        private IServerObjectHelper _serverObjectHelper;
        private ServerLogger _logger;
        private IRESTRequestHandler _reqHandler;

        public Extension()
        {
            _soeName = Common.Names.SoeName;
            _logger = new ServerLogger();
            _reqHandler = new SoeRestImpl(_soeName, CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        public void Init(IServerObjectHelper pSOH)
        {
            _serverObjectHelper = pSOH;
        }

        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        public void Construct(IPropertySet props)
        {
        }

        #endregion

        #region IRESTRequestHandler Members

        public string GetSchema()
        {
            return _reqHandler.GetSchema();
        }

        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat,  string requestProperties, out string responseProperties)
        {
            return _reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            var soeResource = new RestResource(_soeName, false, RootResHandler);

            // Add the operations
            var operations = new List<RestOperation>();

            var sampleRestOperation = new RestOperation(
                Common.Names.SampleOperationName,
                new[] { typeof(GpsData).Name },
                new[] { "json" },
                SampleOperationHandler,
                true);
            operations.Add(sampleRestOperation);

            // TODO : add more operations as needed
            
            soeResource.operations = operations;

            return soeResource;
        }

        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            var result = new JsonObject();
            result.AddString("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            return Encoding.UTF8.GetBytes(result.ToJson());
        }

        private byte[] SampleOperationHandler(NameValueCollection boundVariables,
                                              JsonObject operationInput,
                                              string outputFormat,
                                              string requestProperties,
                                              out string responseProperties)
        {
            var result = new OperationResult();
            responseProperties = null;

            JsonObject jsonObj;
            bool found = operationInput.TryGetJsonObject(typeof(GpsData).Name, out jsonObj);
            if (!found)
                throw new ArgumentNullException(typeof(GpsData).Name);
            var gpsData = JsonableObject.FromJson<GpsData>(jsonObj.ToJson());

            IMapLayerInfo layer = _serverObjectHelper.ServerObject.FindLayer(gpsData.LayerName);
            if (layer == null)
            {
                result.Success = false;
                return Encoding.UTF8.GetBytes(result.ToJson());
            }

            try
            {
                // TODO : add some ArcObjects code here to do whatever needed
                result.ExtraData = string.Format("Layer '{0}' has feature class '{1}'", layer.Name,
                                                 _serverObjectHelper.ServerObject.GetFeatureClassFromLayer(layer).AliasName);
                result.Success = true;
            }
            catch (COMException comExc)
            {
                // Handle any errors that might occur.
                _logger.LogMessage(ServerLogger.msgType.error, "SampleOperationHandler", 666, comExc.Message);
                result.Success = false;
            }
            finally
            {
                // TODO : release any COM handles here, flush buffers etc.
            }
            
            return Encoding.UTF8.GetBytes(result.ToJson());
        } 

    }
}
