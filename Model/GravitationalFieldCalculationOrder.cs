using OSDC.DotnetLibraries.General.DataManagement;
using System;
using System.IO;
using System.Collections.Generic;
using GeographicLib;
namespace NORCE.Drilling.GravitationalField.Model
{   


    public class GravitationalFieldCalculationOrder : GravitationalFieldCalculationOrderLight 
    {
        /// <summary>
        /// a MetaInfo for the GravitationalFieldCalculationOrder
        /// </summary>
        public MetaInfo? MetaInfo { get; set; }

        /// <summary>
        /// name of the data
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// a description of the data
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// the date when the data was created
        /// </summary>
        public DateTimeOffset? CreationDate { get; set; }

        /// <summary>
        /// the date when the data was last modified
        /// </summary>
        public DateTimeOffset? LastModificationDate { get; set; }
    
        /// <summary>
        /// an input list of GravitationalField
        /// </summary>
        public GravitationalField RawGravitationalField { get; set; }

        /// <summary>
        /// an output list of GravitationalField
        /// </summary>
        public GravitationalField ? CompletedGravitationalField { get; set; }

        /// <summary>
        /// default constructor required for JSON serialization
        /// </summary>
        public GravitationalFieldCalculationOrder() : base()
        {
        }

        /// <summary>
        /// main calculation method of the GravitationalFieldCalculationOrder
        /// </summary>
        /// <returns></returns>
        public bool Calculate()
        {
            bool success = false;
            if (RawGravitationalField.GravitationalDataTable != null)
            {
                if (RawGravitationalField.GravitationalDataTable.Count > 0)
                {
                    string gravityModelPath;
                    string baseDir = Path.Combine(AppContext.BaseDirectory, "GravityModelFiles");
                    if (Directory.Exists(baseDir))
                    {
                        gravityModelPath = baseDir;
                    }
                    else
                    {
                        DirectoryInfo? directory = new(Directory.GetCurrentDirectory());
                        while (directory != null && directory.GetFiles("*.sln").Length == 0)
                        {
                            directory = directory.Parent;
                        }
                        gravityModelPath = Path.Combine(directory!.ToString(), "GravityModelFiles");
                    }
                    List<GravitationalData> gravitationalDataListCompleted = new();
                    GravityModel gravityModel = new GravityModel("egm96", gravityModelPath);
                    foreach (GravitationalData gravitationalData in RawGravitationalField.GravitationalDataTable)
                    {
                        if (!double.IsFinite(gravitationalData.Latitude) ||
                            !double.IsFinite(gravitationalData.Longitude) ||
                            !double.IsFinite(gravitationalData.Depth) ||
                            gravitationalData.Latitude < -Math.PI / 2.0 ||
                            gravitationalData.Latitude > Math.PI / 2.0 ||
                            gravitationalData.Longitude < -Math.PI ||
                            gravitationalData.Longitude > Math.PI)
                        {
                            return false;
                        }

                        // GeographicLib expects degrees and ellipsoidal height. The public
                        // OSDC contract uses radians and true vertical depth positive downward.
                        double latitudeDegrees = gravitationalData.Latitude * 180.0 / Math.PI;
                        double longitudeDegrees = gravitationalData.Longitude * 180.0 / Math.PI;
                        (double aux, double gx, double gy, double gz) = gravityModel.Gravity(latitudeDegrees, longitudeDegrees, -gravitationalData.Depth);
                        GravitationalData gravitationalDataCompleted = new GravitationalData
                            {
                                Latitude = gravitationalData.Latitude,
                                Longitude = gravitationalData.Longitude,
                                Depth = gravitationalData.Depth,
                                GravityIntensityX = gx,
                                GravityIntensityY = gy,
                                GravityIntensityZ = gz
                            };
                        gravitationalDataListCompleted.Add(gravitationalDataCompleted);
                    }
                    CompletedGravitationalField = new GravitationalField
                    {
                        MetaInfo = new MetaInfo
                        {
                            ID = Guid.NewGuid(),
                            HttpEndPoint = MetaInfo!.HttpEndPoint,
                            HttpHostBasePath = MetaInfo!.HttpHostBasePath,
                            HttpHostName = MetaInfo!.HttpHostName                            
                        },
                        Name = RawGravitationalField.Name + "(Completed)",
                        Description = "Completed from: " + RawGravitationalField.Name ,                        
                        CreationDate = CreationDate,
                        LastModificationDate = LastModificationDate,
                        GravitationalDataTable = gravitationalDataListCompleted,
                        Type = GravitationalFieldType.Completed

                    };
                    success = true;            
                }
            }
            return success;
        }
    }
}
