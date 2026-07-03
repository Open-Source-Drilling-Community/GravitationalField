using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using System;

namespace NORCE.Drilling.GravitationalField.Model
{
    /// <summary>
    /// a base class other classes may derive from
    /// </summary>
    public class GravitationalData
    {
        /// <summary>
        /// Double containing Latitude
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// Double containing Longitude
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// Double Depth
        /// </summary>
        public double Depth { get; set; }
        /// <summary>
        /// Double with the x component of the gravity
        /// </summary>
        public double? GravitatyIntensityX { get; set; } 
        /// <summary>
        /// Double with the y component of the gravity
        /// </summary>
        public double? GravitatyIntensityY { get; set; } 
        /// <summary>
        /// Double with the z component of the gravity
        /// </summary>
        public double? GravitatyIntensityZ { get; set; } 


        /// <summary>
        /// default constructor required for JSON serialization
        /// </summary>
        public GravitationalData() : base()
        {
        }
    }
}
