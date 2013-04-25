using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaveTimmins.SampleExtension.Register
{
    class Program
    {
        static void Main(string[] args)
        {
            // Must run as an user in the agsadmin group on the SOM
            var agsServerConnection = new ESRI.ArcGIS.ADF.Connection.AGS.AGSServerConnection { Host = "localhost" };
            agsServerConnection.Connect();

            var serverObjectAdmin = agsServerConnection.ServerObjectAdmin as ESRI.ArcGIS.Server.IServerObjectAdmin2;
            if (serverObjectAdmin == null)
                throw new NullReferenceException("agsServerConnection.ServerObjectAdmin as ESRI.ArcGIS.Server.IServerObjectAdmin2");

            // This name must match those defined for property pages 
            string extensionName = Common.Names.SoeName;

            // Check command line arguments to see if SOE is to be unregistered
            if (args != null && args.Length == 1 && string.Equals(args.First(), "/unregister"))
            {
                // Check whether the SOE is registered
                if (ExtensionRegistered(serverObjectAdmin, extensionName))
                {
                    // Delete the SOE
                    serverObjectAdmin.DeleteExtensionType("MapServer", extensionName);
                    Console.WriteLine(extensionName + " successfully unregistered");
                }
                else
                    Console.WriteLine(extensionName + " is not registered with ArcGIS Server");
            }
            else
            {
                // Check whether the SOE is registered
                if (!ExtensionRegistered(serverObjectAdmin, extensionName))
                {
                    // Use IServerObjectExtensionType3 to get access to info properties
                    var serverObjectExtensionType = serverObjectAdmin.CreateExtensionType() as ESRI.ArcGIS.Server.IServerObjectExtensionType3;

                    // Must match the namespace and class name of the class implementing IServerObjectExtension
                    if (serverObjectExtensionType != null)
                    {
                        serverObjectExtensionType.CLSID = "DaveTimmins.SampleExtension.Extension";
                        serverObjectExtensionType.Description = "Extends ArcGIS Server to allow custom operations.";
                        serverObjectExtensionType.Name = extensionName;

                        // Name that will be shown in the capabilities list on property pages
                        serverObjectExtensionType.DisplayName = "Sample SOE Extension";

                        // Required to support MSD
                        serverObjectExtensionType.Info.SetProperty("SupportsMSD", "true");

                        // Required to enable exposure of SOE with ArcGIS Server REST endpoint
                        serverObjectExtensionType.Info.SetProperty("SupportsREST", "true");

                        // Register the SOE with the server
                        serverObjectAdmin.AddExtensionType("MapServer", serverObjectExtensionType);

                        Console.WriteLine(extensionName + " successfully registered with ArcGIS Server");
                    }
                }
                else
                    Console.WriteLine(extensionName + " is already registered with ArcGIS Server");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Checks whether an extension with the extensionName is already registered with the serverObjectAdmin
        /// </summary>
        /// <param name="serverObjectAdmin"></param>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        static private bool ExtensionRegistered(ESRI.ArcGIS.Server.IServerObjectAdmin2 serverObjectAdmin, string extensionName)
        {
            // Get the extensions that extend MapServer server objects
            ESRI.ArcGIS.Server.IEnumServerObjectExtensionType extensionTypes = serverObjectAdmin.GetExtensionTypes("MapServer");
            extensionTypes.Reset();

            // If an extension with a name matching that passed-in is found, return true
            ESRI.ArcGIS.Server.IServerObjectExtensionType extensionType = extensionTypes.Next();
            while (extensionType != null)
            {
                if (string.Equals(extensionType.Name, extensionName))
                    return true;

                extensionType = extensionTypes.Next();
            }

            // No matching extension was found, so return false
            return false;
        }
    }
}
