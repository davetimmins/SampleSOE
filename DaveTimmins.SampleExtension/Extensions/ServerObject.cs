using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace DaveTimmins.SampleExtension.Extensions
{
    internal static class ServerObject
    {
        /// <summary>
        /// Locates the map layer that matches the supplied name.
        /// </summary>
        /// <param name="serverObject"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public static IMapLayerInfo FindLayer(this ESRI.ArcGIS.Server.IServerObject serverObject, string layerName)
        {
            // Get the list of layers
            var mapServer = serverObject as IMapServer3;
            if (mapServer == null)
                return null;

            IMapLayerInfos mapLayerInfos = mapServer.GetServerInfo(mapServer.DefaultMapName).MapLayerInfos;

            for (int i = 0; i < mapLayerInfos.Count; i++)
            {
                IMapLayerInfo layer = mapLayerInfos.get_Element(i);

                if (layer == null || !layer.IsFeatureLayer || !string.Equals(layer.Name, layerName))
                    continue;

                return layer;
            }

            return null;
        }

        /// <summary>
        /// Get the feature class that is associated with the supplied layer.
        /// </summary>
        /// <param name="serverObject"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static IFeatureClass GetFeatureClassFromLayer(this ESRI.ArcGIS.Server.IServerObject serverObject, IMapLayerInfo layer)
        {
            if (layer == null)
                return null;

            // Get the list of layers
            var mapServer = serverObject as IMapServer3;
            if (mapServer == null)
                return null;

            var mapServerDataAccess = mapServer as IMapServerDataAccess;
            if (mapServerDataAccess == null)
                return null;

            return mapServerDataAccess.GetDataSource(mapServer.DefaultMapName, layer.ID) as IFeatureClass;
        }

        /// <summary>
        /// Locates the feature class matching the supplied name.
        /// </summary>
        /// <param name="serverObject"></param>
        /// <param name="featureClassName"></param>
        /// <returns></returns>
        public static IFeatureClass FindFeatureClass(this ESRI.ArcGIS.Server.IServerObject serverObject, string featureClassName)
        {
            // Get the list of layers
            var mapServer = serverObject as IMapServer3;
            if (mapServer == null)
                return null;

            var mapServerDataAccess = mapServer as IMapServerDataAccess;
            if (mapServerDataAccess == null)
                return null;

            IMapLayerInfos mapLayerInfos = mapServer.GetServerInfo(mapServer.DefaultMapName).MapLayerInfos;

            for (int i = 0; i < mapLayerInfos.Count; i++)
            {
                IMapLayerInfo layer = mapLayerInfos.get_Element(i);

                if (layer == null || !layer.IsFeatureLayer)
                    continue;

                var featureClass = mapServerDataAccess.GetDataSource(mapServer.DefaultMapName, layer.ID) as IFeatureClass;

                if (featureClass == null || string.IsNullOrEmpty(featureClass.AliasName))
                    continue;

                if (featureClass.AliasName.EndsWith(featureClassName))
                    return featureClass;
            }

            return null;
        }
    }
}
